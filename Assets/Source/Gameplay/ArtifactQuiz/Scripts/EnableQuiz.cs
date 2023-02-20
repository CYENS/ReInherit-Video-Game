using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class EnableQuiz : MonoBehaviour
    {
        public GameObject objectToEnable;
        // Start is called before the first frame update
        void Start()
        {
            objectToEnable.SetActive(false);
            Invoke("EnableObjectAfter5Seconds", 5f);
        }

        private void EnableObjectAfter5Seconds()
        {
            objectToEnable.SetActive(true);
        }
    }
}
