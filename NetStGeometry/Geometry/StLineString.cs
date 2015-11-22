using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Gry.ArcGis.NetStGeometry.Tools;

namespace Gry.ArcGis.NetStGeometry.Geometry
{
    /// <summary>
    /// Represents a LineString of the Esri ST_Geometry format (in Oracle)
    /// </summary>
    public class StLineString : IStGeometry
    {
        private decimal _maxM;
        private decimal _maxZ;
        private decimal _minM;
        private decimal _minZ;

        public StLineString(byte[] bytes)
        {
            Bytes = bytes;

            // Is it a X/M Geometry ?
            bool hasZ, hasM;
            List<byte> header = StGeometryHelper.ExtractHeader(bytes);
            StGeometryHelper.GetGeometryDimension(header, out hasZ, out hasM);
            HasZ = hasZ;
            HasM = hasM;

            // Split bytes in encode values
            List<List<byte>> bytesList = StGeometryHelper.GetEncodedValues(StGeometryHelper.RemoveHeader(bytes));

            // Checks
            if (!HasZ && !HasM && (bytesList.Count < 4 || (bytesList.Count % 2 != 0)))
                throw new ArgumentException("Cannot create geometry from those bytes.");
            if (HasZ && HasM && (bytesList.Count % 4 != 0))
                throw new ArgumentException("Cannot create geometry from those bytes.");
            if ((HasZ || HasM) && (bytesList.Count % 3 != 0))
                throw new ArgumentException("Cannot create geometry from those bytes.");

            // Set geometry values
            SetGeometry(bytesList);

            CalculateMinMaxAndLengthValues();
        }

        public StLineString(List<StPoint> points)
        {
            // There must be 2 points at least
            if (points.Count < 2)
                throw new ArgumentException("A LineString must have 2 points minimum");

            // If the points have Z or M, then the line has them too
            HasZ = points[0].HasZ;
            HasM = points[0].HasM;

            Points = new List<StPoint>();
            foreach (StPoint point in points)
            {
                Points.Add(point);
            }

            CalculateMinMaxAndLengthValues();
            CalculateBytes();
        }

        public StLineString(string wkt)
        {
            if (wkt.ToUpper().Contains("Z"))
                HasZ = true;
            if (wkt.ToUpper().Contains("M"))
                HasM = true;

            Points = new List<StPoint>();

            string coordsText = wkt.ToUpper().Replace("LINESTRING", String.Empty).Replace("Z", String.Empty).Replace("M", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty).Trim();
            string[] points = coordsText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string point in points)
            {
                string[] coords = point.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                decimal x = Convert.ToDecimal(coords[0], CultureInfo.InvariantCulture);
                decimal y = Convert.ToDecimal(coords[1], CultureInfo.InvariantCulture);
                decimal? z = null;
                decimal? m = null;

                if (HasZ && HasM)
                {
                    z = Convert.ToDecimal(coords[2], CultureInfo.InvariantCulture);
                    m = Convert.ToDecimal(coords[3], CultureInfo.InvariantCulture);
                }
                else if (HasZ)
                    z = Convert.ToDecimal(coords[2], CultureInfo.InvariantCulture);
                else if (HasM)
                    m = Convert.ToDecimal(coords[2], CultureInfo.InvariantCulture);

                var p = new StPoint(x, y, z, m);
                Points.Add(p);
            }

            CalculateMinMaxAndLengthValues();
            CalculateBytes();
        }

        public List<StPoint> Points { get; set; }

        public byte[] Header
        {
            get { return new List<byte>(Bytes).GetRange(0, 8).ToArray(); }
        }

        public decimal MaxX { get; private set; }
        public decimal MinX { get; private set; }
        public decimal MaxY { get; private set; }
        public decimal MinY { get; private set; }

        public decimal MaxZ
        {
            get
            {
                if (!HasZ)
                    throw new ArgumentException();

                return _maxZ;
            }
            set { _maxZ = value; }
        }

        public decimal MinZ
        {
            get
            {
                if (!HasZ)
                    throw new ArgumentException();

                return _minZ;
            }
            set { _minZ = value; }
        }

        public decimal MaxM
        {
            get
            {
                if (!HasZ)
                    throw new ArgumentException();

                return _maxM;
            }
            set { _maxM = value; }
        }

        public decimal MinM
        {
            get
            {
                if (!HasZ)
                    throw new ArgumentException();

                return _minM;
            }
            set { _minM = value; }
        }

        public decimal Length2D { get; private set; }
        public decimal Length3D { get; private set; }

        #region IStGeometry Members

        public bool HasZ { get; private set; }
        public bool HasM { get; private set; }
        public byte[] Bytes { get; private set; }
        public string Wkt
        {
            get { return ToString(); }
        }

        #endregion

        private void CalculateMinMaxAndLengthValues()
        {
            MaxX = MinX = Points[0].X.Value;
            MaxY = MinY = Points[0].Y.Value;
            MaxZ = MinZ = (HasZ) ? Points[0].Z.Value : 0;
            MaxM = MinM = (HasM) ? Points[0].M.Value : 0;

            Length2D = Length3D = 0;

            for (int i = 1; i < Points.Count; ++i)
            {
                StPoint point = Points[i];

                // Min & Max
                if (point.X.Value > MaxX)
                    MaxX = point.X.Value;
                if (point.X.Value < MinX)
                    MinX = point.X.Value;

                if (point.Y.Value > MaxY)
                    MaxY = point.Y.Value;
                if (point.Y.Value < MinY)
                    MinY = point.Y.Value;

                if (HasZ)
                {
                    if (point.Z.Value > MaxZ)
                        MaxZ = point.Z.Value;
                    if (point.Z.Value < MinZ)
                        MinZ = point.Z.Value;
                }

                if (HasM)
                {
                    if (point.M.Value > MaxM)
                        MaxM = point.M.Value;
                    if (point.M.Value < MinM)
                        MinM = point.M.Value;
                }

                // Length
                StPoint previousPoint = Points[i - 1];
                decimal x2 = Math.Abs(point.X.Value - previousPoint.X.Value);
                decimal y2 = Math.Abs(point.Y.Value - previousPoint.Y.Value);
                var l2D = (decimal)Math.Sqrt((Math.Pow((double)x2, 2) + Math.Pow((double)y2, 2)));
                Length2D += l2D;

                if (HasZ)
                {
                    decimal z2 = Math.Abs(point.Z.Value - previousPoint.Z.Value);
                    var l3D = (decimal)Math.Sqrt((Math.Pow((double)l2D, 2) + Math.Pow((double)z2, 2)));
                    Length3D += l3D;
                }
            }
        }

        private void SetGeometry(List<List<byte>> bytesList)
        {
            // Bytes are NOT stored in an intuitive way : X1 Y1 Z1   X2 Y2 Z2   X3 Y3 Z3
            // But in a other way : X1 Y1   X2 Y2   X3 Y3   Z1 Z2 Z3
            // Ex : LINESTRING Z (0 0 0, 1 1 1) : {128 168 198 186 141 17} {128 239 136 224 159 19} {144 156 1} {144 156 1} {128 168 214 185 7} {144 156 1}

            if (HasM)
                throw new NotSupportedException("M not managed yet : Code is written, but not tested");

            Points = new List<StPoint>();

            int zIndex = 0;
            int mIndex = 0;
            int count = bytesList.Count;
            if (HasZ && HasM)
            {
                zIndex = bytesList.Count / 4 * 2;
                mIndex = bytesList.Count / 4 * 3;
                count = bytesList.Count / 2;
            }
            else if (HasZ || HasM)
            {
                zIndex = bytesList.Count / 3 * 2;
                mIndex = bytesList.Count / 3 * 2;
                count = bytesList.Count / 3 * 2;
            }

            // The first point will be set normaly
            StPoint point;
            if (HasZ && HasM)
                point = new StPoint(bytesList[0].ToArray(), bytesList[1].ToArray(), bytesList[zIndex].ToArray(), bytesList[mIndex].ToArray());
            else if (HasZ)
                point = new StPoint(bytesList[0].ToArray(), bytesList[1].ToArray(), bytesList[zIndex].ToArray());
            else if (HasM)
                point = new StPoint(bytesList[0].ToArray(), bytesList[1].ToArray(), null, bytesList[mIndex].ToArray());
            else
                point = new StPoint(bytesList[0].ToArray(), bytesList[1].ToArray());
            Points.Add(point);

            // The next point are set with the difference between the current coordinate and the previous one
            StPoint previousPoint = point;
            for (int i = 2; i < count; i += 2)
            {
                byte[] xBytes = StCoordinatesSystemHelper.GetBytesFromDifference(bytesList[i].ToArray(), previousPoint.X.Bytes, StParameters.CoordinatesSystem, StCoordType.X);
                byte[] yBytes = StCoordinatesSystemHelper.GetBytesFromDifference(bytesList[i + 1].ToArray(), previousPoint.Y.Bytes, StParameters.CoordinatesSystem, StCoordType.Y);

                if (HasZ && HasM)
                {
                    byte[] zBytes = StCoordinatesSystemHelper.GetBytesFromDifference(bytesList[zIndex + i / 2].ToArray(), previousPoint.Z.Bytes, StParameters.CoordinatesSystem, StCoordType.Z);
                    byte[] mBytes = StCoordinatesSystemHelper.GetBytesFromDifference(bytesList[mIndex + i / 2].ToArray(), previousPoint.M.Bytes, StParameters.CoordinatesSystem, StCoordType.M);
                    point = new StPoint(xBytes, yBytes, zBytes, mBytes);
                }
                else if (HasZ)
                {
                    byte[] zBytes = StCoordinatesSystemHelper.GetBytesFromDifference(bytesList[zIndex + i / 2].ToArray(), previousPoint.Z.Bytes, StParameters.CoordinatesSystem, StCoordType.Z);
                    point = new StPoint(xBytes, yBytes, zBytes);
                }
                else if (HasM)
                {
                    byte[] mBytes = StCoordinatesSystemHelper.GetBytesFromDifference(bytesList[mIndex + i / 2].ToArray(), previousPoint.M.Bytes, StParameters.CoordinatesSystem, StCoordType.M);
                    point = new StPoint(xBytes, yBytes, null, mBytes);
                }
                else
                    point = new StPoint(xBytes, yBytes);

                Points.Add(point);

                previousPoint = point;
            }
        }

        private void CalculateBytes()
        {
            if (HasM)
                throw new NotSupportedException("M not managed yet : Code is written, but not tested");

            var allBytes = new List<byte>();
            var allZBytes = new List<byte>();
            var allMBytes = new List<byte>();

            // The first point will be set normaly
            List<List<byte>> previousBytesList = StGeometryHelper.GetEncodedValues(StGeometryHelper.RemoveHeader(Points[0].Bytes));
            allBytes.AddRange(previousBytesList[0]); // X
            allBytes.AddRange(previousBytesList[1]); // Y
            if (HasZ)
                allZBytes.AddRange(previousBytesList[2]);
            if (HasM)
                allMBytes.AddRange(previousBytesList[3]);

            // The next point are set with the difference between the current coordinate and the previous one
            for (int i = 1; i < Points.Count; ++i)
            {
                List<List<byte>> bytesList = StGeometryHelper.GetEncodedValues(StGeometryHelper.RemoveHeader(Points[i].Bytes));
                byte[] xBytes = StCoordinatesSystemHelper.GetDifferenceFromBytes(bytesList[0].ToArray(), previousBytesList[0].ToArray(), StParameters.CoordinatesSystem, StCoordType.X);
                allBytes.AddRange(xBytes);

                byte[] yBytes = StCoordinatesSystemHelper.GetDifferenceFromBytes(bytesList[1].ToArray(), previousBytesList[1].ToArray(), StParameters.CoordinatesSystem, StCoordType.Y);
                allBytes.AddRange(yBytes);

                if (HasZ && HasM)
                {
                    byte[] zBytes = StCoordinatesSystemHelper.GetDifferenceFromBytes(bytesList[2].ToArray(), previousBytesList[2].ToArray(), StParameters.CoordinatesSystem, StCoordType.Z);
                    allZBytes.AddRange(zBytes);
                    byte[] mBytes = StCoordinatesSystemHelper.GetDifferenceFromBytes(bytesList[3].ToArray(), previousBytesList[3].ToArray(), StParameters.CoordinatesSystem, StCoordType.M);
                    allMBytes.AddRange(mBytes);
                }
                else if (HasZ)
                {
                    byte[] zBytes = StCoordinatesSystemHelper.GetDifferenceFromBytes(bytesList[2].ToArray(), previousBytesList[2].ToArray(), StParameters.CoordinatesSystem, StCoordType.Z);
                    allZBytes.AddRange(zBytes);
                }
                else if (HasM)
                {
                    byte[] mBytes = StCoordinatesSystemHelper.GetDifferenceFromBytes(bytesList[2].ToArray(), previousBytesList[3].ToArray(), StParameters.CoordinatesSystem, StCoordType.M);
                    allMBytes.AddRange(mBytes);
                }

                previousBytesList = bytesList;
            }

            // Add Z et M Bytes to XY ones
            if (HasZ)
                allBytes.AddRange(allZBytes);
            if (HasM)
                allBytes.AddRange(allMBytes);

            // Add reserved bytes
            List<byte> header = StGeometryHelper.GenerateHeader(this, allBytes);
            allBytes.InsertRange(0, header);
            Bytes = allBytes.ToArray();
        }

        public override string ToString()
        {
            string prefix = "LINESTRING ";
            if (HasZ)
                prefix += "Z";
            if (HasM)
                prefix += "M";

            var str = new StringBuilder();
            foreach (StPoint point in Points)
            {
                str.Append(StGeometryHelper.GetCoordinateWkt(point)).Append(", ");
            }

            return prefix.TrimEnd(' ') + " (" + str.ToString().TrimEnd(' ', ',') + ")";
        }
    }
}