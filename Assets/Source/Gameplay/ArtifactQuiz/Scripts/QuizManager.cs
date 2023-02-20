using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyens.ReInherit
{
    public class QuizManager : MonoBehaviour
    {
        [SerializeField] List<QuestionsAndAnswers> QnA;
        [SerializeField] GameObject[] options;
        [SerializeField] int currentQuestion;

        [SerializeField] GameObject QuizPanel;
        [SerializeField] GameObject ClosePanel;
        [SerializeField] GameObject BGPanel;
        [SerializeField] GameObject BehindCanvas;

        [SerializeField] Text QuestionTxt;
        [SerializeField] Text CloseTitle;
        [SerializeField] GameObject CloseImage;

        private void Start()
        {
            ClosePanel.SetActive(false);
            generateQuestion();
        }

        private void close()
        {
            QuizPanel.SetActive(false);
            ClosePanel.SetActive(false);
            BGPanel.SetActive(false);
            BehindCanvas.SetActive(false);
        }

        private void GameOver()
        {
            QuizPanel.SetActive(false);
            ClosePanel.SetActive(true);
        }

        public void correct()
        {
            CloseTitle.text = "You Won This Artifact";
            CloseImage.GetComponent<Image>().sprite = QnA[currentQuestion].Answers[QnA[currentQuestion].CorrectAnswer - 1];
            QnA.RemoveAt(currentQuestion);
            generateQuestion();
        }

        public void wrong()
        {
            CloseTitle.text = "You Lost This Artifact";
            CloseImage.GetComponent<Image>().sprite = QnA[currentQuestion].Answers[QnA[currentQuestion].CorrectAnswer - 1];
            QnA.RemoveAt(currentQuestion);
            generateQuestion();
        }



        private void SetAnswers()
        {
            for (int i = 0; i < options.Length; i++)
            {
                options[i].GetComponent<AnswerScript>().isCorrect = false;
                options[i].transform.GetChild(0).GetComponent<Image>().sprite = QnA[currentQuestion].Answers[i];

                if(QnA[currentQuestion].CorrectAnswer == i + 1)
                {
                    options[i].GetComponent<AnswerScript>().isCorrect = true;
                }
            }
        }

        private void generateQuestion()
        {
            if(QnA.Count > 0)
            {
                currentQuestion = Random.Range(0, QnA.Count);

                QuestionTxt.text = QnA[currentQuestion].Question;
                SetAnswers();
            }
            else
            {
                //Debug.Log("Out of Questions");
                Invoke("GameOver", 2f);
            }
            

        }
    }
}
