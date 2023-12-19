using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteInEditMode]
public class AddNamespace : MonoBehaviour
{
#if UNITY_EDITOR
    public string folderPath;
    public bool doit;

    void Update()
    {
        if (doit)
        {
            ProcessScriptsInFolder(folderPath);
            doit = false;
        }
    }

    void ProcessScriptsInFolder(string path)
    {
        var fullPath = Path.Combine(Application.dataPath, path);
        var files = Directory.GetFiles(fullPath, "*.cs", SearchOption.TopDirectoryOnly);

        foreach (var file in files)
        {
            AddNamespaceToFile(file);
        }
    }

    void AddNamespaceToFile(string filePath)
    {
        string content = File.ReadAllText(filePath);
        if (!content.Contains("namespace"))
        {
            // Find the position after the last 'using' statement
            int insertionIndex = FindInsertionIndex(content);

            // Insert the namespace declaration at the found position
            string namespaceLine = "namespace ON.interaction\n{\n";
            string newContent = content.Insert(insertionIndex, namespaceLine) + "\n}";

            File.WriteAllText(filePath, newContent);
            Debug.Log($"Namespace added to {filePath}");
        }
        else
        {
            Debug.Log($"{filePath} already has a namespace.");
        }
    }

    int FindInsertionIndex(string content)
    {
        // Find the last 'using' statement or the start of the file content
        MatchCollection matches = Regex.Matches(content, @"^using [^;]+;", RegexOptions.Multiline);
        int lastIndex = matches.Count > 0 ? matches[matches.Count - 1].Index + matches[matches.Count - 1].Length : 0;

        // Find the first class/interface/enum/struct declaration
        Match match = Regex.Match(content, @"(class|interface|enum|struct) [^{]+{", RegexOptions.Multiline);
        int classIndex = match.Success ? match.Index : content.Length;

        // Return the position after the last 'using' statement but before the first type declaration
        return lastIndex > classIndex ? classIndex : lastIndex;
    }
#endif
}
