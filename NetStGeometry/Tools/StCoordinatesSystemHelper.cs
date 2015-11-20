using System;
using System.Collections.Generic;

namespace Gry.ArcGis.NetStGeometry.Tools
{
    /// <summary>
    /// Helper methods for STCoordinatesSystem class
    /// </summary>
    public static class StCoordinatesSystemHelper
    {
        /// <summary>
        /// Returns byte array from integer value
        /// </summary>
        /// <param name="val"></param>
        /// <param name="coordinatesSystem"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromValue(int val, StCoordinatesSystem coordinatesSystem)
        {
            // Search how many bytes have to be encoded
            int maxIndex = coordinatesSystem.ByteFactors.Length - 1;
            long byteCoeff = 1;
            for (int i = 0; i < coordinatesSystem.ByteCoeff.Length; ++i)
            {
                byteCoeff *= coordinatesSystem.ByteCoeff[i];
            }

            while (maxIndex > 0 && val < byteCoeff)
            {
                byteCoeff /= coordinatesSystem.ByteCoeff[maxIndex];
                maxIndex--;
            }

            var bytes = new List<byte>();

            // Encode utlimate byte first
            decimal factor = coordinatesSystem.ByteFactors[maxIndex] / coordinatesSystem.Resolution;
            var byteValue = (byte)(val / factor);
            decimal restVal = val - (byteValue * factor);
            bytes.Insert(0, byteValue);

            // Encode bytes from penultimate to first
            for (int i = maxIndex - 1; i >= 0; --i)
            {
                factor = coordinatesSystem.ByteFactors[i] / coordinatesSystem.Resolution;
                byteValue = (byte)(restVal / factor);
                restVal -= byteValue * factor;

                // byteValue cannot be < 128 (RestartValue)
                byteValue += coordinatesSystem.RestartValue;
                if (byteValue < coordinatesSystem.RestartValue || byteValue > 255)
                    throw new IndexOutOfRangeException();

                bytes.Insert(0, byteValue);
            }

            if (val < 0)
                SetNegative(bytes, coordinatesSystem);

            return bytes.ToArray();
        }

        public static byte[] GetBytesFromValue(decimal val, StCoordinatesSystem coordinatesSystem, StCoordType type)
        {
            if (type == StCoordType.M)
                throw new NotSupportedException("M Coordiante is not supported yet");

            bool isNegative = false;
            decimal valFromMin = val - coordinatesSystem.GetMin(type);
            if (valFromMin < 0)
            {
                // We have to encode a negative number
                isNegative = true;
                valFromMin = Math.Abs(valFromMin);
            }

            // Search how many bytes have to be encoded
            int maxIndex = coordinatesSystem.ByteFactors.Length - 1;
            while (maxIndex > 0 && valFromMin < coordinatesSystem.ByteFactors[maxIndex])
            {
                maxIndex--;
            }

            var bytes = new List<byte>();

            // Encode utlimate byte first
            decimal div = valFromMin / coordinatesSystem.ByteFactors[maxIndex];
            var byteValue = (byte)Math.Floor(div);
            decimal restVal = valFromMin - (byteValue * coordinatesSystem.ByteFactors[maxIndex]);
            bytes.Insert(0, byteValue);

            // Encode bytes from penultimate to first
            for (int i = maxIndex - 1; i >= 0; --i)
            {
                div = restVal / coordinatesSystem.ByteFactors[i];
                byteValue = (byte)Math.Floor(div);
                restVal -= byteValue * coordinatesSystem.ByteFactors[i];

                // byteValue cannot be < 128 (RestartValue)
                byteValue += coordinatesSystem.RestartValue;
                if (byteValue < coordinatesSystem.RestartValue || byteValue > 255)
                    throw new IndexOutOfRangeException();

                bytes.Insert(0, byteValue);
            }

            if (isNegative)
                SetNegative(bytes, coordinatesSystem);

            return bytes.ToArray();
        }

        public static decimal GetValueFromBytes(byte[] bytes, StCoordinatesSystem coordinatesSystem, StCoordType type)
        {
            if (type == StCoordType.M)
                throw new NotSupportedException("M-Coordinates are not supported yet");

            decimal val = 0;

            // The first byte will give the sign
            byte b;
            bool isNegative = IsNegative(bytes, coordinatesSystem, out b);
            val += b * coordinatesSystem.ByteFactors[0];

            // Next bytes
            for (int i = 1; i < bytes.Length; ++i)
            {
                b = bytes[i];
                if (b >= coordinatesSystem.ByteCoeff[i + 1])
                    b = (byte)(b - coordinatesSystem.RestartValue);
                val += b * coordinatesSystem.ByteFactors[i];
            }

            if (isNegative)
                return coordinatesSystem.GetMin(type) - val;

            return val + coordinatesSystem.GetMin(type);
        }

        public static byte[] GetBytesFromDifference(byte[] initialBytes, byte[] previousBytes, StCoordinatesSystem coordinatesSystem, StCoordType type)
        {
            // Exemple : LINESTRING (-1 -1, 0 0) 
            //  - initial bytes  : { 208 156 1 } ==> the value of the point is 0
            //                                       but the difference with the previous point is encoded => 1
            //  - previous bytes : { 176 139 197 186 141 17 } ==> this is the encoding for -1
            // 

            // Here we get the value decoded from the bytes representing the difference
            decimal encodedDifference = GetValueFromBytes(initialBytes, coordinatesSystem, type);

            // Get The previous value
            decimal valuePrevious = GetValueFromBytes(previousBytes, coordinatesSystem, type);
            decimal encodedPrevious = valuePrevious - coordinatesSystem.GetMin(type);

            // Calculate new value
            decimal value = encodedPrevious + encodedDifference;

            return GetBytesFromValue(value, coordinatesSystem, type);
        }

        public static byte[] GetDifferenceFromBytes(byte[] initialBytes, byte[] previousBytes, StCoordinatesSystem coordinatesSystem, StCoordType type)
        {
            // We mut encode the difference with the previous coordinate instead of the real value
            decimal initialRealValue = GetValueFromBytes(initialBytes, coordinatesSystem, type);

            // Get The previous value
            decimal previousValue = GetValueFromBytes(previousBytes, coordinatesSystem, type);

            // Get the difference
            decimal difference = initialRealValue - previousValue;

            // Get the value to encode
            decimal valueToEncode = coordinatesSystem.GetMin(type) + difference;

            return GetBytesFromValue(valueToEncode, coordinatesSystem, type);
        }

        private static bool IsNegative(byte[] bytes, StCoordinatesSystem coordinatesSystem, out byte firstByteRealValue)
        {
            if (bytes.Length == 1)
            {
                // If there is only 1 encoding byte, then the value is negative if the value is bigger than coordinatesSystem.ByteCoeff[1]
                // Ex : 63 is positive ( = 63 )
                // but  65 is negative ( = -1 )
                if (bytes[0] >= coordinatesSystem.ByteCoeff[1])
                {
                    firstByteRealValue = (byte)(bytes[0] - coordinatesSystem.ByteCoeff[1]);
                    return true;
                }

                firstByteRealValue = bytes[0];
                return false;
            }

            // If there are many encoding bytes, then the value is negative if the value is bigger than coordinatesSystem.ByteCoeff[1] + RestartValue
            // Ex : 130 is positive ( = 2 )
            // but  193 is negative ( = -1 )
            int negativeValue = coordinatesSystem.ByteCoeff[1] + coordinatesSystem.RestartValue;
            if (bytes[0] >= negativeValue)
            {
                firstByteRealValue = (byte)(bytes[0] - negativeValue);
                return true;
            }

            firstByteRealValue = (byte)(bytes[0] - coordinatesSystem.RestartValue);
            return false;
        }

        public static void SetNegative(List<byte> bytes, StCoordinatesSystem coordinatesSystem)
        {
            if (bytes.Count == 1)
            {
                // If there is only 1 encoding byte, then the value is negative if the value is bigger than coordinatesSystem.ByteCoeff[1]
                // We can only encode this if the value of the first byte is < coordinatesSystem.ByteCoeff[1], otherwise we have a problem
                if (bytes[0] > coordinatesSystem.ByteCoeff[1])
                    throw new ArgumentException();

                bytes[0] += coordinatesSystem.ByteCoeff[1];
            }
            else
            {
                // If there are many encoding bytes, then the value is negative if the value is bigger than coordinatesSystem.ByteCoeff[1] + RestartValue
                // We can only encode this if : coordinatesSystem.RestartValue < first byte < coordinatesSystem.RestartValue + coordinatesSystem.ByteCoeff[1], otherwise we have a problem
                if (bytes[0] < coordinatesSystem.RestartValue || bytes[0] > coordinatesSystem.RestartValue + coordinatesSystem.ByteCoeff[1])
                    throw new ArgumentException();

                bytes[0] += coordinatesSystem.ByteCoeff[1];
            }
        }
    }
}