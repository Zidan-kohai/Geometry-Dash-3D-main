using System.Collections;
using UnityEngine;

namespace GD3D
{
    public class LoseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject reviveButton;
        [SerializeField] private GameObject restartButton;
        [SerializeField] private GameObject ExitButton;

        public static int loseCount = 0;
        private void OnEnable()
        {
            restartButton.transform.localScale = Vector3.one;
            ExitButton.transform.localScale = Vector3.one;

            if (loseCount != 0) return;

            reviveButton.transform.localScale = Vector3.zero;
            restartButton.transform.localScale = Vector3.zero;
            ExitButton.transform.localScale = Vector3.zero;

            StartCoroutine(ShowMenu());
        }
        private IEnumerator ShowMenu()
        {
            StartCoroutine(ShowButton(reviveButton));

            yield return new WaitForSeconds(1);

            StartCoroutine(ShowButton(restartButton));
            StartCoroutine(ShowButton(ExitButton));
        }

        private IEnumerator ShowButton(GameObject gameObject)
        {
            float time = 0.5f;
            float deltatime = 0.01f;
            float delta = 0.02f;

            while (time > 0)
            {
                gameObject.transform.localScale += new Vector3(delta, delta, delta);

                yield return new WaitForSeconds(deltatime);
                time -= deltatime;
            }
        }
    }
}
