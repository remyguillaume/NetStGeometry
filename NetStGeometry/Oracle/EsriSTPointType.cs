using System;
using Gry.ArcGis.NetStGeometry.Geometry;
using Gry.ArcGis.NetStGeometry.Geometry.Base;
using Oracle.DataAccess.Types;

namespace Gry.ArcGis.NetStGeometry.Oracle
{
    /// <summary>
    /// Oracle parameter type representing an Oracle UDT object of type SDE.ST_POINT
    /// The is the same as EsrIStGeometryType class, but when reading with another function, the same type cannot be used for different OracleCustomTypeMapping attributes.
    /// Therefore, we have to define a diferent type here.
    /// Will be used when reading an Esri ST_Point object in Oracle
    /// </summary>
    [Serializable]
    [OracleCustomTypeMapping("SDE.ST_POINT")]
    public class EsriStPointType : OracleCustomTypeBase<EsriStPointType>, IEsrIStGeometryType
    {
        private IStGeometry _geometry;

        #region IEsrIStGeometryType Members

        [OracleObjectMapping("ENTITY")]
        public int Entity { get; set; }
        [OracleObjectMapping("NUMPTS")]
        public int NumPts { get; set; }
        [OracleObjectMapping("MINX")]
        public decimal MinX { get; set; }
        [OracleObjectMapping("MINY")]
        public decimal MinY { get; set; }
        [OracleObjectMapping("MAXX")]
        public decimal MaxX { get; set; }
        [OracleObjectMapping("MAXY")]
        public decimal MaxY { get; set; }
        [OracleObjectMapping("MINZ")]
        public decimal? MinZ { get; set; }
        [OracleObjectMapping("MAXZ")]
        public decimal? MaxZ { get; set; }
        [OracleObjectMapping("MINM")]
        public decimal? MinM { get; set; }
        [OracleObjectMapping("MAXM")]
        public decimal? MaxM { get; set; }
        [OracleObjectMapping("AREA")]
        public decimal Area { get; set; }
        [OracleObjectMapping("LEN")]
        public decimal Len { get; set; }
        [OracleObjectMapping("SRID")]
        public int Srid { get; set; }
        [OracleObjectMapping("POINTS")]
        public byte[] Points { get; set; }

        public IStGeometry Geometry
        {
            get { return _geometry; }
            set
            {
                _geometry = value;
                EsrIStGeometryTypeHelper.CalculateOtherValuesFromGeometry(this);
            }
        }

        #endregion

        protected override void MapFromCustomObject()
        {
            EsrIStGeometryTypeHelper.MapFromCustomObject(this);
        }

        protected override void MapToCustomObject()
        {
            EsrIStGeometryTypeHelper.MapToCustomObject(this);
        }
    }
}