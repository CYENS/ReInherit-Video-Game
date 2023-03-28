using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Patterns;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class CoinManager : Singleton<CoinManager>
    {
        [Header ("UI references")]
        [SerializeField] private GameObject m_coinPrefab;
        [SerializeField] private GameObject m_target;
        
        [SerializeField] private int m_maxCoins;
        private Queue<GameObject> coinsQueue = new Queue<GameObject> ();
        
        private Camera m_mainCamera;

        void Awake ()
        {
            PrepareCoins();
            m_mainCamera = Camera.main;
        }

        void PrepareCoins ()
        { 
            for (int i = 0; i < m_maxCoins; i++) {
                GameObject coin = Instantiate(m_coinPrefab, transform);
                coin.SetActive(false);
                coinsQueue.Enqueue(coin);
            }
        }

        IEnumerator Animate (Vector3 coinStartPos, int amount)
        {
            for (int i = 0; i < amount; i++) {
                //check if there's coins in the pool
                if (coinsQueue.Count > 0) {
                    //extract a coin from the pool
                    GameObject coin = coinsQueue.Dequeue();
                    coin.transform.position = coinStartPos;
                    coin.transform.rotation = Quaternion.LookRotation(m_mainCamera.transform.position) * Quaternion.Euler(90, 0, 0);
                    coin.SetActive(true);
                    
                    Vector3 screenPoint = m_target.transform.position + new Vector3(0, 0, 8f);
                    Vector3 targetWorldPos = m_mainCamera.ScreenToWorldPoint(screenPoint) + new Vector3(-0.6f, -1f, 0);
                    coin.GetComponent<Coin>().AnimateCoin(targetWorldPos);
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void CoinOnDestination(GameObject coin)
        {
            m_target.GetComponent<BudgetBox>().CoinCollected();
            coin.SetActive(false);
            coinsQueue.Enqueue(coin);
        }

        public void AddCoins (Vector3 coinPos, int amount)
        {
            StartCoroutine(Animate(coinPos, amount));
        }
    }
}
