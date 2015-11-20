using Gry.ArcGis.NetStGeometry.Geometry;

namespace Gry.ArcGis.NetStGeometry.Oracle
{
    /// <summary>
    /// Interface representing a Oracle UDT object of type SDE.ST_GEOMETRY.
    /// </summary>
    public interface IEsrIStGeometryType
    {
        int Entity { get; set; }
        int NumPts { get; set; }
        decimal MinX { get; set; }
        decimal MinY { get; set; }
        decimal MaxX { get; set; }
        decimal MaxY { get; set; }
        decimal? MinZ { get; set; }
        decimal? MaxZ { get; set; }
        decimal? MinM { get; set; }
        decimal? MaxM { get; set; }
        decimal Area { get; set; }
        decimal Len { get; set; }
        int Srid { get; set; }
        byte[] Points { get; set; }
        IStGeometry Geometry { get; set; }

        void SetValue(string oracleColumnName, object value);

        void SetValue(int oracleColumnId, object value);

        TU GetValue<TU>(string oracleColumnName);

        TU GetValue<TU>(int oracleColumnId);
    }
}