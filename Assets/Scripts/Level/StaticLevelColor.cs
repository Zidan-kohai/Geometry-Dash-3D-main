using GD3D.Level;
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
