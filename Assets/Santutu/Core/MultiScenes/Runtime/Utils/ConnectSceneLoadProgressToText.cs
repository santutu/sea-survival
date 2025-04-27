using System;
using R3;
using TMPro;
using UnityEngine;

namespace Santutu.Modules.MultiScenes.Runtime.Utils
{
    public class ConnectSceneLoadProgressToText : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private void Start()
        {
            text.text = "0";
            MultiSceneManager.OnSceneLoadProgressChanged.Subscribe(v => { text.text = ((int)(v * 100)).ToString(); })
                             .AddTo(this);
        }
    }
}