using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using Gry.ArcGis.NetStGeometry;
using Gry.ArcGis.NetStGeometry.Geometry.Base;
using Gry.ArcGis.NetStGeometry.Geometry.Primitives;
using Gry.ArcGis.NetStGeometry.Oracle;
using Gry.ArcGis.NetStGeometry.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.DataAccess.Client;

namespace Gry.ArcGis.NetStGeometryTest
{
    public static class StTestHelper
    {
        private static readonly Random Random = new Random(Process.GetCurrentProcess().Id + DateTime.Now.Millisecond);

        public static string ConnectionString
        {
            get { return ConfigurationManager.AppSettings["ConnectionString"]; }
        }

        public static IEsriStGeometryType GetGeometry(string geo, OracleConnection con)
        {
            var command = new OracleCommand("select sde.st_geomfromtext(:geo, :srid) as shape from dual", con);

            command.Parameters.Add(new OracleParameter("geo", OracleDbType.Clob, geo, ParameterDirection.Input));
            command.Parameters.Add(new OracleParameter("srid", OracleDbType.Int32, StParameters.CoordinatesSystem.Srid, ParameterDirection.Input));

            using (OracleDataReader reader = command.ExecuteReader())
            {
                if (reader != null && reader.Read())
                {
                    // Read bytes
                    return reader["shape"] as IEsriStGeometryType;
                }
            }
            throw new ArgumentException();
        }

        public static void AssertAreEqual(StPoint geo1, StPoint geo2)
        {
            // Compare geometry objects
            Assert.AreEqual(geo1.HasZ, geo2.HasZ);
            Assert.AreEqual(geo1.HasM, geo2.HasM);
            Assert.AreEqual(geo1.X.Value, geo2.X.Value);
            Assert.AreEqual(geo1.Y.Value, geo2.Y.Value);
            if (geo1.HasZ)
                Assert.AreEqual(geo1.Z.Value, geo2.Z.Value);
            if (geo1.HasM)
                Assert.AreEqual(geo1.M.Value, geo2.M.Value);
            // Compare bytes
            AssertAreEqual(geo1.Bytes, geo2.Bytes);
        }

        public static void AssertAreEqual(StLineString geo1, StLineString geo2)
        {
            // Compare geometry objects
            Assert.AreEqual(geo1.HasZ, geo2.HasZ);
            Assert.AreEqual(geo1.HasM, geo2.HasM);

            Assert.AreEqual(geo1.Points.Count, geo2.Points.Count);
            for (int i = 0; i < geo1.Points.Count; ++i)
            {
                Assert.AreEqual(geo1.Points[i].X.Value, geo2.Points[i].X.Value);
                Assert.AreEqual(geo1.Points[i].Y.Value, geo2.Points[i].Y.Value);
                if (geo1.HasZ)
                    Assert.AreEqual(geo1.Points[i].Z.Value, geo2.Points[i].Z.Value);
                if (geo1.HasM)
                    Assert.AreEqual(geo1.Points[i].M.Value, geo2.Points[i].M.Value);
            }

            // Compare bytes
            AssertAreEqual(geo1.Bytes, geo2.Bytes);
        }

        public static void AssertAreEqual(byte[] b1, byte[] b2)
        {
            Assert.AreEqual(b1.Length, b2.Length);
            for (int i = 0; i < b1.Length; ++i)
            {
                Assert.AreEqual(b1[i], b2[i]);
            }
        }

        public static void AssertAreEqualWithoutHeader(StLineString geo1, StLineString geo2)
        {
            // Compare geometry objects
            Assert.AreEqual(geo1.HasZ, geo2.HasZ);
            Assert.AreEqual(geo1.HasM, geo2.HasM);

            Assert.AreEqual(geo1.Points.Count, geo2.Points.Count);
            for (int i = 0; i < geo1.Points.Count; ++i)
            {
                Assert.AreEqual(geo1.Points[i].X.Value, geo2.Points[i].X.Value);
                Assert.AreEqual(geo1.Points[i].Y.Value, geo2.Points[i].Y.Value);
                if (geo1.HasZ)
                    Assert.AreEqual(geo1.Points[i].Z.Value, geo2.Points[i].Z.Value);
                if (geo1.HasM)
                    Assert.AreEqual(geo1.Points[i].M.Value, geo2.Points[i].M.Value);
            }

            // Compare bytes
            AssertAreEqualWithoutHeader(geo1.Bytes, geo2.Bytes);
        }

        private static void AssertAreEqualWithoutHeader(byte[] b1, byte[] b2)
        {
            Assert.AreEqual(b1.Length, b2.Length);
            for (int i = 8; i < b1.Length; ++i)
            {
                Assert.AreEqual(b1[i], b2[i]);
            }
        }

        public static void Write(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                string log = b + " ";
                Debug.Write(log);
            }
            Debug.WriteLine(String.Empty);
        }

        public static void Write(IStGeometry geo)
        {
            var log = geo + " : ";
            Debug.Write(log);

            Write(geo.Bytes);
        }

        public static void Write(string geo)
        {
            string log = geo + " : ";
            Debug.Write(log);
        }

        public static void WriteLine(string text)
        {
            Debug.WriteLine(text);
        }

        public static string ToChar38(this Guid guid)
        {
            return "{" + guid.ToString().ToUpperInvariant() + "}";
        }

        public static void InitializeCoordinatesSystem(OracleConnection con)
        {
            StParameters.CoordinatesSystem = new StCoordinatesSystem(
                3857,
                "WGS_1984_Web_Mercator_Auxiliary_Sphere",
                0.0001m,    // Precision
                0.001m,     // Tolerance
                -20037700m, 900699887774.099m,  // X
                -30241100m, 900689684374.099m,  // Y
                -100000m, 900719825474.099m,    // Z
                null,
                null);
        }

        public static decimal GetRandomX(int? min = null, int? max = null)
        {
            if (min == null)
                min = (int.MinValue < StParameters.CoordinatesSystem.MinX) ? (int) Math.Truncate(StParameters.CoordinatesSystem.MinX) : int.MinValue;
            if (max == null)
                max = (int.MaxValue > StParameters.CoordinatesSystem.MaxX) ? (int) Math.Truncate(StParameters.CoordinatesSystem.MaxX) : int.MaxValue;

            decimal x = Random.Next((int.MinValue < min) ? min.Value : int.MinValue, (int.MaxValue > max) ? max.Value : int.MaxValue);
            x += Random.Next(0, 10000)*StParameters.CoordinatesSystem.Resolution;

            return x;
        }

        public static decimal GetRandomY(int? min = null, int? max = null)
        {
            if (min == null)
                min = (int.MinValue < StParameters.CoordinatesSystem.MinY) ? (int) Math.Truncate(StParameters.CoordinatesSystem.MinY) : int.MinValue;
            if (max == null)
                max = (int.MaxValue > StParameters.CoordinatesSystem.MaxY) ? (int) Math.Truncate(StParameters.CoordinatesSystem.MaxY) : int.MaxValue;
            
            decimal y = Random.Next((int.MinValue < min) ? min.Value : int.MinValue, (int.MaxValue > max) ? max.Value : int.MaxValue);
            y += Random.Next(0, 10000)*StParameters.CoordinatesSystem.Resolution;

            return y;
        }

        public static decimal GetRandomZ(int? min = null, int? max = null)
        {
            if (min == null)
                min = (int.MinValue < StParameters.CoordinatesSystem.MinZ) ? (int) Math.Truncate(StParameters.CoordinatesSystem.MinZ) : int.MinValue;
            if (max == null)
                max = (int.MaxValue > StParameters.CoordinatesSystem.MaxZ) ? (int) Math.Truncate(StParameters.CoordinatesSystem.MaxZ) : int.MaxValue;
            
            decimal z = Random.Next((int.MinValue < min) ? min.Value : int.MinValue, (int.MaxValue > max) ? max.Value : int.MaxValue);
            z += Random.Next(0, 10000)*StParameters.CoordinatesSystem.Resolution;

            return z;
        }
    }
}