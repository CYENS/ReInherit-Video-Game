using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace Cyens.ReInherit
{
    public class LoadSceneDropdown : MonoBehaviour
    {
        // Define the list to hold file names
        private List<string> fileNames = new List<string>();

        // Define the directory to fetch file names from
        private string directoryPath = "Assets/Scenes/LoadScenes";

        private TMP_Dropdown dropdown;

        // Start is called before the first frame update
        void Start()
        {
            GetFileNames();

            dropdown = transform.GetComponent<TMP_Dropdown>();
            dropdown.options.Clear();

            foreach (var scene in fileNames)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData() {text = scene});
            }

            
        }

        private void GetFileNames()
        {
            // Check if the directory exists
            if (Directory.Exists(directoryPath))
            {
                // Get all file names from the directory
                string[] filesInDirectory = Directory.GetFiles(directoryPath, "*.unity");

                // Add each file name to the list
                foreach (string file in filesInDirectory)
                {
                    fileNames.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
            else
            {
                Debug.LogError("Directory does not exist: " + directoryPath);
            }
        }

        public void LoadTheScene()
        {
            // Get the index of the selected option
            int selectedIndex = dropdown.value;

            // Get the text of the selected option
            SceneManager.LoadScene(dropdown.options[selectedIndex].text);
        }
    }
}
