using System;
using Gry.ArcGis.NetStGeometry.Geometry;
using Gry.ArcGis.NetStGeometry.Geometry.MultiPart;
using Gry.ArcGis.NetStGeometry.Geometry.Primitives;

namespace Gry.ArcGis.NetStGeometry.Oracle
{
    internal static class EsrIStGeometryTypeHelper
    {
        internal static void MapFromCustomObject(IEsriStGeometryType geoType)
        {
            geoType.SetValue("ENTITY", geoType.Entity);
            geoType.SetValue("NUMPTS", geoType.NumPts);
            geoType.SetValue("MINX", geoType.MinX);
            geoType.SetValue("MINY", geoType.MinY);
            geoType.SetValue("MAXX", geoType.MaxX);
            geoType.SetValue("MAXY", geoType.MaxY);
            geoType.SetValue("MINZ", geoType.MinZ);
            geoType.SetValue("MAXZ", geoType.MaxZ);
            geoType.SetValue("MINM", geoType.MinM);
            geoType.SetValue("MAXM", geoType.MaxM);
            geoType.SetValue("AREA", geoType.Area);
            geoType.SetValue("LEN", geoType.Len);
            geoType.SetValue("SRID", geoType.Srid);
            geoType.SetValue("POINTS", geoType.Points);
        }

        internal static void MapToCustomObject(IEsriStGeometryType geoType)
        {
            geoType.Entity = geoType.GetValue<int>("ENTITY");
            geoType.NumPts = geoType.GetValue<int>("NUMPTS");
            geoType.MinX = geoType.GetValue<decimal>("MINX");
            geoType.MinY = geoType.GetValue<decimal>("MINY");
            geoType.MaxX = geoType.GetValue<decimal>("MAXX");
            geoType.MaxY = geoType.GetValue<decimal>("MAXY");
            geoType.MinZ = geoType.GetValue<decimal?>("MINZ");
            geoType.MaxZ = geoType.GetValue<decimal?>("MAXZ");
            geoType.MinM = geoType.GetValue<decimal?>("MINM");
            geoType.MaxM = geoType.GetValue<decimal?>("MAXM");
            geoType.Area = geoType.GetValue<decimal>("AREA");
            geoType.Len = geoType.GetValue<decimal>("LEN");
            geoType.Srid = geoType.GetValue<int>("SRID");
            geoType.Points = geoType.GetValue<byte[]>("POINTS");

            switch ((StGeometryType) geoType.Entity)
            {
                case StGeometryType.Point:
                    geoType.Geometry = new StPoint(geoType.Points);
                    break;
                case StGeometryType.LineString:
                case StGeometryType.LineStringZ:
                    geoType.Geometry = new StLineString(geoType.Points);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        internal static void CalculateOtherValuesFromGeometry(IEsriStGeometryType geoType)
        {
            if (geoType.Geometry is StPoint)
            {
                geoType.Entity = (int) StGeometryType.Point;

                var point = (StPoint) geoType.Geometry;
                geoType.NumPts = 1;
                geoType.MinX = geoType.MaxX = point.X.Value;
                geoType.MinY = geoType.MaxY = point.Y.Value;
                if (point.HasZ)
                    geoType.MinZ = geoType.MaxZ = point.Z.Value;
                if (point.HasM)
                    geoType.MinM = geoType.MaxM = point.M.Value;

                geoType.Area = 0;
                geoType.Len = 0;
                geoType.Srid = StParameters.CoordinatesSystem.Srid;

                geoType.Points = geoType.Geometry.Bytes;
            }
            else if (geoType.Geometry is StLineString)
            {
                var line = (StLineString) geoType.Geometry;
                if (line.HasZ)
                    geoType.Entity = (int) StGeometryType.LineStringZ;
                else
                    geoType.Entity = (int) StGeometryType.LineString;

                geoType.NumPts = line.Points.Count;

                geoType.MinX = line.MinX;
                geoType.MaxX = line.MaxX;
                geoType.MinY = line.MinY;
                geoType.MaxY = line.MaxY;
                if (line.HasZ)
                {
                    geoType.MinZ = line.MinZ;
                    geoType.MaxZ = line.MaxZ;
                }
                if (line.HasM)
                {
                    geoType.MinM = line.MinM;
                    geoType.MaxM = line.MaxM;
                }

                geoType.Area = 0;
                geoType.Len = line.Length2D;
                geoType.Srid = StParameters.CoordinatesSystem.Srid;

                geoType.Points = geoType.Geometry.Bytes;
            }
            else
                throw new NotSupportedException();
        }
    }
}