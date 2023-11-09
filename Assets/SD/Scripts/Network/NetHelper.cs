using System;
using System.Security.Cryptography;
using System.Text;



public class NetHelper
{
    public static string k_ApiServer ="https://igrovebackendtest.azurewebsites.net";

    
    
    //es ori raari gavarkviot
    private static string key = "QowrjSIK1yVkkgxECcaAi13kaMtJxSxYrxJwTtKAWW8=";
    private static string iv = "TeqCD/Jn+CDyf7J/lNFRFg==";
    public static string Encrypt(string plainText)
    {
        var aes = new RijndaelManaged();
        aes.IV = Convert.FromBase64String(iv);
        aes.Key = Convert.FromBase64String(key);
        aes.Mode = CipherMode.CBC;
        var textBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var encryptedBytes = encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

        return Convert.ToBase64String(encryptedBytes);
    }
    
    public static string Decrypt(string bs64EncryptedText)
    {
        var aes = new RijndaelManaged();
        aes.IV = Convert.FromBase64String(iv);
        aes.Key = Convert.FromBase64String(key);
        aes.Mode = CipherMode.CBC;
        var textBytes = Convert.FromBase64String(bs64EncryptedText);
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var decryptedBytes = Encoding.UTF8.GetString(decryptor.TransformFinalBlock(textBytes, 0, textBytes.Length));

        return decryptedBytes;
    }
}
