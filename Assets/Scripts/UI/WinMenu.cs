using GD3D.Player;
using GD3D.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class WinMenu : MonoBehaviour
    {

        [SerializeField] private GameObject doubleAwardButton;
        [SerializeField] private GameObject restartButton;
        [SerializeField] private GameObject ExitButton;

        private void OnEnable()
        {
            doubleAwardButton.transform.localScale = Vector3.zero;
            restartButton.transform.localScale = Vector3.zero;
            ExitButton.transform.localScale = Vector3.zero;

            StartCoroutine(ShowMenu());
        }

        private IEnumerator ShowMenu()
        {
            StartCoroutine(ShowButton(doubleAwardButton));

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
