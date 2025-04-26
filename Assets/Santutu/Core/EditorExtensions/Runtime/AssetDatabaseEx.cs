using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Santutu.Editors.EditorExtensions.Runtime
{
    public static class AssetDatabaseEx
    {
        public static bool IsValidFolder(string path)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.IsValidFolder(path);
#else
            return false;
#endif
        }

        public static string GetAssetFolderPath(this Object asset)
        {
            var filePath = GetAssetPath(asset);
            return Path.GetDirectoryName(filePath);
        }

        public static string GetAssetPath(this Object asset)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(asset);
#else
            return null;
#endif
        }

        public static void SaveAsset(Object target)
        {
            EditorUtilityEx.SetDirty(target);
            SaveAssets();
        }

        public static void SaveAssets()
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        public static void CreateAssetAsUniquePath(Object asset, string assetPath)
        {
#if UNITY_EDITOR
            string uniquePath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(assetPath);
            UnityEditor.AssetDatabase.CreateAsset(asset, uniquePath);
#endif
        }

        
        
        public static void CreateAsset(Object asset, string assetPath)
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
#endif
        }


        public static T LoadOrCreateAsset<T>(string assetPath) where T : ScriptableObject
        {
#if UNITY_EDITOR
            string folderPath = Path.GetDirectoryName(assetPath)?.Replace("\\", "/");

            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                return asset;
            }

            EnsureFolderExists(folderPath);

            asset = ScriptableObject.CreateInstance<T>();

            UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();

            Debug.Log($"Created {assetPath}", asset);


            return asset;
#else
            return null;
#endif
        }


        public static void EnsureFolderExists(string folderPath)
        {
#if UNITY_EDITOR
            string[] parts = folderPath.Split('/');
            string currentPath = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string nextPath = $"{currentPath}/{parts[i]}";
                if (!UnityEditor.AssetDatabase.IsValidFolder(nextPath))
                {
                    UnityEditor.AssetDatabase.CreateFolder(currentPath, parts[i]);
                }

                currentPath = nextPath;
            }
#endif
        }
        public static string ExtractAssertRelativePath(string absolutePath)
        {
            int index = absolutePath.IndexOf("Assets", StringComparison.OrdinalIgnoreCase);

            if (index == -1)
            {
                throw new Exception();
            }

            return absolutePath.Substring(index);
        }
    }
}