using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Patterns;
using UnityEngine;
using TMPro;

namespace Cyens.ReInherit.Gameplay.Management
{
    /// <summary>
    /// Stores the state of the game.
    /// Keeps track of various metrics.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {

        public enum MuseumState { Close = 0, Open = 1 }

        [SerializeField]
        [Tooltip("Whether the museum is currently open or closed")]
        private MuseumState m_museumState = MuseumState.Close;


        public int funds = 1000;

        [Header("References")]
        public TMP_Text txtFunds;

        public static int GetFunds() => Instance.funds;
        public static void SetFunds(int amount)
        {
            Instance.funds = amount;
            Instance.Refresh();
        }




        public void Refresh()
        {
            txtFunds.text = "€" + funds.ToString();
        }

        public override void Awake()
        {
            base.Awake();
            
        }
    }
}
