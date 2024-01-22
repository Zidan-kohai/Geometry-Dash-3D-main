using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GD3D
{
    public class StaticLocalization : MonoBehaviour
    {

        public List<Localization> Texts = new List<Localization>();


        public void Start()
        {
            if(Geekplay.Instance.language == "ru")
            {
                foreach (var item in Texts)
                {
                    item.text.text = item.ru;
                }
            }
            else if (Geekplay.Instance.language == "en")
            {
                foreach (var item in Texts)
                {
                    item.text.text = item.en;
                }
            }
            else if (Geekplay.Instance.language == "tu")
            {
                foreach (var item in Texts)
                {
                    item.text.text = item.tu;
                }
            }
        }
    }

    [Serializable]
    public class Localization
    {
        public TextMeshProUGUI text;
        public string ru;
        public string en;
        public string tu;
    }
}
