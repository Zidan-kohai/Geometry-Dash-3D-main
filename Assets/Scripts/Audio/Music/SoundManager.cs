using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        public AudioSource mainAudio;

        [Header("Death")]
        public AudioSource DeathClip;

        [Header("Win")]
        public AudioSource WinClip;

        [Header("Button Click")]
        public AudioSource buttonClickClip;

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


        public void StopMainAudio()
        {
            mainAudio.Pause();
        }

        public void PlayMainAudio()
        {
            mainAudio.UnPause();
        }

        public void PlayButtonClickAudio()
        {
            buttonClickClip.Play();
        }
    }
}
