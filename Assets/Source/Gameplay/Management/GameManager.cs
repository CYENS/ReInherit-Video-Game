using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit.Patterns;
using UnityEngine;

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

        
        
        

        public override void Awake()
        {
            base.Awake();
            
        }
    }
}
