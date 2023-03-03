using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cyens.ReInherit
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Object SceneName;
        [SerializeField] Animator transitionAnimator;
        [SerializeField] float transitionTime = 1f;

        public void PlayGame()
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex+1));
        }

        public void QuitGame()
        {
            Debug.Log("QUIT");
            Application.Quit();
        }

        IEnumerator LoadLevel(int levelIndex)
        {
            transitionAnimator.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            SceneManager.LoadScene(levelIndex);
        }

    }
}
