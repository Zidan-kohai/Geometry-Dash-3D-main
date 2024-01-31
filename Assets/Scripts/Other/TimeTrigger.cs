using GD3D.Player;
using System.Collections;
using UnityEngine;

namespace GD3D
{
    [RequireComponent(typeof(BoxCollider))]
    public class TimeTrigger : MonoBehaviour
    {
        [Range(0, 1)]
        [SerializeField] private float slowdown;


        [Range(1, 10)]
        [SerializeField] private float normalSpeed = 1;

        [Range(0.01f, 10)]
        [SerializeField] private float acceleration;


        [SerializeField] private float currentSpeed = 1;

        private void Start ()
        {
            PlayerMain.Instance.OnDeath += (()=>
            {
                StopAllCoroutines();
                currentSpeed = 1;
                Time.timeScale = currentSpeed;
            });
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                StopAllCoroutines();
                StartCoroutine(DegreaseSpeed());
            }
        }



        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                StopAllCoroutines();
                StartCoroutine(IncreaseSpeed());
            }
        }

        private IEnumerator IncreaseSpeed()
        {
            while(currentSpeed < normalSpeed)
            {
                yield return new WaitForSeconds(0.01f);
                currentSpeed += acceleration;
                Time.timeScale = currentSpeed;
            }
        }

        private IEnumerator DegreaseSpeed()
        {
            while (currentSpeed > slowdown)
            {
                yield return new WaitForSeconds(0.01f);
                currentSpeed -= acceleration;
                Time.timeScale = currentSpeed;
            }
        }
    }
}
