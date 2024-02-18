using System;
using System.Collections.Generic;
using System.Linq;

public class Cipher
{
    private Dictionary<char, char> encodeMap;
    private Dictionary<char, char> decodeMap;

    public Cipher() // Corrected the constructor name to match the class name
    {
        InitializeMaps();
    }

    private void InitializeMaps()
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        const string cipher =  "M3Fi54nWX1w7C6IzaTRhbuxkvBQdpGsNVU9JmO8qKZSfyHrcAj0Dto2LElYPeg";
        
        encodeMap = new Dictionary<char, char>();
        decodeMap = new Dictionary<char, char>();

        for (int i = 0; i < letters.Length; i++)
        {
            encodeMap.Add(letters[i], cipher[i]);
            decodeMap.Add(cipher[i], letters[i]);
        }
    }

    public string Encode(string input)
    {
        return new string(input.Select(c => encodeMap.ContainsKey(c) ? encodeMap[c] : c).ToArray());
    }

    public string Decode(string input)
    {
        return new string(input.Select(c => decodeMap.ContainsKey(c) ? decodeMap[c] : c).ToArray());
    }
}
