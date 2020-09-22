using System;

/// 字符串转16进制字节
public class StringToByteArray  {
    
    public static byte[] strsToHexByte(string hexString)
    {
        hexString = hexString.Replace(" ", "");
        if ((hexString.Length % 2) != 0)
            hexString += " ";
        byte[] returnBytes = new byte[hexString.Length / 2];
        
        for (int i = 0; i < returnBytes.Length; i++)
        {
            returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
        }
        return returnBytes;
    }

    public static byte[] strsToHexByte(string[] hexString)
    {
        byte[] returnBytes = new byte[hexString.Length];

        for (int i = 0; i < returnBytes.Length; i++)
        {
            returnBytes[i] = strToHexByte(hexString[i]);
        }

        return returnBytes;
    }

    
    public static byte strToHexByte(string hexString)
    {
        hexString = hexString.Replace(" ", "");
        return Convert.ToByte(hexString.Substring(0, 2), 16);
    }

    public static int strToHexInt(string hexString)
    {
        hexString = hexString.Replace(" ", "");
        return Convert.ToInt32(hexString, 16);
    }
}
