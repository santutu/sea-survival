﻿using System;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;
using UnityEngine.UI;

namespace sea_survival.Scripts
{
    public class MenuInGameController : SingletonMonoBehaviour<MenuInGameController>
    {
        [SerializeField] private GameObject menu;

        private void Start()
        {
            var btns = menu.GetComponentsInChildren<Button>();

            btns[0].onClick.AddListener(() => { Toggle(); });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Toggle();
            }
        }

        private void Toggle()
        {
            menu.SetActive(!menu.activeSelf);
            Time.timeScale = menu.activeSelf ? 0 : 1;
        }
    }
}