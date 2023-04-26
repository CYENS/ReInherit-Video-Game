using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Patterns;

namespace Cyens.ReInherit.Exhibition
{
    /// <summary>
    /// Makes it easier for external scripts to interact with Exhibit game objects.
    /// - It is able to change the state of exhibits.
    /// - It is able to add/remove exhibits.
    /// - It manages exhibit related events, such as '0 condition' event
    /// </summary>
    public class ExhibitManager : CollectionManager<ExhibitManager,Exhibit>
    {
        
        
    }
}
