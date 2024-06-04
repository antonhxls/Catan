using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Placements
{
    public class StreetPlacement : MonoBehaviour
    {
        #region Properties 

        public GameObject PlacementGameObject { get; set; }

        public GameObject StreetGameObject { get; set; }

        public Player Owner { get; set; }

        public int Id { get; set; }

        public List<GameObject> NeighbouredStreetPlacements { get; set; }

        public List<GameObject> NeighbouredBuildingPlacements { get; set; }

        #endregion Properties

        public GameObject GetPlacementGameObject()
        {
            return Game.GetGameObjectByName(this.name);
        }

        public GameObject GetStreetGameObject()
        {
            string pattern = @"\d+";

            Match m = Regex.Match(this.name, pattern);

            if (m.Success)
            {
                int number = Int32.Parse(m.Value);

                return Game.GetGameObjectByName($"StreetGameObject{number}");
            }

            return null;
        }

        public void OnPlaneClicked()
        {
            UnityEngine.Debug.Log($"StreetPlacement {this.name} clicked");

            if (Game.Instance.PlayersHavePlacedTheirFirstTwoHouses == false)
            {
                //If game just started and all players need to place their first given objects

                //Decide what has been placed
                if (Game.Instance.CurrentPlayer.PlacedFirstStreetAlready == false)
                {
                    Game.Instance.CurrentPlayer.PlaceStreet(this.GetPlacementGameObject());

                    Game.Instance.CurrentPlayer.PlacedFirstStreetAlready = true;

                    //Only let players build second building and street when all palyers have placed their first building and street
                    if (Game.Instance.AllPlayers.Where(x => x.PlacedFirstStreetAlready == false).Count() == 0)
                    {
                        Game.Instance.LetPlayersPlaceSecondHouseAndStreet();
                    }
                }
                else if (Game.Instance.CurrentPlayer.PlacedSecondStreetAlready == false)
                {
                    Game.Instance.CurrentPlayer.PlaceStreet(this.GetPlacementGameObject());

                    this.GetPlacementGameObject().SetActive(false);

                    Game.Instance.CurrentPlayer.PlacedSecondStreetAlready = true;
                }

            }
            else
            {
                // Player buys a street mid game
                Game.Instance.CurrentPlayer.PlaceStreet(this.GetPlacementGameObject());

                foreach (GameObject street in Game.Instance.AvailableStreetPlacements)
                {
                    street.SetActive(false);
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
