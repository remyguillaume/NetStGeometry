using System.Collections.Generic;
using System.Globalization;
using Gry.ArcGis.NetStGeometry.Tools;

namespace Gry.ArcGis.NetStGeometry.Geometry
{
    /// <summary>
    /// Represents a Coordinate (X/Y/Z/M). Will be used for Esri ST Geometries.
    /// </summary>
    public class StCoordinate
    {
        public StCoordinate(byte[] points, StCoordType type)
        {
            Bytes = points;
            Value = StCoordinatesSystemHelper.GetValueFromBytes(Bytes, StParameters.CoordinatesSystem, type);
        }

        public StCoordinate(List<byte> points, StCoordType type)
            : this(points.ToArray(), type)
        {
        }

        public StCoordinate(decimal val, StCoordType type)
        {
            Value = val;
            Bytes = StCoordinatesSystemHelper.GetBytesFromValue(Value, StParameters.CoordinatesSystem, type);
        }

        public decimal Value { get; set; }
        public byte[] Bytes { get; set; }

        public override string ToString()
        {
            return Value.ToString("0.####", CultureInfo.InvariantCulture);
        }
    }
}