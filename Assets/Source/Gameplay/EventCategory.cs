using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cyens.ReInherit
{
    /// <summary>
    /// A category of events that is displayed at the end of each round.
    /// </summary>
    [CreateAssetMenu(fileName = "EventCategory", menuName = "Data/EventCategory", order = 0)]
    public class EventCategory : ScriptableObject 
    {
        
        public enum Selection { Ordered = 0, Randomized = 1, TwoForwardOneBack = 2 }
        public enum Frequency { Rare = 0, Uncommon = 1, Common = 2, Frequent = 3 }

        public enum Wrapping { Loop = 0, Clamp = 1, End = 2 }

        [TextArea(3,5)]
        [Tooltip("Adds further context to the category")]
        public string comment;


        [Tooltip("Determines how the event is selected by the game")]
        public Selection selection;
        
        [Tooltip(@"How frequently to use this type of event. 
        Rare: Event type is shown once every 3 event groupings
        Uncommon: Event type is shown once every 2 event groups
        Common: Event is shown every event group
        Frequent: Event is shown twice in each event group ")]
        public Frequency frequency = Frequency.Common;

        [Tooltip(@"How to handle when the index runs out of the bounds of the events in the category
        Loop: Loop back at the beginning
        Clamp: Use the final item repeteadly after the end
        End: No new events are given once the end is reached ")]
        public Wrapping wrapping = Wrapping.Clamp;


        [Header("References")]
        public AssetLibrary eventLibrary;
        

        [Header("Runtime")]

        [Tooltip("Events under this category (will be found automatically!)")]
        public List<GameEvent> events;

        public int index;

        public int ticker;

        public int[] indices;

        private int forwardCounter;


        protected void ResetTicker()
        {
            switch( frequency )
            {
                case Frequency.Rare: ticker = Random.Range(0,7); break;
                case Frequency.Uncommon: ticker = Random.Range(0,5); break;
                case Frequency.Common: ticker = Random.Range(0,2); break;
                case Frequency.Frequent: ticker = 0; break;
            }
        }



        public void Init()
        {
            index = 0;
            forwardCounter = 0;
            ResetTicker();

            // Create a sequential index array
            indices = new int[events.Count];
            for(int i=0; i<events.Count; i++ )
            {
                indices[i] = i;
            }

            // Shuffle indices if selection is random
            if( selection == Selection.Randomized )
            {
                // Using Knuth's shuffling algo
                for( int i=0; i<indices.Length; i++ )
                {
                    var temp = indices[i];
                    int r = Random.Range(i, indices.Length);
                    indices[i] = indices[r];
                    indices[r] = temp;
                }
            }
            
        }

        public GameEvent Next() 
        {   

            // Ticker is to ensure we get events
            // at the right frequency
            if( ticker > 0 )
            {
                ticker--;
                return null;
            }
            ResetTicker();

            //Debug.Log("Event: "+this+", Index: "+ index);
            if( index >= indices.Length )
            {
                return null;
            }

            // Grab the next event before we advance the index
            GameEvent nextEvent = events[indices[index]];

            // Advance index
            switch( selection )
            {
                case Selection.Ordered:
                case Selection.Randomized:
                index++;
                break;

                case Selection.TwoForwardOneBack:
                index += ( forwardCounter<2 ? 1 : -1 );
                forwardCounter++;
                if( forwardCounter >= 2 )
                {
                    forwardCounter = 0;
                }
                break;
            }

            // Wrapping modes are taken into account here
            switch( wrapping )
            {
                case Wrapping.Loop:
                    index %= indices.Length;
                break;

                case Wrapping.Clamp:
                    index = Mathf.Clamp( index, 0, indices.Length-1 );
                break;

                case Wrapping.End:
                    // Do nothing
                break;
            }
            
            return nextEvent;
        }


        #if UNITY_EDITOR

        private void Refresh()
        {

            Debug.Log("Refresh");
            events = new List<GameEvent>();

            GameEvent[] allEvents = eventLibrary.GetAssets<GameEvent>();
            foreach( var anEvent in allEvents )
            {
                if( anEvent.category == this )
                {
                    events.Add(anEvent);
                }
            }

        }
        private void Awake() 
        {
            Refresh();
        }

        private void OnValidate()
        {
            Refresh();
        }

        #endif


    }

}
