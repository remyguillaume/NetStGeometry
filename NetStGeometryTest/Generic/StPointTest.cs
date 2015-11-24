using System.Globalization;
using Gry.ArcGis.NetStGeometry;
using Gry.ArcGis.NetStGeometry.Geometry.Primitives;
using Gry.ArcGis.NetStGeometry.Oracle;
using Gry.ArcGis.NetStGeometry.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.DataAccess.Client;

namespace Gry.ArcGis.NetStGeometryTest.Generic
{
    [TestClass]
    public class StPointTest
    {
        private static OracleConnection _con;

        private static StCoordinatesSystem CoordinatesSystem
        {
            get { return StParameters.CoordinatesSystem; }
        }

        #region Initialize

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _con = new OracleConnection(StTestHelper.ConnectionString);
            _con.Open();
            StTestHelper.InitializeCoordinatesSystem(_con);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _con.Close();
            _con.Dispose();
        }

        #endregion Initialize

        #region Z & M

        [TestMethod]
        public void TeStPointNotZNotM()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                string wkt = "POINT (0 0)";
                StTestHelper.Write(wkt);

                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
                StTestHelper.Write(geo.Points);
                StTestHelper.Write(geo.Geometry);

                Assert.IsFalse(geo.Geometry.HasZ);
                Assert.IsFalse(geo.Geometry.HasM);

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointZNotM()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                string wkt = "POINT Z (0 0 1)";
                StTestHelper.Write(wkt);

                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
                StTestHelper.Write(geo.Points);
                StTestHelper.Write(geo.Geometry);

                Assert.IsTrue(geo.Geometry.HasZ);
                Assert.IsFalse(geo.Geometry.HasM);

                tr.Rollback();
            }
        }

        #endregion Z & M

        #region Zero values

        [TestMethod]
        public void TeStPointZero()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                string wkt = "POINT (0 0)";
                StTestHelper.Write(wkt);

                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
                StTestHelper.Write(geo.Points);
                StTestHelper.Write(geo.Geometry);

                // Create point with the same coordinates
                var p1 = new StPoint(0m, 0m);
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
        public void TeStPointZZero()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                string wkt = "POINT Z (0 0 0)";
                StTestHelper.Write(wkt);

                IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
                StTestHelper.Write(geo.Points);
                StTestHelper.Write(geo.Geometry);

                // Create point with the same coordinates
                var p1 = new StPoint(0m, 0, 0);
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

        #endregion Zero values

        #region First Values

        [TestMethod]
        public void TeStPointFirstPositiveXValues()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (decimal i = 0; i < CoordinatesSystem.Resolution*100; i += CoordinatesSystem.Resolution)
                {
                    string wkt = "POINT Z (" + i.ToString(CultureInfo.InvariantCulture) + " 0 0)";
                    StTestHelper.Write(wkt);

                    IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
                    StTestHelper.Write(geo.Points);
                    StTestHelper.Write(geo.Geometry);

                    // Create point with the same coordinates
                    var p1 = new StPoint(i, 0, 0);
                    StTestHelper.Write(p1);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                    // Create point from shape
                    var p2 = new StPoint(geo.Points);
                    StTestHelper.Write(p2);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);
                }

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointFirstNegativeXValues()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (decimal i = 0; i >= -CoordinatesSystem.Resolution*100; i -= CoordinatesSystem.Resolution)
                {
                    string wkt = "POINT Z (" + i.ToString(CultureInfo.InvariantCulture) + " 0 0)";
                    StTestHelper.Write(wkt);

                    IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
                    StTestHelper.Write(geo.Points);
                    StTestHelper.Write(geo.Geometry);

                    // Create point with the same coordinates
                    var p1 = new StPoint(i, 0, 0);
                    StTestHelper.Write(p1);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                    // Create point from shape
                    var p2 = new StPoint(geo.Points);
                    StTestHelper.Write(p2);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);
                }

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointFirstPositiveYValues()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (decimal i = 0; i < CoordinatesSystem.Resolution*100; i += CoordinatesSystem.Resolution)
                {
                    string wkt = "POINT Z (0 " + i.ToString(CultureInfo.InvariantCulture) + " 0)";
                    StTestHelper.Write(wkt);

                    IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
                    StTestHelper.Write(geo.Points);
                    StTestHelper.Write(geo.Geometry);

                    // Create point with the same coordinates
                    var p1 = new StPoint(0, i, 0);
                    StTestHelper.Write(p1);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                    // Create point from shape
                    var p2 = new StPoint(geo.Points);
                    StTestHelper.Write(p2);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);
                }

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointFirstNegativeYValues()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (decimal i = 0; i >= -CoordinatesSystem.Resolution*100; i -= CoordinatesSystem.Resolution)
                {
                    string wkt = "POINT Z (0 " + i.ToString(CultureInfo.InvariantCulture) + " 0)";
                    StTestHelper.Write(wkt);

                    IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
                    StTestHelper.Write(geo.Points);
                    StTestHelper.Write(geo.Geometry);

                    // Create point with the same coordinates
                    var p1 = new StPoint(0, i, 0);
                    StTestHelper.Write(p1);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                    // Create point from shape
                    var p2 = new StPoint(geo.Points);
                    StTestHelper.Write(p2);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);
                }

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointFirstPositiveZValues()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (decimal i = 0; i < CoordinatesSystem.Resolution*100; i += CoordinatesSystem.Resolution)
                {
                    string wkt = "POINT Z (0 0 " + i.ToString(CultureInfo.InvariantCulture) + ")";
                    StTestHelper.Write(wkt);

                    IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
                    StTestHelper.Write(geo.Points);
                    StTestHelper.Write(geo.Geometry);

                    // Create point with the same coordinates
                    var p1 = new StPoint(0, 0, i);
                    StTestHelper.Write(p1);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                    // Create point from shape
                    var p2 = new StPoint(geo.Points);
                    StTestHelper.Write(p2);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);
                }

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointFirstNegativeZValues()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (decimal i = 0; i >= -CoordinatesSystem.Resolution*100; i -= CoordinatesSystem.Resolution)
                {
                    string wkt = "POINT Z (0 0 " + i.ToString(CultureInfo.InvariantCulture) + ")";
                    StTestHelper.Write(wkt);

                    IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
                    StTestHelper.Write(geo.Points);
                    StTestHelper.Write(geo.Geometry);

                    // Create point with the same coordinates
                    var p1 = new StPoint(0, 0, i);
                    StTestHelper.Write(p1);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p1);

                    // Create point from shape
                    var p2 = new StPoint(geo.Points);
                    StTestHelper.Write(p2);

                    // Assert values are equal
                    StTestHelper.AssertAreEqual((StPoint) geo.Geometry, p2);
                }

                tr.Rollback();
            }
        }

        #endregion First Values

        #region Limits

        [TestMethod]
        public void TeStPointZerosPositiveLimitX()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (int j = 1; j < CoordinatesSystem.ByteFactors.Length; ++j)
                {
                    decimal diff = CoordinatesSystem.ByteFactors[j];

                    if (diff > CoordinatesSystem.MaxX)
                    {
                        // Coordinates Limits are reached
                        // We do not test those values
                        continue;
                    }

                    for (int i = -5; i <= 5; ++i)
                    {
                        decimal val = diff + (i*CoordinatesSystem.Resolution);

                        string wkt = "POINT Z (" + val.ToString("0.####", CultureInfo.InvariantCulture) + " 0 0)";
                        StTestHelper.Write(wkt);

                        IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
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
                    }
                }

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointZerosPositiveLimitY()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (int j = 1; j < CoordinatesSystem.ByteFactors.Length; ++j)
                {
                    decimal diff = CoordinatesSystem.ByteFactors[j];

                    if (diff > CoordinatesSystem.MaxY)
                    {
                        // Coordinates Limits are reached
                        // We do not test those values
                        continue;
                    }

                    for (int i = -5; i <= 5; ++i)
                    {
                        decimal val = diff + (i*CoordinatesSystem.Resolution);

                        string wkt = "POINT Z (0 " + val.ToString("0.####", CultureInfo.InvariantCulture) + " 0)";
                        StTestHelper.Write(wkt);

                        IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
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
                    }
                }

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointZerosPositiveLimitZ()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (int j = 1; j < CoordinatesSystem.ByteFactors.Length; ++j)
                {
                    decimal diff = CoordinatesSystem.ByteFactors[j];

                    if (diff > CoordinatesSystem.MaxZ)
                    {
                        // Coordinates Limits are reached
                        // We do not test those values
                        continue;
                    }

                    for (int i = -5; i <= 5; ++i)
                    {
                        decimal val = diff + (i*CoordinatesSystem.Resolution);

                        string wkt = "POINT Z (0 0 " + val.ToString("0.####", CultureInfo.InvariantCulture) + ")";
                        StTestHelper.Write(wkt);

                        IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
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
                    }
                }

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointZerosNegativeLimitX()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (int j = 1; j < CoordinatesSystem.ByteFactors.Length; ++j)
                {
                    decimal diff = CoordinatesSystem.ByteFactors[j];

                    if (-diff < CoordinatesSystem.MinX)
                    {
                        // Coordinates Limits are reached
                        // We do not test those values
                        continue;
                    }

                    for (int i = -5; i <= 5; ++i)
                    {
                        decimal val = -diff + (i*CoordinatesSystem.Resolution);

                        string wkt = "POINT Z (" + val.ToString("0.####", CultureInfo.InvariantCulture) + " 0 0)";
                        StTestHelper.Write(wkt);

                        IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
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
                    }
                }

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointZerosNegativeLimitY()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (int j = 1; j < CoordinatesSystem.ByteFactors.Length; ++j)
                {
                    decimal diff = CoordinatesSystem.ByteFactors[j];

                    if (-diff < CoordinatesSystem.MinY)
                    {
                        // Coordinates Limits are reached
                        // We do not test those values
                        continue;
                    }

                    for (int i = -5; i <= 5; ++i)
                    {
                        decimal val = -diff + (i*CoordinatesSystem.Resolution);

                        string wkt = "POINT Z (0 " + val.ToString("0.####", CultureInfo.InvariantCulture) + " 0)";
                        StTestHelper.Write(wkt);

                        IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
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
                    }
                }

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointZerosNegativeLimitZ()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                for (int j = 1; j < CoordinatesSystem.ByteFactors.Length; ++j)
                {
                    decimal diff = CoordinatesSystem.ByteFactors[j];

                    if (-diff < CoordinatesSystem.MinZ)
                    {
                        // Coordinates Limits are reached
                        // We do not test those values
                        continue;
                    }

                    for (int i = -5; i <= 5; ++i)
                    {
                        decimal val = -diff + (i*CoordinatesSystem.Resolution);

                        string wkt = "POINT Z (0 0 " + val.ToString("0.####", CultureInfo.InvariantCulture) + ")";
                        StTestHelper.Write(wkt);

                        IEsriStGeometryType geo = StTestHelper.GetGeometry(wkt, _con);
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
                    }
                }

                tr.Rollback();
            }
        }

        #endregion Limits

        #region WKT

        [TestMethod]
        public void TeStPointWkt()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                string wkt = "POINT (0.0001 0.0002)";
                StTestHelper.Write(wkt);

                var geo = new StPoint(0.0001m, 0.0002m);
                StTestHelper.Write(geo);

                Assert.AreEqual(wkt, geo.ToString());

                tr.Rollback();
            }
        }

        [TestMethod]
        public void TeStPointZWkt()
        {
            using (OracleTransaction tr = _con.BeginTransaction())
            {
                string wkt = "POINT Z (0.0001 0.0002 10)";
                StTestHelper.Write(wkt);

                var geo = new StPoint(0.0001m, 0.0002m, 10);
                StTestHelper.Write(geo);

                Assert.AreEqual(wkt, geo.ToString());

                tr.Rollback();
            }
        }

        #endregion WKT
    }
}