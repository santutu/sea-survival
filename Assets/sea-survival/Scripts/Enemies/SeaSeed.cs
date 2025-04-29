using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using sea_survival.Scripts.Players;

namespace sea_survival.Assets.sea_survival.Scripts.Enemies
{
    public class SeaSeed : MonoBehaviour
    {
        [SerializeField] private float speedReductionMultiplier = 0.5f; // 이동 속도 감소 배율
        private Player player => Player.Ins;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && player != null)
            {
                player.speedmultiplyModifier *= speedReductionMultiplier;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && player != null)
            {
                player.speedmultiplyModifier /= speedReductionMultiplier;
            }
        }
    }
}