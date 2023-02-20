using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
    public class AnswerScript : MonoBehaviour
    {
        public bool isCorrect = false;
        [SerializeField] QuizManager quizManager;

        private Color StartColor;

        private void Start()
        {
            StartColor = GetComponent<Image>().color;   
        }

        public void Answer()
        {
            if (isCorrect)
            {
                GetComponent<Image>().color = Color.green;
                Debug.Log("Correct Answer");
                quizManager.correct();
            }
            else
            {
                GetComponent<Image>().color = Color.red;
                Debug.Log("Wrong Answer");
                quizManager.wrong();
            }
        }
    }
}
