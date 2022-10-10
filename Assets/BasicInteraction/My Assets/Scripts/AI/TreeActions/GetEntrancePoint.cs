using System.Collections;
using System.Collections.Generic;
using Cyens.ReInherit;
using UnityEngine;
using TheKiwiCoder;

namespace Cyens.ReInherit
{
    public class GetEntrancePoint : ActionNode
    {
        private EntranceController m_entranceController;
        private Vector3 m_destination;
        private int m_rowId;

        protected override void OnStart()
        {
            m_entranceController = GameObject.Find("Entrance").GetComponent<EntranceController>();
            m_rowId = m_entranceController.GetFreeSpotRowId(context.transform.position);
            m_destination = m_entranceController.GetRowPoint(m_rowId);
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            blackboard.moveToPosition.x = m_destination.x;
            blackboard.moveToPosition.y = m_destination.y;
            blackboard.moveToPosition.z = m_destination.z;

            blackboard.entranceRowId = m_rowId;
            blackboard.waitingForTicket = false;

            return State.Success;
        }
    }
}
