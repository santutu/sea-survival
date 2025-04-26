using System.IO;
using UnityEditor;
using UnityEngine;

namespace Santutu.Core.Base.Editor.Helpers
{
    public static class AssetHelper
    {
        public static T LoadOrCreateAsset<T>(string assetPath) where T : ScriptableObject
        {
            string folderPath = Path.GetDirectoryName(assetPath)?.Replace("\\", "/");

            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                return asset;
            }

            EnsureFolderExists(folderPath);

            asset = ScriptableObject.CreateInstance<T>();

            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"Created {assetPath}", asset);


            return asset;
        }


        public static void EnsureFolderExists(string folderPath)
        {
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
        }
    }
}