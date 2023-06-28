using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Exhibition;
using Cyens.ReInherit.Managers;
using UnityEngine.UI;
using UnityEngine.AI;

namespace Cyens.ReInherit
{
    public class ExhibitButton : MonoBehaviour
    {

        public enum Mode { Preview = 0, Remove = 1, Move = 2, Upgrade = 3, Repair = 4, CancelPreview=5 }


        [Header("Parameters")]
        [SerializeField] private Mode m_mode;

        [Header("References")]
        [SerializeField] private ExhibitCase m_case;
        [SerializeField] private Exhibit m_exhibit;

        private KeeperManager m_keeperManager;
        private PlacementManager m_placeManager;

        private Button m_button;

        public void Press() 
        {
            Vector3 entrancePoint = new Vector3(5f, 0f, -5f);
            Vector3 destPoint = m_exhibit.ClosestStandPoint(entrancePoint);
            switch(m_mode)
            {
                case Mode.Preview:
                    Vector3 point = m_case.Placement;
                    PreviewManager.Preview(point, m_exhibit);

                    // TODO: Deselect

                break;

                case Mode.Remove:
                    if (m_keeperManager.CheckPathValidity(entrancePoint, destPoint) == false) {
                        ErrorMessage.Instance.CreateErrorMessage("Cannot remove artifact",
                            "Artifact destination cannot be reached by keeper.");
                    }
                    else {
                        m_exhibit.SetState(Exhibit.State.Transit);
                        m_keeperManager.AddRemoveTask(m_exhibit);   
                    }
                    break;

                case Mode.Move:
                    var metaData = m_exhibit.GetComponent<MetaData>();
                    PlacementManager.PlaceExhibit( m_exhibit.gameObject, metaData.mesh );
                    break;

                case Mode.Upgrade:
                    bool canUpgrade = m_exhibit.GetUpgrade() == 0;
                    int cost = m_exhibit.Info.upgradeCost;
                    bool hasBudget = GameManager.Funds > cost;
                    if( canUpgrade == false )
                    {
                        return;
                    }
                    if( hasBudget == false )
                    {
                        ErrorMessage.Instance.CreateErrorMessage("Cannot upgrade artifact", "Not enough funds to afford it.");
                        return;
                    }
                    if (m_keeperManager.CheckPathValidity(entrancePoint, destPoint) == false) {
                        ErrorMessage.Instance.CreateErrorMessage("Cannot upgrade artifact",
                            "Artifact destination cannot be reached by keeper.");
                    }
                    else {
                        GameManager.Funds -= cost;
                        m_exhibit.SetState(Exhibit.State.Transit);
                        m_exhibit.Upgrade();
                        m_keeperManager.AddUpgradeTask(m_exhibit);
                    }
                    break;

                case Mode.Repair:

                break;

                case Mode.CancelPreview:
                    PreviewManager.Cancel();
                break;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if( m_case == null )
            {
                m_case = GetComponent<ExhibitCase>();
            }
            if( m_case == null )
            {
                m_case = GetComponentInParent<ExhibitCase>();
            }
            if( m_case == null )
            {
                m_case = GetComponentInChildren<ExhibitCase>();
            }

            m_keeperManager = KeeperManager.Instance;

            m_placeManager = PlacementManager.Instance;
            
            if(m_case != null)
                m_exhibit = m_case.GetComponentInParent<Exhibit>();

            m_button = GetComponent<Button>();
        }

        private void Update() {
            
            // Remove upgrade option if
            if( m_mode == Mode.Upgrade )
            {
                bool canUpgrade = m_exhibit.GetUpgrade() == 0;
                m_button.interactable = canUpgrade;
            }
        }
    }
}
