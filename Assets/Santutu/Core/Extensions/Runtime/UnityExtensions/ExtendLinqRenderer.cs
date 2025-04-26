using System.Collections.Generic;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendLinqRenderer
    {
        public static IEnumerable<Material> SelectMaterials(this IEnumerable<Renderer> renderers)
        {
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    yield return material;
                }
            }
        }
    }
}