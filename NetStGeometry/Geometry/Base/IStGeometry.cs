namespace Gry.ArcGis.NetStGeometry.Geometry.Base
{
    /// <summary>
    /// Interface representing an Esri ST_Geometry in Oracle
    /// </summary>
    public interface IStGeometry
    {
        bool HasZ { get; }
        bool HasM { get; }

        byte[] Bytes { get; }
        string Wkt { get; }
    }
}