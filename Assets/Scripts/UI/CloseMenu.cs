using UnityEngine;

namespace GD3D
{
    public class CloseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject[] mainMenu;

        private void OnEnable()
        {
            mainMenu = GameObject.FindGameObjectsWithTag("Menu");

            foreach (var menu in mainMenu)
            {
                menu.SetActive(false);
            }
        }

        private void OnDisable()
        {
            foreach (var menu in mainMenu)
            {
                menu.SetActive(true);
            }
        }
    }
}
