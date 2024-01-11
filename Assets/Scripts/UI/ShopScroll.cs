using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class ShopScroll : MonoBehaviour
    {
        [SerializeField] private float deltaX;
        [SerializeField] private float timeToEndAnimation;
        [SerializeField] private GameObject content;
        [SerializeField] private int scrollCount;



        private float animationVelocity;
        private int currentScrollIndex = 1;
        private bool isScrolling = false;
        private void Start ()
        {
            animationVelocity = deltaX / timeToEndAnimation;
        }
        public void ScrollRight()
        {
            if (currentScrollIndex >= scrollCount || isScrolling) return;

            StartCoroutine(Scroll(-1));
            currentScrollIndex++;
        }

        public void ScrollLeft()
        {
            if (currentScrollIndex <= 1 || isScrolling) return;

            StartCoroutine(Scroll(1));

            currentScrollIndex--;
        }

        private IEnumerator Scroll(int direction)
        {
            float spentTime = 0f;
            isScrolling = true;

            while (spentTime <= timeToEndAnimation)
            {
                content.transform.localPosition = new Vector3(content.transform.localPosition.x + (animationVelocity * 0.02f * direction), content.transform.localPosition.y, content.transform.localPosition.z);
                yield return new WaitForSeconds(0.02f);
                Debug.Log(content.transform.localPosition);
                spentTime += 0.02f;
            }

            isScrolling = false;
        }
    }
}
