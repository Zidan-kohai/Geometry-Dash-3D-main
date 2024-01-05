using GD3D.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class LoseMenu : MonoBehaviour
    {
        void Start()
        {
            PlayerMain.Instance.OnDeath += ShowMenu;
        }

        private void ShowMenu()
        {
            transform.gameObject.SetActive(true);
        }
    }
}
