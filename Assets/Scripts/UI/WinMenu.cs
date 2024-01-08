using GD3D.Player;
using GD3D.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class WinMenu : MonoBehaviour
    {
        void Start()
        {
            //PlayerMain.Instance.OnWin += ShowMenu;
        }

        private void ShowMenu()
        { 
            gameObject.SetActive(true);
            //Time.timeScale = 0;
        }
    }
}
