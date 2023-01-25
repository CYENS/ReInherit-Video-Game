using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit.UI.Modes
{
    public class ModeSelection : MonoBehaviour
    {
        public enum Mode {Build=0, Curate=1, Conserve=2, Storage=3}

        private int m_modeCount;

        [SerializeField]
        private Mode mode = Mode.Build;



        [System.Serializable]
        public class Elements
        {
            [SerializeField]
            private Mode mode;
            
            [SerializeField]
            private GameObject target;

            public void Refresh(Mode current)
            {
                if (target == null) return;
                target.SetActive( current == mode );
            }
        }
        
        [SerializeField] 
        [Tooltip("The UI elements attached to each mode. They must have the same order")]
        private Elements[] modeElements;
        
        

        private void SetMode(bool value, int index)
        {
            // Guard conditions
            if (index < 0) return;
            if (index >= m_modeCount) return;


            
            if (value)  mode = (Mode)index;
            Refresh();
        }

        private void Refresh()
        {
            foreach (var modeElement in modeElements)
                modeElement.Refresh(mode);
        }
    
        public void SetBuildMode(bool value) => SetMode(value, 0);
        public void SetCurateMode(bool value) => SetMode(value, 1);
        public void SetConserveMode(bool value) => SetMode(value, 2);
        public void SetStorageMode(bool value) => SetMode(value, 3);

        private void Awake()
        {
            // Find the number of mode states (Relatively expensive operation, must cache)
            m_modeCount = Enum.GetNames(typeof(Mode)).Length;

            Refresh();
        }

    }
}
