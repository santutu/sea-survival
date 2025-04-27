// ---------------------------------------------------------------------------- 
// Author: Ryan Hipple
// Date:   05/07/2018
// ----------------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Santutu.Modules.MultiScenes.Runtime.Models
{
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
        public class SceneLoadException : Exception
        {
            public SceneLoadException(string message) : base(message)
            {
            }
        }

#if UNITY_EDITOR
        public UnityEditor.SceneAsset scene;
#endif

        [Tooltip("The name of the referenced scene. This may be used at runtime to load the scene.")]
        public string name;

        [SerializeField] private int sceneIndex = -1;

        [SerializeField] private bool sceneEnabled;

        private void ValidateScene()
        {
            if (string.IsNullOrEmpty(name))
                throw new SceneLoadException("No scene specified.");

            if (sceneIndex < 0)
                throw new SceneLoadException("Scene " + name + " is not in the build settings");

            if (!sceneEnabled)
                throw new SceneLoadException("Scene " + name + " is not enabled in the build settings");
        }

        public void LoadScene(LoadSceneMode mode = LoadSceneMode.Single)
        {
            ValidateScene();
            SceneManager.LoadScene(name, mode);
        }

        public AsyncOperation LoadSceneAsync(LoadSceneMode mode = LoadSceneMode.Single)
        {
            ValidateScene();
            return SceneManager.LoadSceneAsync(name, mode);
        }


        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (scene != null)
            {
                string sceneAssetPath = UnityEditor.AssetDatabase.GetAssetPath(scene);
                string sceneAssetGUID = UnityEditor.AssetDatabase.AssetPathToGUID(sceneAssetPath);

                var scenes =
                    UnityEditor.EditorBuildSettings.scenes;

                sceneIndex = -1;
                for (int i = 0; i < scenes.Length; i++)
                {
                    if (scenes[i].guid.ToString() == sceneAssetGUID)
                    {
                        sceneIndex = i;
                        sceneEnabled = scenes[i].enabled;
                        if (scenes[i].enabled)
                            name = scene.name;
                        break;
                    }
                }
            }
            else
            {
                name = "";
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}