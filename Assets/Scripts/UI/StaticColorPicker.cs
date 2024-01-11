using GD3D.UI;
using PlasticGui.WorkspaceWindow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace GD3D
{
    public class StaticColorPicker : MonoBehaviour
    {
        private Button button;
        private bool isSelect = false;

        [Tooltip("Get Color from image")]
        [SerializeField] private Image image;
        [SerializeField] private WhitchColor whitchColorChange;
        [Space]
        [SerializeField] private IconKit iconKit;

        private void Awake()
        {
            button = GetComponent<Button>();
            image = GetComponent<Image>();
        }

        private void Start()
        {
            button.onClick.AddListener(ChangePlayerColor);
        }

        private void ChangePlayerColor()
        {
            if (isSelect) return;

            if (whitchColorChange == WhitchColor.Color1)
            {
                iconKit.UpdatePlayerColor1(image.color);
            }
            else
            {
                iconKit.UpdatePlayerColor2(image.color);
            }
        }


        public enum WhitchColor
        {
            Color1,
            Color2
        }
    }
}
