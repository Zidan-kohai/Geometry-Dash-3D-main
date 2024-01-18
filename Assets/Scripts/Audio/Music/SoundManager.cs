using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }


        [Header("Death")]
        public AudioSource DeathClip;

        [Header("Win")]
        public AudioSource WinClip;
        

        private void Awake()
        {
            Instance = this;
        }
        public void PlayDeathClip()
        {
            DeathClip.Play();
        }

        public void PlayWinClip()
        {
            WinClip.Play();
        }

    }
}