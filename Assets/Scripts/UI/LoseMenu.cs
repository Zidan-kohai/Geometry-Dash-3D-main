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
            //reviveButton.transform.localScale = Vector3.zero;
            //restartButton.transform.localScale = Vector3.zero;
            //ExitButton.transform.localScale = Vector3.zero;

            //ShowMenu();
        }
        //private IEnumerator ShowMenu()
        //{
        //    ShowButton(reviveButton);

        //    yield return new WaitForSeconds(3);

        //    ShowButton(restartButton);
        //    ShowButton(ExitButton);
        //}

        //private IEnumerator ShowButton(GameObject gameObject)
        //{
        //    float time = 1f;
        //    float deltatime = 0.1f;
        //    float delta = 0.1f;

        //    while (time > 0)
        //    {
        //        gameObject.transform.localScale += new Vector3(delta, delta, delta);

        //        yield return new WaitForSeconds(deltatime);
        //        time -= deltatime;
        //    }
        //}
    }
}
