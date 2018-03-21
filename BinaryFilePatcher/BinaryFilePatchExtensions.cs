using System;
using System.Text;

namespace Invertex.BinaryFilePatcher.Extensions
{
    public static class BinaryFilePatchExtensions
    {
        public static string BytesToString(this byte[] bytes, string addBetween = "")
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte bite in bytes)
            {
                sb.Append(bite).Append(addBetween);
            }
            sb.Length--;
            return sb.ToString();
        }

        public static byte[] HexStringToBytes(this string hex)
        {
            try
            {
                hex = hex.Replace(" ", string.Empty).Replace("0x", string.Empty).ToUpper();
                if (hex.Length % 2 == 1)
                {
                    Console.WriteLine("Hex string does not have even number of characters: " + hex);
                    return new byte[] { };
                }

                byte[] arr = new byte[hex.Length >> 1];

                for (int i = 0; i < hex.Length >> 1; ++i)
                {
                    arr[i] = (byte)(( (hex[i << 1]).GetHexVal() << 4) + ((hex[(i << 1) + 1]).GetHexVal()));
                }
                return arr;
            }
            catch (Exception)
            {
                Console.WriteLine("Incorrectly formatted HEX string: " + hex);
            }

            return new byte[] { };
        }

        public static int GetHexVal(this char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : 55);
        }
    }
}
