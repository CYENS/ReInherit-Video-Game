using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {

    // This is the blackboard container shared between all nodes.
    // Use this to store temporary data that multiple nodes need read and write access to.
    // Add other properties here that make sense for your specific use case.
    [System.Serializable]
    public class Blackboard {
        public Vector3 moveToPosition;
        public GameObject garbage;
        public GameObject exhibit;
        public int entranceRowId;
        public bool waitingForTicket; //if agent wait to buy a ticket
        public bool inTicketRow; //if agent entered a ticket queue
        public bool moveToExit = false; //if agent is ready to leave
        public List<GameObject> rooms; //all rooms
        public List<int> visitedRooms; //ObjectIDs of visited rooms
        public GameObject currentRoom; //current room the agent is in
        public GameObject nextRoom; //next room agent will visit
        public bool exhibitInitial = true; //If if agent enters first room with exhibits
        public int exhibitIndex; //Index of exhibit that agent visits 
        public int exhibitPosIndex; //Index of position around exhibit that agent visit
        public int exhibitsVisited = 0; //Number of exhibits visited in current room
        public bool firstExhibit = true; //If agent just entered a room and will find first exhibit
        public bool waitSeeExhibit = false; //if agent will wait in font of exhibits
        
        public List<Variable> variables;

        public int FindVariable(string key)
        {
            for( int i=0; i<variables.Count; i++ )
            {
                Variable variable = variables[i];
                if( variable.name.Equals(key) ) 
                    return i;
            }
            return -1;
        }

        public bool HasVariable(string key) => FindVariable(key) >= 0;

        public float GetValue(string key)
        {
            int index = FindVariable(key);
            if( index < 0 ) return float.NaN;
            return variables[index].value;
        }

        public void SetValue(string key, float newValue)
        {
            int index = FindVariable(key);
            if( index < 0 ) return;
            Variable variable = variables[index];
            variable.value = newValue;
            variables[index] = variable;
        }

    }

    // Variables that can be used by the agent at runtime
    [System.Serializable]
    public struct Variable
    {
        public string name;
        public float value;
    }
}