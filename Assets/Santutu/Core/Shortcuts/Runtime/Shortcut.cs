using System;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Santutu.Library.Shortcuts.Runtime
{
    [Serializable]
    public class Shortcut
    {
        [SerializeField]
#if ODIN_INSPECTOR
        [ListDrawerSettings(ShowFoldout = true, DefaultExpandedState = true, ShowIndexLabels = false)]
#endif
        public List<KeyCode> modifiers = new List<KeyCode>();

        [SerializeField] public KeyCode key;


        public bool IsInvoked()
        {
            foreach (var keyCode in modifiers)
            {
                if (!Input.GetKey(keyCode))
                {
                    return false;
                }
            }

            return Input.GetKeyDown(key);
        }
    }
}