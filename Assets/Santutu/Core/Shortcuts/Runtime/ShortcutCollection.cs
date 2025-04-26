using System;
using System.Collections;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Santutu.Library.Shortcuts.Runtime
{
    [Serializable]
    public class ShortcutCollection : IEnumerable<Shortcut>
    {
        [field: SerializeField]
#if ODIN_INSPECTOR
        [ListDrawerSettings(ShowFoldout = true, DefaultExpandedState = true, ShowIndexLabels = false)]
#endif
        public List<Shortcut> Items { get; private set; } = new();

        IEnumerator<Shortcut> IEnumerable<Shortcut>.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsInvoked()
        {
            foreach (var shortcut in Items)
            {
                if (shortcut.IsInvoked())
                {
                    return true;
                }
            }

            return false;
        }

        public List<Shortcut>.Enumerator GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}