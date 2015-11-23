using System.Configuration;
using System.Globalization;
using Gry.ArcGis.NetStGeometry;
using Gry.ArcGis.NetStGeometry.Geometry.Primitives;
using Gry.ArcGis.NetStGeometry.Oracle;
using Gry.ArcGis.NetStGeometry.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.DataAccess.Client;

namespace NetStGeometryTest
{
    [TestClass]
    public class StCoordinatesSystemTest
    {
        private static OracleConnection con;

        private static StCoordinatesSystem CoordinatesSystem
        {
            get { return StParameters.CoordinatesSystem; }
        }

        #region Initialize

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            con = new OracleConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            con.Open();
            StTestHelper.InitializeCoordinatesSystem(con);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            con.Close();
            con.Dispose();
        }

        #endregion Initialize

        #region Limits Overflow

        [TestMethod]
        [ExpectedException(typeof (OracleException))]
        public void TestCoordSystemLimitMaxXOverflow()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                string wkt = "POINT Z (" + (CoordinatesSystem.MaxX + 1).ToString(CultureInfo.InvariantCulture) + " 0 0)";
                StTestHelper.Write(wkt);
                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
            }
        }

        [TestMethod]
        [ExpectedException(typeof (OracleException))]
        public void TestCoordSystemLimitMinXOverflow()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                string wkt = "POINT Z (" + (CoordinatesSystem.MinX - 1).ToString(CultureInfo.InvariantCulture) + " 0 0)";
                StTestHelper.Write(wkt);
                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
            }
        }

        [TestMethod]
        [ExpectedException(typeof (OracleException))]
        public void TestCoordSystemLimitMaxYOverflow()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                string wkt = "POINT Z (0 " + (CoordinatesSystem.MaxY + 1).ToString(CultureInfo.InvariantCulture) + " 0)";
                StTestHelper.Write(wkt);
                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
            }
        }

        [TestMethod]
        [ExpectedException(typeof (OracleException))]
        public void TestCoordSystemLimitMinYOverflow()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                string wkt = "POINT Z (0 " + (CoordinatesSystem.MinY - 1).ToString(CultureInfo.InvariantCulture) + " 0)";
                StTestHelper.Write(wkt);
                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
            }
        }

        [TestMethod]
        [ExpectedException(typeof (OracleException))]
        public void TestCoordSystemLimitMaxZOverflow()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                string wkt = "POINT Z (0 0 " + (CoordinatesSystem.MaxZ + 1).ToString(CultureInfo.InvariantCulture) + ")";
                StTestHelper.Write(wkt);
                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
            }
        }

        [TestMethod]
        [ExpectedException(typeof (OracleException))]
        public void TestCoordSystemLimitMinZOverflow()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                string wkt = "POINT Z (0 0 " + (CoordinatesSystem.MinZ - 1).ToString(CultureInfo.InvariantCulture) + ")";
                StTestHelper.Write(wkt);
                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
            }
        }

        #endregion Limits Overflow

        #region Limits

        [TestMethod]
        public void TestCoordSystemLimitMaxX()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                decimal val = CoordinatesSystem.MaxX;

                string wkt = "POINT Z (" + val.ToString("0.####", CultureInfo.InvariantCulture) + " 0 0)";
                StTestHelper.Write(wkt);

                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
                StTestHelper.Write(geo.Points);
                StTestHelper.Write(geo.Geometry);

                // Create point with the same coordinates
                var p1 = new StPoint(val, 0, 0);
                StTestHelper.Write(p1);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                // Create point from shape
                var p2 = new StPoint(geo.Points);
                StTestHelper.Write(p2);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TestCoordSystemLimitMinX()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                decimal val = CoordinatesSystem.MinX;

                string wkt = "POINT Z (" + val.ToString("0.####", CultureInfo.InvariantCulture) + " 0 0)";
                StTestHelper.Write(wkt);

                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
                StTestHelper.Write(geo.Points);
                StTestHelper.Write(geo.Geometry);

                // Create point with the same coordinates
                var p1 = new StPoint(val, 0, 0);
                StTestHelper.Write(p1);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                // Create point from shape
                var p2 = new StPoint(geo.Points);
                StTestHelper.Write(p2);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TestCoordSystemLimitMaxY()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                decimal val = CoordinatesSystem.MaxY;

                string wkt = "POINT Z (0 " + val.ToString("0.####", CultureInfo.InvariantCulture) + " 0)";
                StTestHelper.Write(wkt);

                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
                StTestHelper.Write(geo.Points);
                StTestHelper.Write(geo.Geometry);

                // Create point with the same coordinates
                var p1 = new StPoint(0, val, 0);
                StTestHelper.Write(p1);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                // Create point from shape
                var p2 = new StPoint(geo.Points);
                StTestHelper.Write(p2);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TestCoordSystemLimitMinY()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                decimal val = CoordinatesSystem.MinY;

                string wkt = "POINT Z (0 " + val.ToString("0.####", CultureInfo.InvariantCulture) + " 0)";
                StTestHelper.Write(wkt);

                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
                StTestHelper.Write(geo.Points);
                StTestHelper.Write(geo.Geometry);

                // Create point with the same coordinates
                var p1 = new StPoint(0, val, 0);
                StTestHelper.Write(p1);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                // Create point from shape
                var p2 = new StPoint(geo.Points);
                StTestHelper.Write(p2);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TestCoordSystemLimitMaxZ()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                decimal val = CoordinatesSystem.MaxZ;

                string wkt = "POINT Z (0 0 " + val.ToString("0.####", CultureInfo.InvariantCulture) + ")";
                StTestHelper.Write(wkt);

                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
                StTestHelper.Write(geo.Points);
                StTestHelper.Write(geo.Geometry);

                // Create point with the same coordinates
                var p1 = new StPoint(0, 0, val);
                StTestHelper.Write(p1);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                // Create point from shape
                var p2 = new StPoint(geo.Points);
                StTestHelper.Write(p2);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TestCoordSystemLimitMinZ()
        {
            using (OracleTransaction tr = con.BeginTransaction())
            {
                decimal val = CoordinatesSystem.MinZ;

                string wkt = "POINT Z (0 0 " + val.ToString("0.####", CultureInfo.InvariantCulture) + ")";
                StTestHelper.Write(wkt);

                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, con);
                StTestHelper.Write(geo.Points);
                StTestHelper.Write(geo.Geometry);

                // Create point with the same coordinates
                var p1 = new StPoint(0, 0, val);
                StTestHelper.Write(p1);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                // Create point from shape
                var p2 = new StPoint(geo.Points);
                StTestHelper.Write(p2);

                // Assert values are equal
                StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);

                tr.Rollback();
            }
        }

        #endregion Limits

        #region Differences

        /*
        [TestMethod]
        public void TestValueFromBytesPositiveDifferenceX()
        {
            var val = StCoordinatesSystemHelper.GetValueFromBytes(PositiveUnityValue, CoordinatesSystem, STCoordType.X);
            var dif = (val > 0) ? val + CoordinatesSystem.GetMin(STCoordType.X) : val - CoordinatesSystem.GetMin(STCoordType.X);
            Assert.AreEqual(dif, 1);
        }

        [TestMethod]
        public void TestValueFromBytesPositiveDifferenceY()
        {
            var val = StCoordinatesSystemHelper.GetValueFromBytes(PositiveUnityValue, CoordinatesSystem, STCoordType.Y);
            var dif = (val > 0) ? val + CoordinatesSystem.GetMin(STCoordType.Y) : val - CoordinatesSystem.GetMin(STCoordType.Y);
            Assert.AreEqual(dif, 1);
        }

        [TestMethod]
        public void TestValueFromBytesPositiveDifferenceZ()
        {
            var val = StCoordinatesSystemHelper.GetValueFromBytes(PositiveUnityValue, CoordinatesSystem, STCoordType.Z);
            var dif = (val > 0) ? val + CoordinatesSystem.GetMin(STCoordType.Z) : val - CoordinatesSystem.GetMin(STCoordType.Z);
            Assert.AreEqual(dif, 1);
        }

        [TestMethod]
        public void TestValueFromBytesNegativeDifferenceX()
        {
            var val = StCoordinatesSystemHelper.GetValueFromBytes(NegativeUnityValue, CoordinatesSystem, STCoordType.X);
            var dif = (val > 0) ? val + CoordinatesSystem.GetMin(STCoordType.X) : val - CoordinatesSystem.GetMin(STCoordType.X);
            Assert.AreEqual(dif, -1);
        }

        [TestMethod]
        public void TestValueFromBytesNegativeDifferenceY()
        {
            var val = StCoordinatesSystemHelper.GetValueFromBytes(NegativeUnityValue, CoordinatesSystem, STCoordType.Y);
            var dif = (val > 0) ? val + CoordinatesSystem.GetMin(STCoordType.Y) : val - CoordinatesSystem.GetMin(STCoordType.Y);
            Assert.AreEqual(dif, -1);
        }

        [TestMethod]
        public void TestValueFromBytesNegativeDifferenceZ()
        {
            var val = StCoordinatesSystemHelper.GetValueFromBytes(NegativeUnityValue, CoordinatesSystem, STCoordType.Z);
            var dif = (val > 0) ? val + CoordinatesSystem.GetMin(STCoordType.Z) : val - CoordinatesSystem.GetMin(STCoordType.Z);
            Assert.AreEqual(dif, -1);
        }

        [TestMethod]
        public void TestBytesFromValuePositiveDifferenceX()
        {
            const int val = 1;
            var valToEncode = CoordinatesSystem.GetMin(STCoordType.X) + val;
            var bytes = StCoordinatesSystemHelper.GetBytesFromValue(valToEncode, CoordinatesSystem, STCoordType.X);

            StTestHelper.AssertAreEqual(bytes, PositiveUnityValue);
        }

        [TestMethod]
        public void TestBytesFromValuePositiveDifferenceY()
        {
            const int val = 1;
            var valToEncode = CoordinatesSystem.GetMin(STCoordType.Y) + val;
            var bytes = StCoordinatesSystemHelper.GetBytesFromValue(valToEncode, CoordinatesSystem, STCoordType.Y);

            StTestHelper.AssertAreEqual(bytes, PositiveUnityValue);
        }

        [TestMethod]
        public void TestBytesFromValuePositiveDifferenceZ()
        {
            const int val = 1;
            var valToEncode = CoordinatesSystem.GetMin(STCoordType.Z) + val;
            var bytes = StCoordinatesSystemHelper.GetBytesFromValue(valToEncode, CoordinatesSystem, STCoordType.Z);

            StTestHelper.AssertAreEqual(bytes, PositiveUnityValue);
        }

        [TestMethod]
        public void TestBytesFromValueNegativeDifferenceX()
        {
            const int val = -1;
            var valToEncode = CoordinatesSystem.GetMin(STCoordType.X) + val;
            var bytes = StCoordinatesSystemHelper.GetBytesFromValue(valToEncode, CoordinatesSystem, STCoordType.X);

            StTestHelper.AssertAreEqual(bytes, NegativeUnityValue);
        }

        [TestMethod]
        public void TestBytesFromValueNegativeDifferenceY()
        {
            const int val = -1;
            var valToEncode = CoordinatesSystem.GetMin(STCoordType.Y) + val;
            var bytes = StCoordinatesSystemHelper.GetBytesFromValue(valToEncode, CoordinatesSystem, STCoordType.Y);

            StTestHelper.AssertAreEqual(bytes, NegativeUnityValue);
        }

        [TestMethod]
        public void TestBytesFromValueNegativeDifferenceZ()
        {
            const int val = -1;
            var valToEncode = CoordinatesSystem.GetMin(STCoordType.Z) + val;
            var bytes = StCoordinatesSystemHelper.GetBytesFromValue(valToEncode, CoordinatesSystem, STCoordType.Z);

            StTestHelper.AssertAreEqual(bytes, NegativeUnityValue);
        }
        */

        #endregion Differences
    }
}