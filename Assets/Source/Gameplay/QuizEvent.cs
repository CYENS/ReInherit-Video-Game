using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class QuizEvent : MonoBehaviour
    {

        public enum State { Quiz = 0, Award = 1, Penalty = 2 }

        [Header("References")]
        [SerializeField] private GameObject m_quizScreen;
        [SerializeField] private GameObject m_awardScreen;
        [SerializeField] private GameObject m_penaltyScreen;
        

        [Header("Variables")]

        [SerializeField] private State m_state;



        // Start is called before the first frame update
        void Start()
        {
        
        }

        public void Correct() => m_state = State.Award;
        public void Wrong() => m_state = State.Penalty;

        public void Remove() => Destroy(gameObject);

        // Update is called once per frame
        void Update()
        {
            m_quizScreen.SetActive( m_state == State.Quiz );
            m_awardScreen.SetActive( m_state == State.Award );
            m_penaltyScreen.SetActive( m_state == State.Penalty );
        }
    }
}
