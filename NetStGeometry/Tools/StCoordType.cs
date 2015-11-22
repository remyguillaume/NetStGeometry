namespace Gry.ArcGis.NetStGeometry.Tools
{
    /// <summary>
    /// Represents the type of the coordinate (X/Y/Z/M)
    /// Every type has a different encoding in bytes, based on the user coordinate system.
    /// Therefore, encoding of the same value will be different for each coordinate type.
    /// When using STCoordinate, we have to use it with the right STCoordType
    /// </summary>
    public enum StCoordType
    {
        X,
        Y,
        Z,
        M
    }
}