using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class LoseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject reviveButton;
        [SerializeField] private GameObject restartButton;
        [SerializeField] private GameObject ExitButton;


        private void OnEnable()
        {
            reviveButton.transform.localScale = Vector3.zero;
            restartButton.transform.localScale = Vector3.zero;
            ExitButton.transform.localScale = Vector3.zero;

            StartCoroutine(ShowMenu());
        }
        private IEnumerator ShowMenu()
        {
            StartCoroutine(ShowButton(reviveButton));

            yield return new WaitForSeconds(3);

            StartCoroutine(ShowButton(restartButton));
            StartCoroutine(ShowButton(ExitButton));
        }

        private IEnumerator ShowButton(GameObject gameObject)
        {
            float time = 1f;
            float deltatime = 0.02f;
            float delta = 0.02f;

            while (time > 0)
            {
                gameObject.transform.localScale += new Vector3(delta, delta, delta);

                yield return new WaitForSeconds(deltatime);
                time -= deltatime;
            }
        }

        //private void OnEnable()
        //{
        //    reviveButton.SetActive(false);
        //    restartButton.SetActive(false); 
        //    ExitButton.SetActive(false);

        //    StartCoroutine(ShowMenu());
        //}
        //private IEnumerator ShowMenu()
        //{
        //    yield return new WaitForSeconds(0.2f);

        //    reviveButton.SetActive (true);

        //    yield return new WaitForSeconds(3);

        //    restartButton.SetActive(true);
        //    ExitButton.SetActive(true);
        //}
    }
}
