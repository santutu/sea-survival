using System;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

namespace Santutu.Core.Runtime.Extensions.Statics
{
    public static class ScreenEx
    {

        public static string GetResolutionKey(Resolution resolution)
        {
            return $"{resolution.width} x {resolution.height}"; 
        }
        
        public static string GetCurrentResolutionKey()
        {
            return $"{Screen.currentResolution.width} x {Screen.currentResolution.height}";
        }

        public static void SetResolutionByKey(string key, FullScreenMode fullScreenMode)
        {
            
            string[] resolution = key.Split(new string[] { " x " }, System.StringSplitOptions.None);
            
            if (resolution.Length != 2)
            {
                throw new Exception($"resolution length should be 2 but was {resolution.Length}");
            }
            
            int width = int.Parse(resolution[0]);
            int height = int.Parse(resolution[1]);
                
            Screen.SetResolution(width, height, fullScreenMode);
        }
    }
}