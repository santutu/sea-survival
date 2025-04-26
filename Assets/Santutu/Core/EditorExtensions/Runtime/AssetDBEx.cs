using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Santutu.Editors.EditorExtensions.Runtime
{
    public static class AssetDBEx
    {
        public static List<T> LoadAssets<T>(string folderPath, SearchOption searchOption) where T : Object
        {
#if UNITY_EDITOR
            string[] assetPaths = Directory.GetFiles(folderPath, "*.asset", searchOption);

            return assetPaths
                  .Select(path => AssetDatabase.LoadAssetAtPath<T>(path.Replace("\\", "/")))
                  .Where(asset => asset != null)
                  .ToList();
#else
            return new List<T>();

#endif
        }

        public static void EnsureFolderExists(string folderPath)
        {
#if UNITY_EDITOR
            if (folderPath == null)
            {
                return;
            }

            string[] parts = folderPath.Split('/');
            string currentPath = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string nextPath = $"{currentPath}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, parts[i]);
                }

                currentPath = nextPath;
            }
#endif
        }

        public static T CreateScriptableObjectAsAsset<T>(string assetPath) where T : ScriptableObject
        {
#if UNITY_EDITOR
            var so = ScriptableObject.CreateInstance<T>();

            var dirPath = Path.GetDirectoryName(assetPath);
            EnsureFolderExists(dirPath);

            string uniqueAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

            AssetDatabase.CreateAsset(so, uniqueAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var newAsset = AssetDatabase.LoadAssetAtPath<T>(uniqueAssetPath);
            Debug.Log($"TableDef created and saved at: {uniqueAssetPath}", newAsset);

            return newAsset;

#else
            return null;

#endif
        }
    }
}