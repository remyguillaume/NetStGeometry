using System;

namespace Gry.ArcGis.NetStGeometry.Tools
{
    /// <summary>
    /// Represents an Esri Coordinate System.
    /// This is the most important object in the whole library.
    /// This has to be defined correctly, otherwise every other operations won't work.
    /// An object of this type MUST BE defined and set to STParameters.CoordiantesSystem property
    /// otherwise the library won't work.
    /// </summary>
    public class StCoordinatesSystem
    {
        private readonly bool _hasZ;

        /// <summary>
        /// Creates a new Coordinates system, that will be used in the whole NetSTGeometry library
        /// </summary>
        /// <param name="srid">SRID Number</param>
        /// <param name="sridName"> Coordinate System Name. This value is only for Info. It does not need to be set.</param>
        /// <param name="resolution">Resolution (use the same value as the configuration value of the FeatureClass in ArcGis)</param>
        /// <param name="tolerance">Tolerance (use the same value as the configuration value of the FeatureClass in ArcGis). Default will be Resolution*10</param>
        /// <param name="minX">Minimal X Value (use the same value as the configuration value of the FeatureClass in ArcGis)</param>
        /// <param name="maxX">Maximal X Value (only used for unittests yet)</param>
        /// <param name="minY">Minimal Y Value (use the same value as the configuration value of the FeatureClass in ArcGis)</param>
        /// <param name="maxY">Maximal Y Value (only used for unittests yet)</param>
        /// <param name="minZ">Minimal Z Value (use the same value as the configuration value of the FeatureClass in ArcGis)</param>
        /// <param name="maxZ">Maximal Z Value (only used for unittests yet)</param>
        /// <param name="byteCoeff">The value of each bytes when decoding the blob representing the geometry in the Oralce UDT. Default value : { 1, 64, 128, 128, 128, 128, 128, 128, 128 }</param>
        /// <param name="restartValue">Value for a byte indicating that there is a next byte. Default Value : 128</param>
        /// <exception cref="ArgumentException"></exception>
        public StCoordinatesSystem(int srid,
                                   string sridName,
                                   decimal resolution,
                                   decimal? tolerance,
                                   decimal minX,
                                   decimal? maxX,
                                   decimal minY,
                                   decimal? maxY,
                                   decimal? minZ,
                                   decimal? maxZ,
                                   byte[] byteCoeff,
                                   byte? restartValue)
        {
            Srid = srid;
            SridName = sridName;
            Resolution = resolution;
            Tolerance = tolerance.HasValue ? tolerance.Value : resolution * 10;

            MinX = minX;
            MaxX = maxX.HasValue ? maxX.Value : Decimal.MaxValue;
            MinY = minY;
            MaxY = maxY.HasValue ? maxY.Value : Decimal.MaxValue;

            _hasZ = minZ.HasValue;
            MinZ = minZ.HasValue ? minZ.Value : Decimal.MinValue;
            MaxZ = maxZ.HasValue ? maxZ.Value : Decimal.MaxValue;

            ByteCoeff = byteCoeff ?? new byte[] { 1, 64, 128, 128, 128, 128, 128, 128, 128 };

            RestartValue = restartValue.HasValue ? restartValue.Value : (byte)128;

            ByteFactors = new decimal[ByteCoeff.Length];
            ByteFactors[0] = Resolution;
            for (int i = 1; i < ByteCoeff.Length; ++i)
            {
                ByteFactors[i] = ByteFactors[i - 1] * ByteCoeff[i];
            }
        }

        /// <summary>
        /// Coordinate System ID
        /// </summary>
        public int Srid { get; private set; }

        /// <summary>
        /// Coordinate System Name
        /// This value is only for Info. It does not need to be set
        /// </summary>
        public string SridName { get; private set; }

        /// <summary>
        /// Coordinates System Resolution.
        /// Every used Byte will have a value of this Resolution
        /// For now we assume that Resolution is the same for X, Y and Z values
        /// </summary>
        public decimal Resolution { get; private set; }

        /// <summary>
        /// Coordinates System tolerance
        /// When doing spatial operations, the values within this tolerance will be consider to have the same value
        /// </summary>
        public decimal Tolerance { get; private set; }

        /// <summary>
        /// How many of previous-level bytes is represented with a byte of current-level.
        /// Example value : { 1, 64, 128, 128, 128, 128, 128, 128, 128 }
        /// ex : 1 byte of level 1 = 64 bytes of level 0
        ///      1 byte of level 2 = 128 bytes of level 1
        /// </summary>
        public byte[] ByteCoeff { get; private set; }

        /// <summary>
        /// How much is a byte ?
        /// The array gives a value for each byte set to a specific index.
        /// </summary>
        public decimal[] ByteFactors { get; private set; }

        /// <summary>
        /// The value of a byte is encoded from 0, up to the corresponding ByteCoeff
        /// But when there is a next byte, the count restart at this Restart Value
        /// This is the way to know that there is a next byte.
        /// ex : { 63 }    => The value of the byte is 63, there is not next byte
        ///      { 128 1 } => The value of the byte is 0 (128 - RestartValue), and there is a next byte.
        ///                   The value of the next byte is 64 (1 * corresponding ByteCoeff)
        /// </summary>
        public byte RestartValue { get; private set; }

        /// <summary>
        /// Minimal X value allowed in this coordinates system
        /// This is the more important configuration value : The bytes encoding of this Value must be { 0 }
        /// </summary>
        public decimal MinX { get; private set; }

        /// <summary>
        /// Maximal X value allowed in this coordinates system
        /// </summary>
        public decimal MaxX { get; private set; }

        /// <summary>
        /// Minimal Y value allowed in this coordinates system
        /// This is the more important configuration value : The bytes encoding of this Value must be { 0 }
        /// </summary>
        public decimal MinY { get; private set; }

        /// <summary>
        /// Maximal Y value allowed in this coordinates system
        /// </summary>
        public decimal MaxY { get; private set; }

        /// <summary>
        /// Minimal Z value allowed in this coordinates system
        /// This is the more important configuration value : The bytes encoding of this Value must be { 0 }
        /// </summary>
        public decimal MinZ { get; private set; }

        /// <summary>
        /// Maximal Z value allowed in this coordinates system
        /// </summary>
        public decimal MaxZ { get; private set; }

        /// <summary>
        /// Gets the minimal value, depending on the type of coordinate (X/Y/Z/M)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public decimal GetMin(StCoordType type)
        {
            switch (type)
            {
                case StCoordType.X:
                    return MinX;
                case StCoordType.Y:
                    return MinY;
                case StCoordType.Z:
                    if (!_hasZ)
                        throw new ArgumentException("This STCoordinatesSystem was not initialized for Z coordinates. Please provide a regular minZ value in the constructor");
                    return MinZ;
                default:
                    throw new ArgumentException();
            }
        }
    }
}