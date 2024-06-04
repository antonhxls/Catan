using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets    
{
    public class Robber
    {

        #region Properties

        public static bool IsInitialized { get; set; }

        public const float YPOS = 0.377f;

        public GameObject GameObject { get; set; }

        public bool NeedsToBeMovedByPlayer { get; set; }

        #endregion Properties

        #region Methods

        public static Robber CreateNewRobber(GameObject robberGO)
        {
            Robber newRobber = new()
            {
                GameObject = robberGO,
            };

            newRobber.PlaceRobberOnDesert();

            return newRobber;
        }

        /// <summary>
        /// Places the robber on the desert
        /// </summary>
        public void PlaceRobberOnDesert()
        {
            //Get desert field (only one desert --> get first field with fieldtype desert)
            Field desertField = Game.Instance.AllFields.Where(x => x.FieldType == FieldType.Desert).First();

            desertField.IsRobberPlacedOnThisField = true;

            this.GameObject.transform.position = new Vector3(desertField.GameObject.transform.position.x - 0.5f * desertField.GameObject.transform.localScale.x, 0.3584f, desertField.GameObject.transform.position.z);
        }

        #endregion Methods
    }
}
