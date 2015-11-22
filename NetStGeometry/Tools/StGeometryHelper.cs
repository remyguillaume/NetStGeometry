using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gry.ArcGis.NetStGeometry.Geometry;
using Gry.ArcGis.NetStGeometry.Geometry.Base;
using Gry.ArcGis.NetStGeometry.Geometry.Primitives;

namespace Gry.ArcGis.NetStGeometry.Tools
{
    /// <summary>
    /// Helper methods for STGeometry objects
    /// </summary>
    public static class StGeometryHelper
    {
        private const int NbReservedBytes = 8;

        public static List<List<byte>> GetEncodedValues(byte[] bytes)
        {
            var points = new List<List<byte>>();

            var current = new List<byte>();
            points.Add(current);

            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];
                current.Add(b);
                if (b >= 128)
                {
                    // The next byte is use for the same coordinate
                }
                else
                {
                    current = new List<byte>();
                    points.Add(current);
                }
            }

            if (current.Count <= 0)
                points.Remove(current);

            return points;
        }

        public static List<byte> GenerateHeader(IStGeometry geometry, List<byte> encodedBytes)
        {
            // 8 bytes header
            var header = new List<byte>();

            // 4 first bytes => number of used bytes to encode coordinates
            var encodedNbBytes = new List<byte>(StCoordinatesSystemHelper.GetBytesFromValue(encodedBytes.Count, StParameters.CoordinatesSystem));
            for (int i = encodedNbBytes.Count; i < 4; ++i)
            {
                encodedNbBytes.Add(0);
            }
            header.AddRange(encodedNbBytes);

            // 1 unknown byte (perhaps big/little endianness ?)
            header.Add(1);

            // 1 byte to define geometry dimension (Z / M)
            int zm = 0;
            if (geometry.HasZ)
                zm += 1;
            if (geometry.HasM)
                zm += 2;
            header.Add((byte)zm);

            // 2 unknown bytes
            header.AddRange(new byte[] { 0, 0 });

            return header;
        }

        public static List<byte> ExtractHeader(byte[] bytes)
        {
            var header = new List<byte>();
            for (int i = 0; i < NbReservedBytes; i++)
            {
                header.Add(bytes[i]);
            }

            return header;
        }

        public static byte[] RemoveHeader(byte[] bytes)
        {
            var data = new List<byte>();
            for (int i = NbReservedBytes; i < bytes.Length; i++)
            {
                data.Add(bytes[i]);
            }

            return data.ToArray();
        }

        public static void GetGeometryDimension(IEnumerable<byte> header, out bool hasZ, out bool hasM)
        {
            if (header.Count() != NbReservedBytes)
                throw new ArgumentException("Only header here !");

            hasZ = hasM = false;
            switch (header.ElementAt(5))
            {
                case 0:
                    // No Z nor M
                    break;
                case 1:
                    hasZ = true;
                    break;
                case 2:
                    hasM = true;
                    break;
                case 3:
                    hasZ = hasM = true;
                    break;
                default:
                    throw new NotSupportedException("Unsupported Geometry dimension");
            }
        }

        public static string GetCoordinateWkt(StPoint point)
        {
            var str = new StringBuilder();
            str.Append(point.X).Append(" ").Append(point.Y);
            if (point.HasZ)
                str.Append(" ").Append(point.Z);
            if (point.HasM)
                str.Append(" ").Append(point.M);

            return str.ToString();
        }
    }
}