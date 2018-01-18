using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby
{
    public class LobbyTopPanel : MonoBehaviour
    {
        public bool isInGame = false;

        protected bool isDisplayed = true;
        protected Image panelImage;

		public Button quitButton;

        void Start()
        {
            panelImage = GetComponent<Image>();
        }


        void Update()
        {
            if (!isInGame)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleVisibility(!isDisplayed);
				quitButton.gameObject.SetActive (false);

				Cursor.visible = isDisplayed;
				if(isDisplayed)
					Cursor.lockState = CursorLockMode.Confined;
				else
					Cursor.lockState = CursorLockMode.Locked;
            }

        }

        public void ToggleVisibility(bool visible)
        {
            isDisplayed = visible;
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(isDisplayed);
            }

            if (panelImage != null)
            {
                panelImage.enabled = isDisplayed;
            }
        }
    }
}