using Gry.ArcGis.NetStGeometry.Geometry.Base;

namespace Gry.ArcGis.NetStGeometry.Geometry.MultiPart
{
    class StMultiPolygon : IStMultiGeometry
    {
        public bool HasZ { get; private set; }
        public bool HasM { get; private set; }
        public byte[] Bytes { get; private set; }
        public string Wkt { get; private set; }
    }
}
