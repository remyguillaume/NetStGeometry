using System;
using System.Collections.Generic;
using System.Globalization;
using Gry.ArcGis.NetStGeometry.Tools;

namespace Gry.ArcGis.NetStGeometry.Geometry
{
    /// <summary>
    /// Represents a Point of the Esri ST_Geometry format (in Oracle)
    /// </summary>
    public class StPoint : IStGeometry
    {
        public StPoint(byte[] bytes)
        {
            Bytes = bytes;

            // Is it a X/M Geometry ?
            bool hasZ, hasM;
            List<byte> header = StGeometryHelper.ExtractHeader(bytes);
            StGeometryHelper.GetGeometryDimension(header, out hasZ, out hasM);
            HasZ = hasZ;
            HasM = HasM;

            // Split bytes in encode values
            List<List<byte>> bytesList = StGeometryHelper.GetEncodedValues(StGeometryHelper.RemoveHeader(bytes));

            // Checks
            if (bytesList.Count < 2)
                throw new ArgumentException("Cannot create geometry from those bytes.");
            if ((HasZ || HasM) && bytesList.Count < 3)
                throw new ArgumentException("Cannot create geometry from those bytes.");
            if (HasZ && HasM && bytesList.Count < 4)
                throw new ArgumentException("Cannot create geometry from those bytes.");
            if (bytesList.Count > 4)
                throw new ArgumentException("Cannot create geometry from those bytes.");

            // Set geometry values
            SetGeometry(bytesList);
        }

        public StPoint(byte[] x, byte[] y, byte[] z = null, byte[] m = null)
        {
            X = new StCoordinate(x, StCoordType.X);
            Y = new StCoordinate(y, StCoordType.Y);
            if (z != null)
            {
                HasZ = true;
                Z = new StCoordinate(z, StCoordType.Z);
            }
            if (m != null)
            {
                HasM = true;
                M = new StCoordinate(m, StCoordType.M);
            }

            // Set Bytes
            CalculateBytes();
        }

        public StPoint(double x, double y, double? z = null, double? m = null)
            : this((decimal)x, (decimal)y, (decimal?)z, (decimal?)m)
        {
        }

        public StPoint(decimal x, decimal y, decimal? z = null, decimal? m = null)
        {
            X = new StCoordinate(x, StCoordType.X);
            Y = new StCoordinate(y, StCoordType.Y);
            if (z.HasValue)
            {
                HasZ = true;
                Z = new StCoordinate(z.Value, StCoordType.Z);
            }
            if (m.HasValue)
            {
                HasM = true;
                M = new StCoordinate(m.Value, StCoordType.M);
            }

            // Set Bytes
            CalculateBytes();
        }

        public StPoint(string wkt)
        {
            if (wkt.ToUpper().Contains("Z"))
                HasZ = true;
            if (wkt.ToUpper().Contains("M"))
                HasM = true;

            string coordsText = wkt.ToUpper().Replace("POINT", String.Empty).Replace("Z", String.Empty).Replace("M", String.Empty).Replace("(", String.Empty).Replace(")", String.Empty).Trim();
            string[] coords = coordsText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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

            X = new StCoordinate(x, StCoordType.X);
            Y = new StCoordinate(y, StCoordType.Y);

            if (HasZ)
            {
                if (!z.HasValue)
                    throw new ArgumentException();
                Z = new StCoordinate(z.Value, StCoordType.Z);
            }

            if (HasM)
            {
                if (!m.HasValue)
                    throw new ArgumentException();
                M = new StCoordinate(m.Value, StCoordType.M);
            }

            CalculateBytes();
        }

        public StCoordinate X { get; private set; }
        public StCoordinate Y { get; private set; }
        public StCoordinate Z { get; private set; }
        public StCoordinate M { get; private set; }

        #region IStGeometry Members

        public bool HasZ { get; private set; }
        public bool HasM { get; private set; }
        public byte[] Bytes { get; private set; }
        public string Wkt
        {
            get { return ToString(); }
        }

        #endregion

        private void SetGeometry(List<List<byte>> bytesList)
        {
            X = new StCoordinate(bytesList[0], StCoordType.X);
            Y = new StCoordinate(bytesList[1], StCoordType.Y);
            if (HasZ && HasM)
            {
                Z = new StCoordinate(bytesList[2], StCoordType.Z);
                M = new StCoordinate(bytesList[3], StCoordType.M);
            }
            else if (HasZ)
                Z = new StCoordinate(bytesList[2], StCoordType.Z);
            else if (HasM)
                M = new StCoordinate(bytesList[2], StCoordType.M);
        }

        private void CalculateBytes()
        {
            var bytes = new List<byte>();

            // Coordinates
            bytes.AddRange(X.Bytes);
            bytes.AddRange(Y.Bytes);

            if (HasZ)
                bytes.AddRange(Z.Bytes);
            if (HasM)
                bytes.AddRange(M.Bytes);

            // Add reserved bytes
            List<byte> header = StGeometryHelper.GenerateHeader(this, bytes);
            bytes.InsertRange(0, header);
            Bytes = bytes.ToArray();
        }

        public override string ToString()
        {
            string prefix = "POINT ";
            if (HasZ)
                prefix += "Z";
            if (HasM)
                prefix += "M";

            string coordinatesWkt = StGeometryHelper.GetCoordinateWkt(this);

            return prefix.TrimEnd(' ') + " (" + coordinatesWkt + ")";
        }
    }
}