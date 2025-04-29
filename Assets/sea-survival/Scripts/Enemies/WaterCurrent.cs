using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using sea_survival.Scripts.Players;

namespace sea_survival.Assets.sea_survival.Scripts.Enemies
{
    public class WaterCurrent : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        private void Update()
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
    }
}