using System;
using UnityEngine;

namespace sea_survival.Scripts
{
    public class RepositionTileMap : MonoBehaviour
    {
        [SerializeField] private Grid grid;

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Area"))
            {
                return;
            }

            Vector3 playerPos = Player.Ins.transform.position;
            Vector3 myPos = transform.position;

            float diffX = Mathf.Abs(playerPos.x - myPos.x);
            float diffY = Mathf.Abs(playerPos.y - myPos.y);

            Vector3 playerDir = Player.Ins.InputVec;
            float dirX = playerDir.x < 0 ? -1 : 1;
            float dirY = playerDir.y < 0 ? -1 : 1;

            if (diffX > diffY)
            {
                transform.Translate(Vector3.right * dirX * 60 * grid.transform.localScale.x);
            }
            else if (diffX < diffY)
            {
                transform.Translate(Vector3.up * dirY * 60 * grid.transform.localScale.x);
            }
            else
            {
                transform.Translate(Vector3.up * dirY * 60 * grid.transform.localScale.x);
                transform.Translate(Vector3.right * dirX * 60 * grid.transform.localScale.x);
            }
        }
    }
}