using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyens.ReInherit.Patterns;

namespace Cyens.ReInherit
{
    public class ShopManager : MonoBehaviour
    {

        public ShopItem[] items;

        [Header("References")]
        public GameObject baseUI;

        private void Start()
        {
            items = GetComponentsInChildren<ShopItem>();
            foreach( var item in items )
                item.manager = this;
            

        }

        private void Update()
        {
            baseUI.SetActive(true);
            if (PlacementManager.Instance.active) baseUI.SetActive(false);
        }
    }
}
