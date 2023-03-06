using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class PauseMenu : MonoBehaviour
    {
        public static bool GameIsPaused = false;
        [SerializeField] GameObject pauseMenuUI;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if(GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
            
        }

        public void Resume()
        {
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            GameIsPaused = false;
        }

        private void Pause()
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            GameIsPaused = true;
        }

    }
}
