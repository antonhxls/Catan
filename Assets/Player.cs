using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Assets
{
    public class Player
    {

        #region Properties

        public Material Material { get; set; }

        public int VictoryPoints { get; set; }

        public bool IsAIPlayer { get; set; }

        public bool PlacedFirstHouseAlready { get; set; }

        public bool PlacedFirstStreetAlready { get; set; }

        public bool PlacedSecondHouseAlready { get; set; }

        public bool PlacedSecondStreetAlready { get; set; }

        /// <summary>
        /// Contains all of the players resources and their amounts
        /// </summary>
        public Dictionary<FieldType, int> Resources { get; set; }

        public List<GameObject> OwnedBuildings { get; set; }

        public List<GameObject> OwnedStreets { get; set; }

        #endregion Properties

        #region Methods

        public async Task MakeMove()
        {
            if (this.IsAIPlayer == true)
            {
                Game.Instance.ShowInfoMessage("AI is playing", 1000);
                Game.HideUIButtons();

                await Task.Delay(1000);

                //Upgrade to Veste if possible
                while (this.CanBuyVeste().Item1 == true)
                {
                    UnityEngine.Debug.Log("AI tries to buy veste");

                    List<GameObject> list = this.CanBuyVeste().Item2;

                    HouseGameObject.HouseToUpgrade = list[new System.Random().Next(0, list.Count)];

                    HouseGameObject.UpgradeToVeste();
                }

                await Task.Delay(1000);

                //Buy new Settlement if possible
                while (this.CanBuySettlement().Item1 == true)
                {
                    UnityEngine.Debug.Log("AI tries to buy settlement");

                    List<GameObject> list = this.CanBuySettlement().Item2;

                    this.PlaceHouse(list[new System.Random().Next(0, list.Count)]);
                }

                await Task.Delay(1000);

                while (this.CanBuyStreet().Item1 == true)
                {
                    UnityEngine.Debug.Log("AI tries to buy street");

                    List<GameObject> list = this.CanBuyStreet().Item2;

                    this.PlaceStreet(list[new System.Random().Next(0, list.Count)]);
                }

                await Task.Delay(1000);

                Game.Instance.NextTurn();
            }
            else
            {
                UnityEngine.Debug.Log("You are playing...");
                Game.ShowUIButtons();
            }
        }

        public void AIPlaceFirstBuildingAndStreet()
        {
            //Get random available placement
            GameObject randomBuildingPlacementGameObject = Game.Instance.AvailableBuildingPlacements[new System.Random().Next(0, Game.Instance.AvailableBuildingPlacements.Count())];

            //Place house on random placement
            this.PlaceHouse(randomBuildingPlacementGameObject);

            this.PlacedFirstHouseAlready = true;

            //Get random neighboured street from first placed building
            List<GameObject> neighbouredStreets = Game.GetNeighbouredStreetGOsOfBuilding(Game.GetNumberFromString(randomBuildingPlacementGameObject.name));

            GameObject randomNeighbouredStreetGameObject = neighbouredStreets[new System.Random().Next(0, neighbouredStreets.Count())];

            //place street
            this.PlaceStreet(randomNeighbouredStreetGameObject);

            this.PlacedFirstStreetAlready = true;

            //Only let players build second building and street when all palyers have placed their first building and street
            if (Game.Instance.AllPlayers.Where(x => x.PlacedFirstStreetAlready == false).Count() == 0)
            {
                Game.Instance.LetPlayersPlaceSecondHouseAndStreet();
            }
        }

        public void AIPlaceSecondBuildingAndStreet()
        {
            //Get random available placement
            GameObject randomBuildingPlacementGameObject = Game.Instance.AvailableBuildingPlacements[new System.Random().Next(0, Game.Instance.AvailableBuildingPlacements.Count())];

            //Place house on random placement
            this.PlaceHouse(randomBuildingPlacementGameObject);

            this.PlacedSecondHouseAlready = true;

            //Get random neighboured street from first placed building
            List<GameObject> neighbouredStreets = Game.GetNeighbouredStreetGOsOfBuilding(Game.GetNumberFromString(randomBuildingPlacementGameObject.name));

            GameObject randomNeighbouredStreetGameObject = neighbouredStreets[new System.Random().Next(0, neighbouredStreets.Count())];

            //place street
            this.PlaceStreet(randomNeighbouredStreetGameObject);

            this.PlacedSecondStreetAlready = true;

            this.HandoutResourcesAfterPlacingSecondHouse();
        }

        public void PlaceHouse(GameObject placementGameObject)
        {
            //Get building that has to be placed on placementGameObject
            GameObject placedBuilding = Game.GetGameObjectByName($"HouseGameObject{Game.GetNumberFromString(placementGameObject.name)}");

            //Place building position to the position of the clicked palcement
            placedBuilding.transform.position = placementGameObject.transform.position;

            //Make placed building visible
            placedBuilding.SetActive(true);

            //Colorize placed building
            placedBuilding.GetComponent<MeshRenderer>().material = this.Material;

            //Add placed building to this players owndebuildings
            this.OwnedBuildings.Add(placedBuilding);

            //Remove resources
            if (Game.Instance.PlayersHavePlacedTheirFirstTwoHouses == true)
            {
                Game.Instance.CurrentPlayer.Resources[FieldType.Wood]--;
                Game.Instance.CurrentPlayer.Resources[FieldType.Brick]--;
                Game.Instance.CurrentPlayer.Resources[FieldType.Wheat]--;
                Game.Instance.CurrentPlayer.Resources[FieldType.Sheep]--;
                this.RefreshDisplayedResourceAmount();
            }

            this.IncreaseVictoryPoints();

            //Refresh buidling properties for Field objects
            foreach (Field field in Game.Instance.AllFields)
            {
                if (field.TopBuildingGameObject.name == placementGameObject.name)
                {
                    field.TopBuildingGameObject = placedBuilding;
                }
                else if (field.TopRightBuildingGameObject.name == placementGameObject.name)
                {
                    field.TopRightBuildingGameObject = placedBuilding;
                }
                else if (field.BottomRightBuildingGameObject.name == placementGameObject.name)
                {
                    field.BottomRightBuildingGameObject = placedBuilding;
                }
                else if (field.BottomBuildingGameObject.name == placementGameObject.name)
                {
                    field.BottomRightBuildingGameObject = placedBuilding;
                }
                else if (field.BottomLeftBuildingGameObject.name == placementGameObject.name)
                {
                    field.BottomLeftBuildingGameObject = placedBuilding;
                }
                else if (field.TopLeftBuildingGameObject.name == placementGameObject.name)
                {
                    field.TopLeftBuildingGameObject = placedBuilding;
                }
            }

            //Building has been placed here --> do not allow to place building here again
            Game.Instance.AvailableBuildingPlacements.Remove(placementGameObject);

            //Remove neightboured buildings from available buildingpositions
            foreach (GameObject neighbouredBuilding in Game.GetNeighbouredBuildingGOsOfBuilding(Game.GetNumberFromString(placementGameObject.name)))
            {
                //Make neighboured buildings invisible
                neighbouredBuilding.SetActive(false);

                //Neighboured buildings are not available anymore
                if (Game.Instance.AvailableBuildingPlacements.Contains(neighbouredBuilding))
                {
                    Game.Instance.AvailableBuildingPlacements.Remove(neighbouredBuilding);
                }
            }

            //Hide placement gameobject
            placementGameObject.SetActive(false);
        }

        public void PlaceStreet(GameObject placementGameObject)
        {
            //Get street gameobject of street that has to be placed
            GameObject placedStreet = Game.GetGameObjectByName($"StreetGameObject{Game.GetNumberFromString(placementGameObject.name)}");

            //Place street on clicked placement
            placedStreet.transform.position = placementGameObject.transform.position;

            //Make placed street visible
            placedStreet.SetActive(true);

            //Colorize street
            placedStreet.transform.Find("default").GetComponent<MeshRenderer>().material = this.Material;

            //Add placed street to players ownedstreets
            this.OwnedStreets.Add(placedStreet);

            //Remove resources
            if (Game.Instance.PlayersHavePlacedTheirFirstTwoHouses == true)
            {
                Game.Instance.CurrentPlayer.Resources[FieldType.Wood]--;
                Game.Instance.CurrentPlayer.Resources[FieldType.Brick]--;
                this.RefreshDisplayedResourceAmount();
            }

            //Refresh street properies for all Field objects
            foreach (Field field in Game.Instance.AllFields)
            {
                if (field.TopLeftStreetGameObject == placementGameObject)
                {
                    field.TopLeftStreetGameObject = placementGameObject;
                }
                else if (field.TopRightStreetGameObject == placementGameObject)
                {
                    field.TopRightStreetGameObject = placementGameObject;
                }
                else if (field.RightStreetGameObject == placementGameObject)
                {
                    field.RightStreetGameObject = placementGameObject;
                }
                else if (field.BottomRightStreetGameObject == placementGameObject)
                {
                    field.BottomRightStreetGameObject = placementGameObject;
                }
                else if (field.BottomLeftStreetGameObject == placementGameObject)
                {
                    field.BottomLeftStreetGameObject = placementGameObject;
                }
                else if (field.LeftStreetGameObject == placementGameObject)
                {
                    field.LeftStreetGameObject = placementGameObject;
                }
            }

            //Make placement gameobject invisible
            placementGameObject.SetActive(false);

            Game.Instance.AvailableStreetPlacements.Remove(placementGameObject);
        }

        public void HandoutResourcesAfterPlacingSecondHouse()
        {
            //Increase amount of resources
            foreach (FieldType fieldType in Game.GetResourcesByBuidlingName(this.OwnedBuildings[1].name))
            {
                this.Resources[fieldType]++;
            }

            if (this.IsAIPlayer == false)
            {
                this.RefreshDisplayedResourceAmount();
            }
        }

        public void IncreaseVictoryPoints()
        {
            this.VictoryPoints++;

            this.RefreshDisplayedVictoryPoints();

            if (this.VictoryPoints == 10)
            {
                Game.Instance.SettingsPanel.SetActive(true);
                Game.Instance.IsSettingsPanelOpen = true;
                Game.Instance.SettingsPanel.transform.Find("CloseSettingsButton").gameObject.SetActive(false);
                Game.Instance.WinMessageText.SetActive(true);
                Game.Instance.WinMessageText.GetComponent<TextMeshProUGUI>().text = this.IsAIPlayer ? "AI player won" : "You won";
            }
        }

        public void RefreshDisplayedResourceAmount()
        {
            if (this.IsAIPlayer == true)
            {
                return;
            }

            Transform resourceBarGO = Game.GetGameObjectByName("Canvas").transform.Find("ResourceBar");

            resourceBarGO.transform.Find("BrickCard").transform.Find("BrickAmountText").GetComponent<TextMeshProUGUI>().text = $"{this.Resources[FieldType.Brick]}x";
            resourceBarGO.transform.Find("WoodCard").transform.Find("WoodAmountText").GetComponent<TextMeshProUGUI>().text = $"{this.Resources[FieldType.Wood]}x";
            resourceBarGO.transform.Find("SheepCard").transform.Find("SheepAmountText").GetComponent<TextMeshProUGUI>().text = $"{this.Resources[FieldType.Sheep]}x";
            resourceBarGO.transform.Find("WheatCard").transform.Find("WheatAmountText").GetComponent<TextMeshProUGUI>().text = $"{this.Resources[FieldType.Wheat]}x";
            resourceBarGO.transform.Find("OreCard").transform.Find("OreAmountText").GetComponent<TextMeshProUGUI>().text = $"{this.Resources[FieldType.Ore]}x";
        }

        public void RefreshDisplayedVictoryPoints()
        {
            string playerType = this.IsAIPlayer == true ? "AI" : "Real";

            Game.GetGameObjectByName("Canvas").transform.Find("Scoreboard").Find($"{playerType}PlayerVictoryPoints").Find("VictoryPoints").GetComponent<TextMeshProUGUI>().text = this.VictoryPoints.ToString();
        }

        public async void MoveRobber()
        {
            foreach (Player player in Game.Instance.AllPlayers)
            {
                player.GiveHalfOfResourcesAway();
            }

            await Task.Delay(400);

            while (GiveAwayResources.ResourcesNeedToBeGivenAway)
            {
                await Task.Delay(200);
            }

            if (this.IsAIPlayer == true)
            {
                Game.Instance.ShowInfoMessage("AI placed robber", 1500);

                //All possible fields the robber can be placed on
                List<Field> newPossibleFieldsForRobber = Game.Instance.AllFields.Where(x => x.IsRobberPlacedOnThisField == false).ToList();

                //Get andom field to place robber on
                Field newFieldWithRobber = newPossibleFieldsForRobber[new System.Random().Next(0, newPossibleFieldsForRobber.Count())];

                Game.Instance.PlaceRobber(newFieldWithRobber);
            }
            else
            {
                Game.Instance.ShowInfoMessage("You need to place the robber!");

                Game.HideUIButtons();

                Game.Instance.Robber.NeedsToBeMovedByPlayer = true;

                while (Game.Instance.Robber.NeedsToBeMovedByPlayer == true)
                {
                    await Task.Delay(200);
                }

                Game.Instance.HideInfoMessage();
            }

            this.StealResourceFromOtherPlayer();
        }

        public void StealResourceFromOtherPlayer()
        {
            Field fieldWithRobber = Game.Instance.AllFields.Where(x => x.IsRobberPlacedOnThisField).FirstOrDefault();

            List<Player> otherPlayers = new();

            //Add all players that have building on field with robber to possible playerlist
            foreach (Player player in Game.Instance.AllPlayers)
            {
                if (player.OwnedBuildings.Contains(fieldWithRobber.TopBuildingGameObject) ||
                    player.OwnedBuildings.Contains(fieldWithRobber.TopRightBuildingGameObject) ||
                    player.OwnedBuildings.Contains(fieldWithRobber.BottomRightBuildingGameObject) ||
                    player.OwnedBuildings.Contains(fieldWithRobber.BottomBuildingGameObject) ||
                    player.OwnedBuildings.Contains(fieldWithRobber.BottomLeftBuildingGameObject) ||
                    player.OwnedBuildings.Contains(fieldWithRobber.TopLeftBuildingGameObject))
                {
                    otherPlayers.Add(player);
                }
            }

            if (otherPlayers.Count == 0)
            {
                return;
            }

            Player otherPlayer = otherPlayers[new System.Random().Next(0, otherPlayers.Count())];
            List<FieldType> resources = new();

            foreach (FieldType field in otherPlayer.Resources.Keys)
            {
                for (int i = 0; i < otherPlayer.Resources[field]; i++)
                {
                    resources.Add(field);
                }
            }

            if (resources.Count == 0)
            {
                Game.Instance.ShowInfoMessage("Opponent has no resources to steal from", 2000);
                return;
            }

            FieldType stolenResource = resources[new System.Random().Next(0, resources.Count())];

            otherPlayer.Resources[stolenResource]--;
            this.Resources[stolenResource]++;
            this.RefreshDisplayedResourceAmount();

            if (this.IsAIPlayer == false)
            {
                Game.Instance.ShowInfoMessage($"You have stolen one {stolenResource}", 2000);
            }
            else if (this.IsAIPlayer == true && otherPlayer.IsAIPlayer == false)
            {
                Game.Instance.ShowInfoMessage($"One {stolenResource} has been stolen from you", 2000);
            }
        }

        public async void GiveHalfOfResourcesAway()
        {
            List<FieldType> resources = new();

            foreach (FieldType field in this.Resources.Keys)
            {
                for (int i = 0; i < this.Resources[field]; i++)
                {
                    resources.Add(field);
                }
            }

            if (resources.Count <= 7)
            {
                return;
            }

            int amountToGiveAway = resources.Count / 2;

            if (this.IsAIPlayer == true)
            {
                for (int i = 0; i < amountToGiveAway; i++)
                {
                    int index = new System.Random().Next(0, resources.Count);
                    FieldType type = resources[index];
                    resources.RemoveAt(index);
                    this.Resources[type]--;
                }
            }
            else
            {
                GiveAwayResources.AmountToGiveAway = amountToGiveAway;
                GiveAwayResources.ShowGiveAwayResourcePanel();

                while (GiveAwayResources.ResourcesNeedToBeGivenAway)
                {
                    await Task.Delay(200);
                }
            }
        }

        public (bool, List<GameObject>) CanBuyStreet()
        {
            if (this.Resources[FieldType.Wood] == 0)
            {
                if (this.GetMissingFieldTypes().Contains(FieldType.Wood))
                {
                    this.TryGetResourceFromTrade(FieldType.Wood);
                }
            }

            if (this.Resources[FieldType.Brick] == 0)
            {
                if (this.GetMissingFieldTypes().Contains(FieldType.Brick))
                {
                    this.TryGetResourceFromTrade(FieldType.Brick);
                }
            }

            if (this.Resources[FieldType.Wood] == 0 || this.Resources[FieldType.Brick] == 0)
            {
                return (false, null);
            }

            List<GameObject> possibleStreets = new();

            //Get all neighboured streets of players owned streets
            foreach (GameObject street in Game.Instance.CurrentPlayer.OwnedStreets)
            {
                possibleStreets.AddRange(Game.GetNeighbouredStreetGOsOfStreet(Game.GetNumberFromString(street.name)));
            }

            //Remove all neighboured streets that are not available anyways
            for (int i = possibleStreets.Count - 1; i >= 0; i--)
            {
                if (Game.Instance.AvailableStreetPlacements.Contains(possibleStreets[i]) == false)
                {
                    possibleStreets.RemoveAt(i);
                }
            }

            if (possibleStreets.Count == 0)
            {
                return (false, null);
            }

            return (true, possibleStreets);
        }

        public (bool, List<GameObject>) CanBuySettlement()
        {
            if (this.Resources[FieldType.Wood] == 0)
            {
                if (this.GetMissingFieldTypes().Contains(FieldType.Wood))
                {
                    this.TryGetResourceFromTrade(FieldType.Wood);
                }
            }

            if (this.Resources[FieldType.Brick] == 0)
            {
                if (this.GetMissingFieldTypes().Contains(FieldType.Brick))
                {
                    this.TryGetResourceFromTrade(FieldType.Brick);
                }
            }

            if (this.Resources[FieldType.Wheat] == 0)
            {
                if (this.GetMissingFieldTypes().Contains(FieldType.Wheat))
                {
                    this.TryGetResourceFromTrade(FieldType.Wheat);
                }
            }

            if (this.Resources[FieldType.Sheep] == 0)
            {
                if (this.GetMissingFieldTypes().Contains(FieldType.Sheep))
                {
                    this.TryGetResourceFromTrade(FieldType.Sheep);
                }
            }

            //Check if player has enough resources 
            if (this.Resources[FieldType.Wood] == 0 || this.Resources[FieldType.Brick] == 0 || this.Resources[FieldType.Wheat] == 0 || this.Resources[FieldType.Sheep] == 0)
            {
                return (false, null);
            }

            List<GameObject> possibleHouses = new();

            //Get all possible buildings by players streets
            foreach (GameObject street in Game.Instance.CurrentPlayer.OwnedStreets)
            {
                possibleHouses.AddRange(Game.GetNeighbouredBuildingGOsOfStreet(Game.GetNumberFromString(street.name)));
            }

            //Remove all buidlings that are not available anymore
            for (int i = possibleHouses.Count - 1; i >= 0; i--)
            {
                if (Game.Instance.AvailableBuildingPlacements.Contains(possibleHouses[i]) == false)
                {
                    possibleHouses.RemoveAt(i);
                }
            }

            //Check if there are available settlements
            if (possibleHouses.Count == 0)
            {
                return (false, null);
            }

            return (true, possibleHouses);
        }

        public (bool, List<GameObject>) CanBuyVeste()
        {
            for (int i = 0; i < 2; i++)
            {
                if (this.Resources[FieldType.Wheat] < 2)
                {
                    if (this.GetMissingFieldTypes().Contains(FieldType.Wheat))
                    {
                        this.TryGetResourceFromTrade(FieldType.Wheat);
                    }
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (this.Resources[FieldType.Ore] < 2)
                {
                    if (this.GetMissingFieldTypes().Contains(FieldType.Ore))
                    {
                        this.TryGetResourceFromTrade(FieldType.Ore);
                    }
                }
            }

            //Check if player has enough resources
            if (this.Resources[FieldType.Wheat] < 2 || this.Resources[FieldType.Ore] < 3)
            {
                return (false, null);
            }

            //Check if player owns at least one settlement that can be upgraded
            if (this.OwnedBuildings.Where(x => x.name.Contains("House") == true).Count() < 1)
            {
                return (false, null);
            }

            return (true, this.OwnedBuildings.Where(x => x.name.Contains("House") == true).ToList());
        }

        /// <summary>
        /// Returns a list of FieldTypes the players has no acces to because no settlements placed on this field
        /// </summary>
        /// <returns></returns>
        public List<FieldType> GetMissingFieldTypes()
        {
            List<FieldType> missingFieldTypes = new() { FieldType.Wood, FieldType.Brick, FieldType.Wheat, FieldType.Sheep, FieldType.Ore };

            foreach (Field field in Game.Instance.AllFields)
            {
                if (this.OwnedBuildings.Contains(field.TopBuildingGameObject) ||
                    this.OwnedBuildings.Contains(field.TopRightBuildingGameObject) ||
                    this.OwnedBuildings.Contains(field.BottomRightBuildingGameObject) ||
                    this.OwnedBuildings.Contains(field.BottomBuildingGameObject) ||
                    this.OwnedBuildings.Contains(field.BottomLeftBuildingGameObject) ||
                    this.OwnedBuildings.Contains(field.TopLeftBuildingGameObject))
                {
                    //Player has building on this field --> remove from missing fieldtypes
                    if (missingFieldTypes.Contains(field.FieldType))
                    {
                        missingFieldTypes.Remove(field.FieldType);
                    }
                }
            }

            return missingFieldTypes;
        }

        /// <summary>
        /// Tries to trade resources for the wantedResource
        /// </summary>
        /// <param name="wantedResource"></param>
        public void TryGetResourceFromTrade(FieldType wantedResource)
        {
            foreach (FieldType field in this.Resources.Keys)
            {
                if (field != wantedResource && this.Resources[field] >= 4)
                {
                    this.Resources[field] -= 4;
                    this.Resources[wantedResource]++;
                    break;
                }
            }
        }

        public void SetMaterial()
        {
            if (this.IsAIPlayer == true)
            {
                this.Material = Game.GetGameObjectByName("GOWithAIPlayer1Material").GetComponent<MeshRenderer>().material;
            }
            else
            {
                this.Material = Game.GetGameObjectByName("GOWithRealPlayerMaterial").GetComponent<MeshRenderer>().material;
            }
        }

        /// <summary>
        /// Creates a new Player object
        /// </summary>
        /// <param name="color"></param>
        /// <param name="isAIPlayer"></param>
        /// <returns></returns>
        public static Player CreateNewPlayer(bool isAIPlayer)
        {
            Player newPlayer = new()
            {
                VictoryPoints = 0,
                IsAIPlayer = isAIPlayer,
                Resources = new Dictionary<FieldType, int>(),
                OwnedBuildings = new List<GameObject>(),
                OwnedStreets = new List<GameObject>(),
            };

            if (Game.Instance.IsTestGame == false)
            {
                newPlayer.Resources.Add(FieldType.Ore, 0);
                newPlayer.Resources.Add(FieldType.Brick, 0);
                newPlayer.Resources.Add(FieldType.Wood, 0);
                newPlayer.Resources.Add(FieldType.Wheat, 0);
                newPlayer.Resources.Add(FieldType.Sheep, 0);
            } 
            else
            {
                newPlayer.Resources.Add(FieldType.Ore, 20);
                newPlayer.Resources.Add(FieldType.Brick, 20);
                newPlayer.Resources.Add(FieldType.Wood, 20);
                newPlayer.Resources.Add(FieldType.Wheat, 20);
                newPlayer.Resources.Add(FieldType.Sheep, 20);
            }

            newPlayer.SetMaterial();

            if (newPlayer.IsAIPlayer == false)
            {
                newPlayer.RefreshDisplayedResourceAmount();
                newPlayer.RefreshDisplayedVictoryPoints();
            }

            return newPlayer;
        }

        #endregion Methods

    }
}
