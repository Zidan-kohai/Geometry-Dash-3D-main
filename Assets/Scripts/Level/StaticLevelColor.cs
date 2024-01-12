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
            LevelColors.AddEase(LevelColors.ColorType.background, col, new EaseObject(0.3f));
            LevelColors.AddEase(LevelColors.ColorType.ground, col, new EaseObject(0.3f));
        }
    }
}
