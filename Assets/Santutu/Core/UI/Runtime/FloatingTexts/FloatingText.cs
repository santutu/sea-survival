using System.Collections;
using TMPro;
using UnityEngine;

namespace Santutu.Modules.UI.Runtime.FloatingTexts
{
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMesh;
        [SerializeField] public float moveSpeed = 2.0f;
        [SerializeField] public float destroyTime = 0.7f;


        private void Start()
        {
            StartCoroutine(FloatingTextRoutine());
        }

        public void SetText(string text)
        {
            textMesh.text = text;
        }

        private IEnumerator FloatingTextRoutine()
        {
            float alpha = 1.0f;
            float elapsedTime = 0f;
            Vector3 startPos = transform.position;

            while (elapsedTime < destroyTime)
            {
                if (!enabled)
                {
                    yield return new WaitUntil(() => enabled);
                }

                transform.position = startPos + WorldCanvas.Instance.transform.up * moveSpeed * elapsedTime;

                alpha = Mathf.Lerp(1f, 0f, elapsedTime / destroyTime);
                textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}