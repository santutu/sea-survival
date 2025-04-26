using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Santutu.Core.GameObjectTraveler.Runtime;


namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendRenderer
    {
        public static void ExceptMaterials(this Renderer renderer, Func<Material, bool> except)
        {
            var newMaterials = renderer.materials;

            newMaterials = newMaterials
                          .Where((mat) => !except(mat))
                          .ToArray();

            renderer.materials = newMaterials;
        }

        public static void AddMaterials(this Renderer renderer, IEnumerable<Material> materials)
        {
            renderer.materials =
                renderer.sharedMaterials.Concat(materials).ToArray();
        }

        public static void AddMaterials(this Renderer renderer, Material material)
        {
            renderer.materials =
                renderer.sharedMaterials.Concat(new[] { material }).ToArray();
        }

        public static IEnumerable<Renderer> AddMaterials(this IEnumerable<Renderer> renderers, Material material)
        {
            foreach (var renderer in renderers)
            {
                renderer.AddMaterials(material);
            }

            return renderers;
        }

        public static void ExceptMaterials(this IEnumerable<Renderer> renderers, Func<Material, bool> except)
        {
            foreach (var renderer in renderers)
            {
                renderer.ExceptMaterials(except);
            }
        }

        public static void SetMaterials(this Renderer renderer, Material material)
        {
            renderer.materials = renderer.materials.Select(m => material).ToArray();
        }

        public static void SetMaterials(this Renderer renderer, Material[] materials)
        {
            renderer.materials = materials;
        }

        public static void SetMaterials(this IEnumerable<Renderer> renderers, Material material)
        {
            foreach (var renderer in renderers)
            {
                SetMaterials(renderer, material);
            }
        }

        public static void SetSharedMaterials(this Renderer renderer, Material material)
        {
            renderer.sharedMaterials = renderer.sharedMaterials.Select(m => material).ToArray();
        }


        public static void SetSharedMaterials(this Renderer renderer, Material[] materials)
        {
            renderer.sharedMaterials = materials;
        }

        public static void SetSharedMaterials(this IEnumerable<Renderer> renderers, Material material)
        {
            foreach (var renderer in renderers)
            {
                SetSharedMaterials(renderer, material);
            }
        }


        public static void SetMaterials(this IEnumerable<Renderer> renderers, Material[] materials)
        {
            foreach (var renderer in renderers)
            {
                renderer.materials = materials;
            }
        }

        public static void SetSharedMaterialsInSelfBelowRenderers(this GameObject target, Material material)
        {
            foreach (var go in target.SelfBelow())
            {
                var renderers = go.GetComponents<Renderer>();
                renderers.SetSharedMaterials(material);
            }
        }

        public static void SetMaterialsInSelfBelowRenderers(this GameObject target, Material material)
        {
            foreach (var go in target.SelfBelow())
            {
                var renderers = go.GetComponents<Renderer>();
                renderers.SetMaterials(material);
            }
        }
    }
}