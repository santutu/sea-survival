using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendImage
    {
        public static Image SetAlpha(this Image image, float alpha)
        {
            var color = image.color;
            color.a = alpha;
            image.color = color;
            return image;
        }
    }
}