using System;
using System.Collections.Generic;
using Santutu.Core.GameObjectTraveler.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendScene
    {
        public static T GetComponent<T>(this Scene scene) where T : class
        {
            foreach (var go in scene.GetAllGameObjects())
            {
                if (go.TryGetComponent<T>(out var comp))
                {
                    return comp;
                }
            }

            throw new Exception($"not found component {typeof(T).Name} in scene {scene.name}");
        }


        public static T GeComponentOrNull<T>(this Scene scene) where T : class
        {
            foreach (var go in scene.GetAllGameObjects())
            {
                if (go.TryGetComponent<T>(out var comp))
                {
                    return comp;
                }
            }

            return null;
        }

        public static IEnumerable<T> GetAllComponents<T>(this Scene scene)
        {
            foreach (var go in scene.GetAllGameObjects())
            {
                foreach (var comp in go.GetComponents<T>())
                {
                    yield return comp;
                }
            }
        }

        public static IEnumerable<GameObject> GetAllGameObjects(this Scene scene)
        {
            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                foreach (var gameObject in rootGameObject.SelfBelow())
                {
                    yield return gameObject;
                }
            }
        }
    }
}