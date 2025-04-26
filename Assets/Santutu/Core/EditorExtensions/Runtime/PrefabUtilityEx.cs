using System.IO;
using UnityEngine;

namespace Santutu.Editors.EditorExtensions.Runtime
{
    public static class PrefabUtilityEx
    {
        public static void RecordPrefabInstancePropertyModifications(Object targetObject)
        {
#if UNITY_EDITOR
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(targetObject);
#endif
        }

        public static GameObject SaveAsPrefabAsset(GameObject go, string path)
        {
#if UNITY_EDITOR

            string folderPath = Path.GetDirectoryName(path)?.Replace("\\", "/");

            AssetDatabaseEx.EnsureFolderExists(folderPath);
            var prefab = UnityEditor.PrefabUtility.SaveAsPrefabAsset(go, path);

            Debug.Log($"Prefab saved at {path}", prefab);

            return prefab;
#else
            return null;
#endif
        }

        public static GameObject SavePrefabAsset(GameObject go)
        {
#if UNITY_EDITOR
            return UnityEditor.PrefabUtility.SavePrefabAsset(go);
#else
            return null;
#endif
        }

        public static GameObject InstantiatePrefab(Object assetComponentOrGameObject)
        {
#if UNITY_EDITOR
            return (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(assetComponentOrGameObject);
#else
            return null;
#endif
        }
    }
}