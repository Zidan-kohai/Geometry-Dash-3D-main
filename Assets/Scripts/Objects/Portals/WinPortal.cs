using GD3D.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class WinPortal : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            PlayerMain.Instance.OnWin?.Invoke();
        }
    }
}
