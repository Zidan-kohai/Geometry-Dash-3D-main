using GD3D.Easing;
using GD3D.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D
{
    public class StaticLevelColor : MonoBehaviour
    {
        public Color col;

        void Start()
        {
            LevelColors.AddEase(LevelColors.ColorType.background, col);
            LevelColors.AddEase(LevelColors.ColorType.ground, col);
        }
    }
}
