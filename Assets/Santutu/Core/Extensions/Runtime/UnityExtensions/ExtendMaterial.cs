using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendMaterial
    {
        public static Material Instantiate(this Material mat)
        {
            return Object.Instantiate(mat);
        }

        public static string GetInstanceName(this Material mat)
        {
            return $"{mat.name} (Instance)";
        }

        public static bool IsInstanceEquals(this Material loaded, Material instance)
        {
            return loaded.renderQueue == instance.renderQueue && loaded.GetInstanceName() == instance.name;
        }

        public static void SetTransparent(this Material mat)
        {
            if (!mat.HasModeProperty())
            {
                throw new Exception($"{mat}, {mat.name} doesn't have mode property. can not be transparent.");
            }

            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }

        public static void SetTransparent(this IEnumerable<Material> materials)
        {
            foreach (var mat in materials)
            {
                mat.SetTransparent();
            }
        }


        public static IEnumerable<Material> GetAllMaterials(this Transform tf)
        {
            return tf.GetComponentsInChildren<Renderer>()
                     .Select(renderer1 => renderer1.materials)
                     .SelectMany(x => x);
        }

        public static bool CanTransparent(this Material mat)
        {
            if (!mat.HasModeProperty())
            {
                return false;
            }

            return Math.Abs(mat.GetFloat("_Mode") - 3) < 1;
        }

        public static bool HasModeProperty(this Material mat)
        {
            return mat.HasProperty("_Mode");
        }

        public static void SetShader(this Material[] materials, Shader shader)
        {
            foreach (var material in materials)
            {
                material.shader = shader;
            }
        }

        public static void SetFloat(this Material[] materials, string name, float value)
        {
            foreach (var material in materials)
            {
                material.SetFloat(name, value);
            }
        }

        public static void SetTexture(this Material[] materials, int nameID, Texture value)
        {
            foreach (var material in materials)
            {
                material.SetTexture(nameID, value);
            }
        }


        public static void SetTexture(this Material[] materials, string name, Texture value)
        {
            foreach (var material in materials)
            {
                material.SetTexture(name, value);
            }
        }


        public static void CopyPropertiesFromMaterial(this Material[] materials, Material source)
        {
            foreach (var material in materials)
            {
                material.CopyPropertiesFromMaterial(source);
            }
        }


        public static void SetMainTexture(this Material[] materials, Texture texture)
        {
            foreach (var material in materials)
            {
                material.mainTexture = texture;
            }
        }
    }
}