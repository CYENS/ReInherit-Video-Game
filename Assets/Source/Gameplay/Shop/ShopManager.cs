using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Patterns;

namespace Cyens.ReInherit
{
    public class ShopManager : MonoBehaviour
    {

        public ShopItem[] items;

        private void Start()
        {
            items = GetComponentsInChildren<ShopItem>();
            foreach( var item in items )
                item.manager = this;
            

        }


    }
}
