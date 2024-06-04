using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor.Build;
using UnityEngine;

namespace Assets.Placements
{
    public class BuildingPlacement : MonoBehaviour
    {
        #region Properties 

        public GameObject PlacementGameObject { get; set; }

        public GameObject HouseGameObject { get; set; }

        public GameObject VesteGameObject { get; set; }

        public Player Owner { get; set; }

        public int Id { get; set; }

        public List<GameObject> NeighbouredStreetPlacements { get; set; }

        public List<GameObject> NeighbouredBuildingPlacements { get; set; }

        #endregion Properties

        public GameObject GetPlacementGameObject()
        {
            return Game.GetGameObjectByName(this.name);
        }

        public GameObject GetHouseGameObject()
        {
            string pattern = @"\d+";

            Match m = Regex.Match(this.name, pattern);

            if (m.Success)
            {
                int number = Int32.Parse(m.Value);

                return Game.GetGameObjectByName($"HouseGameObject{number}");
            }

            return null;
        }

        public GameObject GetVesteGameObject()
        {
            string pattern = @"\d+";

            Match m = Regex.Match(this.name, pattern);

            if (m.Success)
            {
                int number = Int32.Parse(m.Value);

                return Game.GetGameObjectByName($"VesteGameObject{number}");
            }

            return null;
        }

        public void OnPlaneClicked()
        {
            UnityEngine.Debug.Log($"BuildingPlacement {this.name} clicked");

            if (Game.Instance.PlayersHavePlacedTheirFirstTwoHouses == false)
            {
                //If game just started and all players need to place their first given objects

                //Decide what has been placed
                if (Game.Instance.CurrentPlayer.PlacedFirstHouseAlready == false)
                {
                    Game.Instance.CurrentPlayer.PlaceHouse(this.GetPlacementGameObject());

                    Game.Instance.CurrentPlayer.PlacedFirstHouseAlready = true;
                }
                else if (Game.Instance.CurrentPlayer.PlacedSecondHouseAlready == false)
                {
                    Game.Instance.CurrentPlayer.PlaceHouse(this.GetPlacementGameObject());

                    Game.Instance.CurrentPlayer.PlacedSecondHouseAlready = true;

                    Game.Instance.CurrentPlayer.HandoutResourcesAfterPlacingSecondHouse();
                }
            } else
            {
                //Player buys house mid game

                Game.Instance.CurrentPlayer.PlaceHouse(this.GetPlacementGameObject());

                foreach(GameObject building in Game.Instance.AvailableBuildingPlacements)
                {
                    building.SetActive(false);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Überprüfen Sie, ob die linke Maustaste gedrückt wurde
            if (Input.GetMouseButtonDown(0))
            {
                if (Game.Instance.IsSettingsPanelOpen || GiveAwayResources.GiveAwayResourcePanel.activeSelf || Game.Instance.StartGamePanel.activeSelf || Game.Instance.QuickGuidePanel.activeSelf)
                {
                    return;
                }

                // Erstellen Sie einen Raycast von der Mausposition
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Überprüfen Sie, ob der Raycast ein Objekt getroffen hat
                if (Physics.Raycast(ray, out hit))
                {
                    // Überprüfen Sie, ob das getroffene Objekt dieses Plane-Objekt ist
                    if (hit.transform == this.transform)
                    {
                        UnityEngine.Debug.Log("PlacementClicked");
                        // Rufen Sie Ihre Funktion hier auf
                        OnPlaneClicked();
                    }
                }
            }
        }
    }
}
