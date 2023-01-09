using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Cyens.ReInherit.Gameplay.Management;

namespace Cyens.ReInherit
{
    public class ShopItem : MonoBehaviour
    {

        public ShopManager manager;

        public int stock = 0;
        public int price = 100;

        [Header("References")]
        public TMP_Text priceTag;


        // Start is called before the first frame update
        void Start()
        {

            Refresh();
        }

        public void Refresh()
        {
            if (stock > 0) priceTag.text = stock.ToString();
            else priceTag.text = "€" + price.ToString();
        }

        public void Purchase()
        {
            if (stock > 0)
            {
                stock--;

                Refresh();
                return;
            }

            // Check if player has enough funds to buy the object
            int funds = GameManager.GetFunds();
            if( funds < price )
            {
                // TODO: play some sound effect, or play an animation to indicate failed purchase
                return;
            }
            funds -= price;
            stock++;
            GameManager.SetFunds(funds);

            // TODO: Placement time!

            //if( stock <= 0 )
            Refresh();
        }


        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
