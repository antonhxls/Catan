using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Assets;
using Unity.VisualScripting;
using System.Text.RegularExpressions;
using TMPro;

namespace Assets
{
    public class Game : MonoBehaviour
    {

        #region Properties

        public static Game Instance { get; set; }

        #region Fieldnames in hierarchy

        const string FIELD0_NAME = "Tile0";
        const string FIELD1_NAME = "Tile1";
        const string FIELD2_NAME = "Tile2";
        const string FIELD3_NAME = "Tile3";
        const string FIELD4_NAME = "Tile4";
        const string FIELD5_NAME = "Tile5";
        const string FIELD6_NAME = "Tile6";
        const string FIELD7_NAME = "Tile7";
        const string FIELD8_NAME = "Tile8";
        const string FIELD9_NAME = "Tile9";
        const string FIELD10_NAME = "Tile10";
        const string FIELD11_NAME = "Tile11";
        const string FIELD12_NAME = "Tile12";
        const string FIELD13_NAME = "Tile13";
        const string FIELD14_NAME = "Tile14";
        const string FIELD15_NAME = "Tile15";
        const string FIELD16_NAME = "Tile16";
        const string FIELD17_NAME = "Tile17";
        const string FIELD18_NAME = "Tile18";

        #endregion Fieldnames in hierarchy

        #region Maximum fieldtype amounts

        const int DESERTFIELD_AMOUNT = 1;
        const int BRICKFIELD_AMOUNT = 3;
        const int OREFIELD_AMOUNT = 3;
        const int WOODFIELD_AMOUNT = 4;
        const int WHEATFIELD_AMOUNT = 4;
        const int SHEEPFIELD_AMOUNT = 4;

        #endregion Maximum fieldtype amounts

        #region Materials

        public static Material DesertMaterial;
        public static Material OreMaterial;
        public static Material BrickMaterial;
        public static Material WheatMaterial;
        public static Material WoodMaterial;
        public static Material SheepMaterial;

        #endregion Materials

        const int PLAYER_AMOUNT = 2;

        public List<Field> AllFields { get; set; }

        public List<Player> AllPlayers { get; set; }

        public List<GameObject> AvailableBuildingPlacements { get; set; }

        public List<GameObject> AvailableStreetPlacements { get; set; }

        public Player CurrentPlayer { get; set; }

        public Robber Robber { get; set; }

        public bool PlayersHavePlacedTheirFirstTwoHouses { get; set; }

        public static GameObject NextTurnButtonGO { get; set; }

        public GameObject LastRolledNumberBoard { get; set; }

        public GameObject TempLastRolledNumber { get; set; }

        public GameObject UpgradeToVestePanel { get; set; }

        public GameObject BuyPanel { get; set; }

        public GameObject BuyButton { get; set; }

        public GameObject TradePanel { get; set; }

        public GameObject TradeButton { get; set; }

        public GameObject MessagePanel { get; set; }

        public GameObject Message { get; set; }

        public GameObject SettingsPanel { get; set; }

        public GameObject WinMessageText { get; set; }

        public bool IsSettingsPanelOpen { get; set; }

        public int LastRolledNumber { get; set; }

        public GameObject QuickGuidePanel { get; set; }

        public GameObject StartGamePanel { get; set; }

        public bool IsTestGame { get; set; }

        #endregion Properties

        void Start()
        {
            Game.Instance = this;

            this.MakeGameObjectsInvisibleAtStart();
        }

        public void Initialize(bool isTestGame = false)
        {
            this.IsTestGame = isTestGame;

            UnityEngine.Debug.Log("Initializing materials...");

            //Get Materials
            Game.InitializeMaterials();

            UnityEngine.Debug.Log("Initialized materials successfully");
            UnityEngine.Debug.Log("Initializing fields...");

            this.InitializeFields();

            UnityEngine.Debug.Log("Initialized fields successfully");

            //Get GameObject of the robber
            this.Robber = Robber.CreateNewRobber(GameObject.Find("Robber"));

            Robber.IsInitialized = true;

            UnityEngine.Debug.Log("Initializing players...");

            this.InitializePlayers();

            UnityEngine.Debug.Log("Initialized players successfully");
            UnityEngine.Debug.Log("Initializing placements...");

            this.InitializePlacements();

            UnityEngine.Debug.Log("Initialized placements successfully");
            UnityEngine.Debug.Log("Players now start to place the first 2 houses and streets");

            this.LetPlayersPlaceFirstHouseAndStreet();
        }

        public void MakeGameObjectsInvisibleAtStart()
        {
            this.LastRolledNumberBoard = Game.GetGameObjectByName("Canvas").transform.Find("LastRolledNumberBoard").gameObject;
            this.TempLastRolledNumber = Game.GetGameObjectByName("Canvas").transform.Find("TempLastRolledNumber").gameObject;
            this.UpgradeToVestePanel = Game.GetGameObjectByName("Canvas").transform.Find("UpgradeToVestePanel").gameObject;
            this.TradePanel = Game.GetGameObjectByName("Canvas").transform.Find("TradePanel").gameObject;
            this.BuyPanel = Game.GetGameObjectByName("Canvas").transform.Find("BuyPanel").gameObject;
            this.BuyButton = Game.GetGameObjectByName("Canvas").transform.Find("BuyButton").gameObject;
            this.TradeButton = Game.GetGameObjectByName("Canvas").transform.Find("TradeButton").gameObject;
            this.MessagePanel = Game.GetGameObjectByName("Canvas").transform.Find("MessagePanel").gameObject;
            this.Message = Game.GetGameObjectByName("Canvas").transform.Find("MessagePanel").transform.Find("Message").gameObject;
            this.SettingsPanel = Game.GetGameObjectByName("Canvas").transform.Find("SettingsPanel").gameObject;
            this.WinMessageText = Game.GetGameObjectByName("Canvas").transform.Find("SettingsPanel").transform.Find("WinMessageText").gameObject;
            this.QuickGuidePanel = Game.GetGameObjectByName("Canvas").transform.Find("QuickGuidePanel").gameObject;
            this.StartGamePanel = Game.GetGameObjectByName("Canvas").transform.Find("GameStartPanel").gameObject;

            Game.NextTurnButtonGO = Game.GetGameObjectByName("Next Turn Button");

            for (int i = 0; i <= 53; i++)
            {
                Game.GetGameObjectByName($"HouseGameObject{i}").SetActive(false);
                Game.GetGameObjectByName($"VesteGameObject{i}").SetActive(false);
            }

            for (int i = 0; i <= 71; i++)
            {
                Game.GetGameObjectByName($"StreetGameObject{i}").SetActive(false);
            }

            this.LastRolledNumberBoard.SetActive(false);
            this.TempLastRolledNumber.SetActive(false);
            this.UpgradeToVestePanel.SetActive(false);
            this.BuyPanel.SetActive(false);
            this.TradePanel.SetActive(false);
            this.MessagePanel.SetActive(false);
            this.SettingsPanel.SetActive(false);
            this.WinMessageText.SetActive(false);
            this.QuickGuidePanel.SetActive(false);

            Game.HideUIButtons();
        }

        private void InitializeFields()
        {
            this.SetAllFieldsFromHierarchy();

            List<FieldType> missingFieldTypes = new() { FieldType.Desert, FieldType.Brick, FieldType.Ore, FieldType.Wood, FieldType.Wheat, FieldType.Sheep };

            int desertFieldsCount = 0;
            int brickFieldsCount = 0;
            int oreFieldsCount = 0;
            int woodFieldsCount = 0;
            int wheatFieldsCount = 0;
            int sheepFieldsCount = 0;

            foreach (Field field in this.AllFields)
            {
                //Generate random field
                FieldType randomGeneratedFieldType = missingFieldTypes[new System.Random().Next(0, missingFieldTypes.Count())];

                //Set fieldtype of this field
                field.SetFieldType(randomGeneratedFieldType);

                //Remove random generated fieldtype from list if all fields of this fieldtype have been placed
                switch (randomGeneratedFieldType)
                {
                    case FieldType.Desert:
                        {
                            desertFieldsCount++;

                            if (desertFieldsCount == DESERTFIELD_AMOUNT)
                            {
                                missingFieldTypes.Remove(FieldType.Desert);
                            }

                            break;
                        }
                    case FieldType.Brick:
                        {
                            brickFieldsCount++;

                            if (brickFieldsCount == BRICKFIELD_AMOUNT)
                            {
                                missingFieldTypes.Remove(FieldType.Brick);
                            }

                            break;
                        }
                    case FieldType.Ore:
                        {
                            oreFieldsCount++;

                            if (oreFieldsCount == OREFIELD_AMOUNT)
                            {
                                missingFieldTypes.Remove(FieldType.Ore);
                            }

                            break;
                        }
                    case FieldType.Wood:
                        {
                            woodFieldsCount++;

                            if (woodFieldsCount == WOODFIELD_AMOUNT)
                            {
                                missingFieldTypes.Remove(FieldType.Wood);
                            }

                            break;
                        }
                    case FieldType.Wheat:
                        {
                            wheatFieldsCount++;

                            if (wheatFieldsCount == WHEATFIELD_AMOUNT)
                            {
                                missingFieldTypes.Remove(FieldType.Wheat);
                            }

                            break;
                        }
                    case FieldType.Sheep:
                        {
                            sheepFieldsCount++;

                            if (sheepFieldsCount == SHEEPFIELD_AMOUNT)
                            {
                                missingFieldTypes.Remove(FieldType.Sheep);
                            }

                            break;
                        }
                }
            }

            this.AssignDiceNumbersToAllFields();
        }

        /// <summary>
        /// Returns a list of <see cref="Field"/>s from all field tiles in hierarchy
        /// </summary>
        /// <returns></returns>
        private void SetAllFieldsFromHierarchy()
        {
            this.AllFields = new List<Field>
            {
                Field.CreateNewField(GameObject.Find(FIELD0_NAME), 0),
                Field.CreateNewField(GameObject.Find(FIELD1_NAME), 1),
                Field.CreateNewField(GameObject.Find(FIELD2_NAME), 2),
                Field.CreateNewField(GameObject.Find(FIELD3_NAME), 3),
                Field.CreateNewField(GameObject.Find(FIELD4_NAME), 4),
                Field.CreateNewField(GameObject.Find(FIELD5_NAME), 5),
                Field.CreateNewField(GameObject.Find(FIELD6_NAME), 6),
                Field.CreateNewField(GameObject.Find(FIELD7_NAME), 7),
                Field.CreateNewField(GameObject.Find(FIELD8_NAME), 8),
                Field.CreateNewField(GameObject.Find(FIELD9_NAME), 9),
                Field.CreateNewField(GameObject.Find(FIELD10_NAME), 10),
                Field.CreateNewField(GameObject.Find(FIELD11_NAME), 11),
                Field.CreateNewField(GameObject.Find(FIELD12_NAME), 12),
                Field.CreateNewField(GameObject.Find(FIELD13_NAME), 13),
                Field.CreateNewField(GameObject.Find(FIELD14_NAME), 14),
                Field.CreateNewField(GameObject.Find(FIELD15_NAME), 15),
                Field.CreateNewField(GameObject.Find(FIELD16_NAME), 16),
                Field.CreateNewField(GameObject.Find(FIELD17_NAME), 17),
                Field.CreateNewField(GameObject.Find(FIELD18_NAME), 18)
            };
        }

        public static void InitializeMaterials()
        {
            Game.DesertMaterial = GameObject.Find("GOWithDesertMaterial").GetComponent<MeshRenderer>().material;
            Game.OreMaterial = GameObject.Find("GOWithOreMaterial").GetComponent<MeshRenderer>().material;
            Game.BrickMaterial = GameObject.Find("GOWithBrickMaterial").GetComponent<MeshRenderer>().material;
            Game.WoodMaterial = GameObject.Find("GOWithWoodMaterial").GetComponent<MeshRenderer>().material;
            Game.SheepMaterial = GameObject.Find("GOWithSheepMaterial").GetComponent<MeshRenderer>().material;
            Game.WheatMaterial = GameObject.Find("GOWithWheatMaterial").GetComponent<MeshRenderer>().material;
        }

        /// <summary>
        /// Assigns a dice number to all fields and places the dice number gameobjects on the board
        /// </summary>
        public void AssignDiceNumbersToAllFields()
        {
            List<int> missingDiceNumbers = new() { 2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12 };

            foreach (Field field in this.AllFields)
            {
                if (field.FieldType == FieldType.Desert)
                {
                    //Desert does not have a dice number
                    field.DiceNumber = 0;
                    continue;
                }

                //Get a random dice number from all numbers that have not been assigned to a field yet
                int randomDiceNumber = missingDiceNumbers[new System.Random().Next(0, missingDiceNumbers.Count())];

                //Assign new dice number to field
                field.DiceNumber = randomDiceNumber;

                //Set DiceNumber gameobject on this field
                GameObject diceNumberGO = null;

                //Most numbers have 2 gameobjects --> decide which gameobject has to be placed on this field
                if (randomDiceNumber != 2 && randomDiceNumber != 12 && missingDiceNumbers.Count(x => x == randomDiceNumber) == 2)
                {
                    //First time this number has been assigned --> place first of the dicenumbers gameobject
                    diceNumberGO = GameObject.Find($"DiceNumber{randomDiceNumber}.2");
                }
                else
                {
                    diceNumberGO = GameObject.Find($"DiceNumber{randomDiceNumber}.1");
                }

                field.DiceNumberGameObject = diceNumberGO;

                diceNumberGO.transform.position = new Vector3(field.GameObject.transform.position.x, 0.0603f, field.GameObject.transform.position.z);

                //Remove new dice number from list since it has been assigned to a field
                missingDiceNumbers.RemoveAt(missingDiceNumbers.IndexOf(randomDiceNumber));
            }
        }

        /// <summary>
        /// Creates all players
        /// </summary>
        public void InitializePlayers()
        {
            this.AllPlayers = new List<Player>();

            int playersTurn = new System.Random().Next(0, PLAYER_AMOUNT);

            //Randomize order of players
            for (int i = 0; i < PLAYER_AMOUNT; i++)
            {
                if (i == playersTurn)
                {
                    this.AllPlayers.Add(Player.CreateNewPlayer(isAIPlayer: false));
                }
                else
                {
                    this.AllPlayers.Add(Player.CreateNewPlayer(isAIPlayer: true));
                }
            }

            this.CurrentPlayer = this.AllPlayers.First();
        }

        public void InitializePlacements()
        {
            this.AvailableBuildingPlacements = new List<GameObject>();
            this.AvailableStreetPlacements = new List<GameObject>();

            for (int i = 0; i <= 53; i++)
            {
                this.AvailableBuildingPlacements.Add(Game.GetGameObjectByName($"BuildingPlacement{i}"));
            }

            for (int i = 0; i <= 71; i++)
            {
                this.AvailableStreetPlacements.Add(Game.GetGameObjectByName($"StreetPlacement{i}"));
            }
        }

        /// <summary>
        /// Executed when a player ends his turn --> next player
        /// </summary>
        public async void NextTurn()
        {
            int indexOfNextPlayer = this.AllPlayers.IndexOf(this.CurrentPlayer) + 1;

            if (indexOfNextPlayer == PLAYER_AMOUNT)
            {
                //End of playerlist reached --> start from beginning
                indexOfNextPlayer = 0;
            }

            this.CurrentPlayer = this.AllPlayers.ElementAt(indexOfNextPlayer);

            await this.RollTheDice();

            if (this.LastRolledNumber != 7 || this.CurrentPlayer.IsAIPlayer == true)
            {
                await this.CurrentPlayer.MakeMove();
            }
        }

        public async Task RollTheDice()
        {
            this.LastRolledNumber = new System.Random().Next(1, 7) + new System.Random().Next(1, 7);
            UnityEngine.Debug.Log(this.LastRolledNumber);
            this.LastRolledNumberBoard.SetActive(true);
            this.TempLastRolledNumber.SetActive(true);

            this.LastRolledNumberBoard.transform.Find("LastRolledNumberText").transform.Find("Number").GetComponent<TextMeshProUGUI>().text = this.LastRolledNumber.ToString();
            this.TempLastRolledNumber.transform.Find("Number").GetComponent<TextMeshProUGUI>().text = this.LastRolledNumber.ToString();

            if (this.LastRolledNumber != 7)
            {
                this.HandoutResourcesAfterDiceHasBeenRolled(rolledNumber: this.LastRolledNumber);
            }
            else
            {
                //7 --> move robber
                this.CurrentPlayer.MoveRobber();
            }

            await Task.Delay(2000);

            Game.GetGameObjectByName("Canvas").transform.Find("TempLastRolledNumber").gameObject.SetActive(false);
        }

        public void PlaceRobber(Field field)
        {
            this.Robber.GameObject.transform.position = new Vector3(field.GameObject.transform.position.x - 0.4f, Robber.YPOS, field.GameObject.transform.position.z);

            this.AllFields.Where(x => x.IsRobberPlacedOnThisField == true).First().IsRobberPlacedOnThisField = false;

            field.IsRobberPlacedOnThisField = true;
        }

        public void HandoutResourcesAfterDiceHasBeenRolled(int rolledNumber)
        {
            //Handout resources of all fields with the number that has been rolled
            foreach (Field field in this.AllFields.Where(x => x.DiceNumber == rolledNumber && x.IsRobberPlacedOnThisField == false))
            {
                foreach (Player player in this.AllPlayers)
                {
                    if (player.OwnedBuildings.Contains(field.TopBuildingGameObject))
                    {
                        player.Resources[field.FieldType]++;

                        if (field.TopBuildingGameObject.name.Contains("Veste"))
                        {
                            //Increase by 2 for Veste
                            player.Resources[field.FieldType]++;
                        }
                    }

                    if (player.OwnedBuildings.Contains(field.TopRightBuildingGameObject))
                    {
                        player.Resources[field.FieldType]++;

                        if (field.TopRightBuildingGameObject.name.Contains("Veste"))
                        {
                            //Increase by 2 for Veste
                            player.Resources[field.FieldType]++;
                        }
                    }

                    if (player.OwnedBuildings.Contains(field.BottomRightBuildingGameObject))
                    {
                        player.Resources[field.FieldType]++;

                        if (field.BottomRightBuildingGameObject.name.Contains("Veste"))
                        {
                            //Increase by 2 for Veste
                            player.Resources[field.FieldType]++;
                        }
                    }

                    if (player.OwnedBuildings.Contains(field.BottomBuildingGameObject))
                    {
                        player.Resources[field.FieldType]++;

                        if (field.BottomBuildingGameObject.name.Contains("Veste"))
                        {
                            //Increase by 2 for Veste
                            player.Resources[field.FieldType]++;
                        }
                    }

                    if (player.OwnedBuildings.Contains(field.BottomLeftBuildingGameObject))
                    {
                        player.Resources[field.FieldType]++;

                        if (field.BottomLeftBuildingGameObject.name.Contains("Veste"))
                        {
                            //Increase by 2 for Veste
                            player.Resources[field.FieldType]++;
                        }
                    }

                    if (player.OwnedBuildings.Contains(field.TopLeftBuildingGameObject))
                    {
                        player.Resources[field.FieldType]++;

                        if (field.TopLeftBuildingGameObject.name.Contains("Veste"))
                        {
                            //Increase by 2 for Veste
                            player.Resources[field.FieldType]++;
                        }
                    }
                }
            }

            //Refresh displayed resources amounts
            foreach (Player player in this.AllPlayers.Where(x => x.IsAIPlayer == false))
            {
                player.RefreshDisplayedResourceAmount();
            }
        }

        public async void LetPlayersPlaceFirstHouseAndStreet()
        {
            //Make all possible streetplacements invisible
            foreach (GameObject streetPlacement in this.AvailableStreetPlacements)
            {
                streetPlacement.SetActive(false);
            }

            //Make all possible buildingplacements invisible
            foreach (GameObject buildingPlacement in this.AvailableBuildingPlacements)
            {
                buildingPlacement.SetActive(false);
            }

            //Start first buidling process for all players
            foreach (Player player in this.AllPlayers)
            {
                this.CurrentPlayer = player;

                if (player.IsAIPlayer == true)
                {
                    //AI player builds his first building and his first street
                    UnityEngine.Debug.Log("AI is placing first house and street");
                    player.AIPlaceFirstBuildingAndStreet();
                }
                else
                {
                    //Real player builds his first building and his first street
                    UnityEngine.Debug.Log("You can now place your first house and street");

                    //Make all available buildingplacements visible for player
                    foreach (GameObject buildingPlacement in this.AvailableBuildingPlacements)
                    {
                        buildingPlacement.SetActive(true);
                    }

                    //Wait for real player to select a field where he wants to place his first building
                    while (player.PlacedFirstHouseAlready == false)
                    {
                        await Task.Delay(200);
                    }

                    //Real player has placed his first building --> make all available buildingplacements invisible
                    foreach (GameObject buildingPlacement in this.AvailableBuildingPlacements)
                    {
                        buildingPlacement.SetActive(false);
                    }

                    //Make all neighboured streets to the first placed building visible for the real player
                    foreach (GameObject streetPlacement in Game.GetNeighbouredStreetGOsOfBuilding(Game.GetNumberFromString(this.CurrentPlayer.OwnedBuildings.First().name)))
                    {
                        streetPlacement.SetActive(true);
                    }

                    //Wait for real player to select a field where he wants to place his first street
                    while (player.PlacedFirstStreetAlready == false)
                    {
                        await Task.Delay(200);
                    }

                    //Real Player has placed his first street --> make all available streetplacements invisible
                    foreach (GameObject streetPlacement in this.AvailableStreetPlacements)
                    {
                        streetPlacement.SetActive(false);
                    }
                }
            }
        }

        public async void LetPlayersPlaceSecondHouseAndStreet()
        {
            //Make all possible streetplacements invisible
            foreach (GameObject streetPlacement in this.AvailableStreetPlacements)
            {
                streetPlacement.SetActive(false);
            }

            //Make all possible buildingplacements invisible
            foreach (GameObject buildingPlacement in this.AvailableBuildingPlacements)
            {
                buildingPlacement.SetActive(false);
            }

            //Now let all players place their second house and second street --> now backwards
            for (int i = PLAYER_AMOUNT - 1; i >= 0; i--)
            {
                this.CurrentPlayer = this.AllPlayers[i];

                if (this.CurrentPlayer.IsAIPlayer == true)
                {
                    //AI player builds his second building and his second street
                    UnityEngine.Debug.Log("AI is placing second house and street");
                    this.CurrentPlayer.AIPlaceSecondBuildingAndStreet();
                }
                else
                {
                    //Real player builds his first building and his first street
                    UnityEngine.Debug.Log("You can now place your second house and street");

                    //Make all available buildingplacements visible for player
                    foreach (GameObject buildingPlacement in this.AvailableBuildingPlacements)
                    {
                        buildingPlacement.SetActive(true);
                    }

                    //Wait for real player to place his second house
                    while (this.CurrentPlayer.PlacedSecondHouseAlready == false)
                    {
                        await Task.Delay(200);
                    }

                    //Real player has placed his second house --> make all available buildingplacements invisible
                    foreach (GameObject buildingPlacement in this.AvailableBuildingPlacements)
                    {
                        buildingPlacement.SetActive(false);
                    }

                    //Make all streetplacements visible that are neighbours of the players second building
                    foreach (GameObject streetPlacement in Game.GetNeighbouredStreetGOsOfBuilding(Game.GetNumberFromString(this.CurrentPlayer.OwnedBuildings[1].name)))
                    {
                        streetPlacement.SetActive(true);
                    }

                    //Wait for the real player to select a field where he wants to place his second street
                    while (this.CurrentPlayer.PlacedSecondStreetAlready == false)
                    {
                        await Task.Delay(200);
                    }

                    //Real player has placed his second street --> make all available streetplacements invisible
                    foreach (GameObject streetPlacement in this.AvailableStreetPlacements)
                    {
                        streetPlacement.SetActive(false);
                    }
                }
            }

            this.PlayersHavePlacedTheirFirstTwoHouses = true;

            this.NextTurn();
        }

        public async void ShowInfoMessage(string message, int duration = 3000)
        {
            this.Message.GetComponent<TextMeshProUGUI>().text = message;
            this.MessagePanel.SetActive(true);

            await Task.Delay(duration);

            this.MessagePanel.SetActive(false);
        }

        public void ShowInfoMessage(string message)
        {
            this.Message.GetComponent<TextMeshProUGUI>().text = message;
            this.MessagePanel.SetActive(true);
        }

        public void HideInfoMessage()
        {
            this.MessagePanel.SetActive(false);
        }

        public static GameObject GetGameObjectByName(string name)
        {

            foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (go.name == name)
                {
                    return go;
                }
            }

            UnityEngine.Debug.Log($"Could not get GameObject (function: GetGameObjectByName, parameter name = {name})");
            return null;
        }

        public static int GetNumberFromString(string name)
        {
            string pattern = @"\d+";

            Match m = Regex.Match(name, pattern);
            int number = 0;

            if (m.Success)
            {
                number = Int32.Parse(m.Value);
            }

            return number;
        }

        public static void HideUIButtons()
        {
            Game.NextTurnButtonGO.SetActive(false);
            Game.Instance.BuyButton.SetActive(false);
            Game.Instance.TradeButton.SetActive(false);
        }

        public static void ShowUIButtons()
        {
            Game.NextTurnButtonGO.SetActive(true);
            Game.Instance.BuyButton.SetActive(true);
            Game.Instance.TradeButton.SetActive(true);
        }

        public static List<FieldType> GetResourcesByBuidlingName(string buildingName)
        {
            List<FieldType> resourcesOfBuilding = new();

            foreach (Field field in Game.Instance.AllFields)
            {
                if (field.FieldType == FieldType.Desert)
                {
                    continue;
                }

                if (field.TopBuildingGameObject.name == buildingName)
                {
                    resourcesOfBuilding.Add(field.FieldType);
                }
                else if (field.TopRightBuildingGameObject.name == buildingName)
                {
                    resourcesOfBuilding.Add(field.FieldType);
                }
                else if (field.BottomRightBuildingGameObject.name == buildingName)
                {
                    resourcesOfBuilding.Add(field.FieldType);
                }
                else if (field.BottomBuildingGameObject.name == buildingName)
                {
                    resourcesOfBuilding.Add(field.FieldType);
                }
                else if (field.BottomLeftBuildingGameObject.name == buildingName)
                {
                    resourcesOfBuilding.Add(field.FieldType);
                }
                else if (field.TopLeftBuildingGameObject.name == buildingName)
                {
                    resourcesOfBuilding.Add(field.FieldType);
                }
            }

            UnityEngine.Debug.Log($"{buildingName} resources:");
            foreach (FieldType field in resourcesOfBuilding)
            {
                UnityEngine.Debug.Log(field.ToString());
            }

            return resourcesOfBuilding;
        }

        /// <summary>
        /// Returns a list of all street gameobjects that are located next to a building
        /// </summary>
        /// <returns></returns>
        public static List<GameObject> GetNeighbouredStreetGOsOfBuilding(int buildingNumber)
        {
            int? firstNeighbouredStreetNumber = null;
            int? secondNeighbouredStreetNumber = null;
            int? thirdNeighbouredStreetNumber = null;

            //Determine the numbers of the neighboured street GameObjects depending on the number of the building
            switch (buildingNumber)
            {
                case 0:
                    firstNeighbouredStreetNumber = 71;
                    secondNeighbouredStreetNumber = 0;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 1:
                    firstNeighbouredStreetNumber = 39;
                    secondNeighbouredStreetNumber = 40;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 2:
                    firstNeighbouredStreetNumber = 40;
                    secondNeighbouredStreetNumber = 41;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 3:
                    firstNeighbouredStreetNumber = 38;
                    secondNeighbouredStreetNumber = 36;
                    thirdNeighbouredStreetNumber = 37;
                    break;
                case 4:
                    firstNeighbouredStreetNumber = 43;
                    secondNeighbouredStreetNumber = 42;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 5:
                    firstNeighbouredStreetNumber = 44;
                    secondNeighbouredStreetNumber = 6;
                    thirdNeighbouredStreetNumber = 43;
                    break;
                case 6:
                    firstNeighbouredStreetNumber = 10;
                    secondNeighbouredStreetNumber = 44;
                    thirdNeighbouredStreetNumber = 45;
                    break;
                case 7:
                    firstNeighbouredStreetNumber = 47;
                    secondNeighbouredStreetNumber = 45;
                    thirdNeighbouredStreetNumber = 46;
                    break;
                case 8:
                    firstNeighbouredStreetNumber = 13;
                    secondNeighbouredStreetNumber = 47;
                    thirdNeighbouredStreetNumber = 48;
                    break;
                case 9:
                    firstNeighbouredStreetNumber = 2;
                    secondNeighbouredStreetNumber = 1;
                    thirdNeighbouredStreetNumber = 3;
                    break;
                case 10:
                    firstNeighbouredStreetNumber = 17;
                    secondNeighbouredStreetNumber = 2;
                    thirdNeighbouredStreetNumber = 4;
                    break;
                case 11:
                    firstNeighbouredStreetNumber = 25;
                    secondNeighbouredStreetNumber = 26;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 12:
                    firstNeighbouredStreetNumber = 24;
                    secondNeighbouredStreetNumber = 25;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 13:
                    firstNeighbouredStreetNumber = 22;
                    secondNeighbouredStreetNumber = 24;
                    thirdNeighbouredStreetNumber = 23;
                    break;
                case 14:
                    firstNeighbouredStreetNumber = 21;
                    secondNeighbouredStreetNumber = 22;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 15:
                    firstNeighbouredStreetNumber = 19;
                    secondNeighbouredStreetNumber = 21;
                    thirdNeighbouredStreetNumber = 20;
                    break;
                case 16:
                    firstNeighbouredStreetNumber = 18;
                    secondNeighbouredStreetNumber = 19;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 17:
                    firstNeighbouredStreetNumber = 18;
                    secondNeighbouredStreetNumber = 17;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 18:
                    firstNeighbouredStreetNumber = 29;
                    secondNeighbouredStreetNumber = 30;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 19:
                    firstNeighbouredStreetNumber = 26;
                    secondNeighbouredStreetNumber = 29;
                    thirdNeighbouredStreetNumber = 28;
                    break;
                case 20:
                    firstNeighbouredStreetNumber = 16;
                    secondNeighbouredStreetNumber = 28;
                    thirdNeighbouredStreetNumber = 27;
                    break;
                case 21:
                    firstNeighbouredStreetNumber = 23;
                    secondNeighbouredStreetNumber = 16;
                    thirdNeighbouredStreetNumber = 31;
                    break;
                case 22:
                    firstNeighbouredStreetNumber = 33;
                    secondNeighbouredStreetNumber = 31;
                    thirdNeighbouredStreetNumber = 32;
                    break;
                case 23:
                    firstNeighbouredStreetNumber = 20;
                    secondNeighbouredStreetNumber = 33;
                    thirdNeighbouredStreetNumber = 1;
                    break;
                case 24:
                    firstNeighbouredStreetNumber = 4;
                    secondNeighbouredStreetNumber = 5;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 25:
                    firstNeighbouredStreetNumber = 7;
                    secondNeighbouredStreetNumber = 6;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 26:
                    firstNeighbouredStreetNumber = 8;
                    secondNeighbouredStreetNumber = 7;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 27:
                    firstNeighbouredStreetNumber = 30;
                    secondNeighbouredStreetNumber = 8;
                    thirdNeighbouredStreetNumber = 9;
                    break;
                case 28:
                    firstNeighbouredStreetNumber = 11;
                    secondNeighbouredStreetNumber = 9;
                    thirdNeighbouredStreetNumber = 10;
                    break;
                case 29:
                    firstNeighbouredStreetNumber = 27;
                    secondNeighbouredStreetNumber = 11;
                    thirdNeighbouredStreetNumber = 12;
                    break;
                case 30:
                    firstNeighbouredStreetNumber = 14;
                    secondNeighbouredStreetNumber = 12;
                    thirdNeighbouredStreetNumber = 13;
                    break;
                case 31:
                    firstNeighbouredStreetNumber = 32;
                    secondNeighbouredStreetNumber = 14;
                    thirdNeighbouredStreetNumber = 15;
                    break;
                case 32:
                    firstNeighbouredStreetNumber = 35;
                    secondNeighbouredStreetNumber = 15;
                    thirdNeighbouredStreetNumber = 34;
                    break;
                case 33:
                    firstNeighbouredStreetNumber = 3;
                    secondNeighbouredStreetNumber = 35;
                    thirdNeighbouredStreetNumber = 36;
                    break;
                case 34:
                    firstNeighbouredStreetNumber = 5;
                    secondNeighbouredStreetNumber = 38;
                    thirdNeighbouredStreetNumber = 39;
                    break;
                case 35:
                    firstNeighbouredStreetNumber = 50;
                    secondNeighbouredStreetNumber = 48;
                    thirdNeighbouredStreetNumber = 49;
                    break;
                case 36:
                    firstNeighbouredStreetNumber = 34;
                    secondNeighbouredStreetNumber = 50;
                    thirdNeighbouredStreetNumber = 51;
                    break;
                case 37:
                    firstNeighbouredStreetNumber = 37;
                    secondNeighbouredStreetNumber = 53;
                    thirdNeighbouredStreetNumber = 54;
                    break;
                case 38:
                    firstNeighbouredStreetNumber = 41;
                    secondNeighbouredStreetNumber = 54;
                    thirdNeighbouredStreetNumber = 55;
                    break;
                case 39:
                    firstNeighbouredStreetNumber = 55;
                    secondNeighbouredStreetNumber = 56;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 40:
                    firstNeighbouredStreetNumber = 53;
                    secondNeighbouredStreetNumber = 51;
                    thirdNeighbouredStreetNumber = 52;
                    break;
                case 41:
                    firstNeighbouredStreetNumber = 59;
                    secondNeighbouredStreetNumber = 58;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 42:
                    firstNeighbouredStreetNumber = 57;
                    secondNeighbouredStreetNumber = 58;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 43:
                    firstNeighbouredStreetNumber = 60;
                    secondNeighbouredStreetNumber = 42;
                    thirdNeighbouredStreetNumber = 57;
                    break;
                case 44:
                    firstNeighbouredStreetNumber = 46;
                    secondNeighbouredStreetNumber = 60;
                    thirdNeighbouredStreetNumber = 61;
                    break;
                case 45:
                    firstNeighbouredStreetNumber = 64;
                    secondNeighbouredStreetNumber = 61;
                    thirdNeighbouredStreetNumber = 63;
                    break;
                case 46:
                    firstNeighbouredStreetNumber = 63;
                    secondNeighbouredStreetNumber = 59;
                    thirdNeighbouredStreetNumber = 62;
                    break;
                case 47:
                    firstNeighbouredStreetNumber = 49;
                    secondNeighbouredStreetNumber = 64;
                    thirdNeighbouredStreetNumber = 66;
                    break;
                case 48:
                    firstNeighbouredStreetNumber = 65;
                    secondNeighbouredStreetNumber = 62;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 49:
                    firstNeighbouredStreetNumber = 0;
                    secondNeighbouredStreetNumber = 67;
                    thirdNeighbouredStreetNumber = null;
                    break;
                case 50:
                    firstNeighbouredStreetNumber = 68;
                    secondNeighbouredStreetNumber = 65;
                    thirdNeighbouredStreetNumber = 67;
                    break;
                case 51:
                    firstNeighbouredStreetNumber = 69;
                    secondNeighbouredStreetNumber = 66;
                    thirdNeighbouredStreetNumber = 68;
                    break;
                case 52:
                    firstNeighbouredStreetNumber = 52;
                    secondNeighbouredStreetNumber = 69;
                    thirdNeighbouredStreetNumber = 70;
                    break;
                case 53:
                    firstNeighbouredStreetNumber = 56;
                    secondNeighbouredStreetNumber = 70;
                    thirdNeighbouredStreetNumber = 71;
                    break;
            }

            List<GameObject> neighbouredGameObjects = new List<GameObject>();

            //Add neighboured street GameObjects to list and return it
            if (firstNeighbouredStreetNumber != null)
            {
                neighbouredGameObjects.Add(Game.GetGameObjectByName($"StreetPlacement{firstNeighbouredStreetNumber}"));
            }

            if (secondNeighbouredStreetNumber != null)
            {
                neighbouredGameObjects.Add(Game.GetGameObjectByName($"StreetPlacement{secondNeighbouredStreetNumber}"));
            }

            if (thirdNeighbouredStreetNumber != null)
            {
                neighbouredGameObjects.Add(Game.GetGameObjectByName($"StreetPlacement{thirdNeighbouredStreetNumber}"));
            }

            return neighbouredGameObjects;
        }

        /// <summary>
        /// Returns a list of all building gameobjects that are located next to a building
        /// </summary>
        /// <returns></returns>
        public static List<GameObject> GetNeighbouredBuildingGOsOfBuilding(int buildingNumber)
        {
            int? firstNeighbouredBuildingNumber = null;
            int? secondNeighbouredBuildingNumber = null;
            int? thirdNeighbouredBuildingNumber = null;

            switch (buildingNumber)
            {
                case 0:
                    firstNeighbouredBuildingNumber = 53;
                    secondNeighbouredBuildingNumber = 49;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 1:
                    firstNeighbouredBuildingNumber = 43;
                    secondNeighbouredBuildingNumber = 2;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 2:
                    firstNeighbouredBuildingNumber = 1;
                    secondNeighbouredBuildingNumber = 38;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 3:
                    firstNeighbouredBuildingNumber = 36;
                    secondNeighbouredBuildingNumber = 37;
                    thirdNeighbouredBuildingNumber = 38;
                    break;
                case 4:
                    firstNeighbouredBuildingNumber = 5;
                    secondNeighbouredBuildingNumber = 43;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 5:
                    firstNeighbouredBuildingNumber = 4;
                    secondNeighbouredBuildingNumber = 6;
                    thirdNeighbouredBuildingNumber = 25;
                    break;
                case 6:
                    firstNeighbouredBuildingNumber = 28;
                    secondNeighbouredBuildingNumber = 5;
                    thirdNeighbouredBuildingNumber = 7;
                    break;
                case 7:
                    firstNeighbouredBuildingNumber = 8;
                    secondNeighbouredBuildingNumber = 6;
                    thirdNeighbouredBuildingNumber = 44;
                    break;
                case 8:
                    firstNeighbouredBuildingNumber = 30;
                    secondNeighbouredBuildingNumber = 7;
                    thirdNeighbouredBuildingNumber = 35;
                    break;
                case 9:
                    firstNeighbouredBuildingNumber = 10;
                    secondNeighbouredBuildingNumber = 23;
                    thirdNeighbouredBuildingNumber = 33;
                    break;
                case 10:
                    firstNeighbouredBuildingNumber = 17;
                    secondNeighbouredBuildingNumber = 9;
                    thirdNeighbouredBuildingNumber = 24;
                    break;
                case 11:
                    firstNeighbouredBuildingNumber = 12;
                    secondNeighbouredBuildingNumber = 19;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 12:
                    firstNeighbouredBuildingNumber = 13;
                    secondNeighbouredBuildingNumber = 11;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 13:
                    firstNeighbouredBuildingNumber = 14;
                    secondNeighbouredBuildingNumber = 12;
                    thirdNeighbouredBuildingNumber = 21;
                    break;
                case 14:
                    firstNeighbouredBuildingNumber = 15;
                    secondNeighbouredBuildingNumber = 13;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 15:
                    firstNeighbouredBuildingNumber = 16;
                    secondNeighbouredBuildingNumber = 14;
                    thirdNeighbouredBuildingNumber = 20;
                    break;
                case 16:
                    firstNeighbouredBuildingNumber = 17;
                    secondNeighbouredBuildingNumber = 15;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 17:
                    firstNeighbouredBuildingNumber = 16;
                    secondNeighbouredBuildingNumber = 10;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 18:
                    firstNeighbouredBuildingNumber = 19;
                    secondNeighbouredBuildingNumber = 27;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 19:
                    firstNeighbouredBuildingNumber = 11;
                    secondNeighbouredBuildingNumber = 18;
                    thirdNeighbouredBuildingNumber = 20;
                    break;
                case 20:
                    firstNeighbouredBuildingNumber = 21;
                    secondNeighbouredBuildingNumber = 19;
                    thirdNeighbouredBuildingNumber = 29;
                    break;
                case 21:
                    firstNeighbouredBuildingNumber = 13;
                    secondNeighbouredBuildingNumber = 20;
                    thirdNeighbouredBuildingNumber = 22;
                    break;
                case 22:
                    firstNeighbouredBuildingNumber = 23;
                    secondNeighbouredBuildingNumber = 21;
                    thirdNeighbouredBuildingNumber = 31;
                    break;
                case 23:
                    firstNeighbouredBuildingNumber = 15;
                    secondNeighbouredBuildingNumber = 22;
                    thirdNeighbouredBuildingNumber = 9;
                    break;
                case 24:
                    firstNeighbouredBuildingNumber = 10;
                    secondNeighbouredBuildingNumber = 24;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 25:
                    firstNeighbouredBuildingNumber = 26;
                    secondNeighbouredBuildingNumber = 5;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 26:
                    firstNeighbouredBuildingNumber = 27;
                    secondNeighbouredBuildingNumber = 25;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 27:
                    firstNeighbouredBuildingNumber = 18;
                    secondNeighbouredBuildingNumber = 26;
                    thirdNeighbouredBuildingNumber = 28;
                    break;
                case 28:
                    firstNeighbouredBuildingNumber = 29;
                    secondNeighbouredBuildingNumber = 27;
                    thirdNeighbouredBuildingNumber = 6;
                    break;
                case 29:
                    firstNeighbouredBuildingNumber = 20;
                    secondNeighbouredBuildingNumber = 28;
                    thirdNeighbouredBuildingNumber = 30;
                    break;
                case 30:
                    firstNeighbouredBuildingNumber = 31;
                    secondNeighbouredBuildingNumber = 29;
                    thirdNeighbouredBuildingNumber = 8;
                    break;
                case 31:
                    firstNeighbouredBuildingNumber = 22;
                    secondNeighbouredBuildingNumber = 30;
                    thirdNeighbouredBuildingNumber = 32;
                    break;
                case 32:
                    firstNeighbouredBuildingNumber = 33;
                    secondNeighbouredBuildingNumber = 31;
                    thirdNeighbouredBuildingNumber = 36;
                    break;
                case 33:
                    firstNeighbouredBuildingNumber = 9;
                    secondNeighbouredBuildingNumber = 32;
                    thirdNeighbouredBuildingNumber = 3;
                    break;
                case 34:
                    firstNeighbouredBuildingNumber = 24;
                    secondNeighbouredBuildingNumber = 3;
                    thirdNeighbouredBuildingNumber = 1;
                    break;
                case 35:
                    firstNeighbouredBuildingNumber = 36;
                    secondNeighbouredBuildingNumber = 8;
                    thirdNeighbouredBuildingNumber = 47;
                    break;
                case 36:
                    firstNeighbouredBuildingNumber = 32;
                    secondNeighbouredBuildingNumber = 35;
                    thirdNeighbouredBuildingNumber = 40;
                    break;
                case 37:
                    firstNeighbouredBuildingNumber = 3;
                    secondNeighbouredBuildingNumber = 40;
                    thirdNeighbouredBuildingNumber = 38;
                    break;
                case 38:
                    firstNeighbouredBuildingNumber = 2;
                    secondNeighbouredBuildingNumber = 37;
                    thirdNeighbouredBuildingNumber = 39;
                    break;
                case 39:
                    firstNeighbouredBuildingNumber = 38;
                    secondNeighbouredBuildingNumber = 53;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 40:
                    firstNeighbouredBuildingNumber = 37;
                    secondNeighbouredBuildingNumber = 36;
                    thirdNeighbouredBuildingNumber = 52;
                    break;
                case 41:
                    firstNeighbouredBuildingNumber = 46;
                    secondNeighbouredBuildingNumber = 42;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 42:
                    firstNeighbouredBuildingNumber = 43;
                    secondNeighbouredBuildingNumber = 41;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 43:
                    firstNeighbouredBuildingNumber = 44;
                    secondNeighbouredBuildingNumber = 4;
                    thirdNeighbouredBuildingNumber = 42;
                    break;
                case 44:
                    firstNeighbouredBuildingNumber = 7;
                    secondNeighbouredBuildingNumber = 43;
                    thirdNeighbouredBuildingNumber = 45;
                    break;
                case 45:
                    firstNeighbouredBuildingNumber = 47;
                    secondNeighbouredBuildingNumber = 44;
                    thirdNeighbouredBuildingNumber = 46;
                    break;
                case 46:
                    firstNeighbouredBuildingNumber = 45;
                    secondNeighbouredBuildingNumber = 41;
                    thirdNeighbouredBuildingNumber = 48;
                    break;
                case 47:
                    firstNeighbouredBuildingNumber = 35;
                    secondNeighbouredBuildingNumber = 45;
                    thirdNeighbouredBuildingNumber = 51;
                    break;
                case 48:
                    firstNeighbouredBuildingNumber = 50;
                    secondNeighbouredBuildingNumber = 46;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 49:
                    firstNeighbouredBuildingNumber = 0;
                    secondNeighbouredBuildingNumber = 50;
                    thirdNeighbouredBuildingNumber = null;
                    break;
                case 50:
                    firstNeighbouredBuildingNumber = 51;
                    secondNeighbouredBuildingNumber = 48;
                    thirdNeighbouredBuildingNumber = 49;
                    break;
                case 51:
                    firstNeighbouredBuildingNumber = 52;
                    secondNeighbouredBuildingNumber = 47;
                    thirdNeighbouredBuildingNumber = 50;
                    break;
                case 52:
                    firstNeighbouredBuildingNumber = 40;
                    secondNeighbouredBuildingNumber = 51;
                    thirdNeighbouredBuildingNumber = 53;
                    break;
                case 53:
                    firstNeighbouredBuildingNumber = 39;
                    secondNeighbouredBuildingNumber = 52;
                    thirdNeighbouredBuildingNumber = 0;
                    break;
            }

            List<GameObject> neighbouredGameObjects = new List<GameObject>();

            //Add neighboured street GameObjects to list and return it
            if (firstNeighbouredBuildingNumber != null)
            {
                neighbouredGameObjects.Add(Game.GetGameObjectByName($"BuildingPlacement{firstNeighbouredBuildingNumber}"));
            }

            if (secondNeighbouredBuildingNumber != null)
            {
                neighbouredGameObjects.Add(Game.GetGameObjectByName($"BuildingPlacement{secondNeighbouredBuildingNumber}"));
            }

            if (thirdNeighbouredBuildingNumber != null)
            {
                neighbouredGameObjects.Add(Game.GetGameObjectByName($"BuildingPlacement{thirdNeighbouredBuildingNumber}"));
            }

            return neighbouredGameObjects;
        }

        /// <summary>
        /// Returns a list of all street gameobjects that are located next to a street
        /// </summary>
        /// <param name="streetNumber"></param>
        /// <returns></returns>
        public static List<GameObject> GetNeighbouredStreetGOsOfStreet(int streetNumber)
        {
            int? firstStreetNumber = null;
            int? secondStreetNumber = null;
            int? thirdStreetNumber = null;
            int? fourthStreetNumber = null;

            switch (streetNumber)
            {
                case 0:
                    firstStreetNumber = 71;
                    secondStreetNumber = 67;
                    thirdStreetNumber = null;
                    fourthStreetNumber = null;
                    break;
                case 1:
                    firstStreetNumber = 20;
                    secondStreetNumber = 33;
                    thirdStreetNumber = 2;
                    fourthStreetNumber = 3;
                    break;
                case 2:
                    firstStreetNumber = 17;
                    secondStreetNumber = 4;
                    thirdStreetNumber = 1;
                    fourthStreetNumber = 3;
                    break;
                case 3:
                    firstStreetNumber = 1;
                    secondStreetNumber = 2;
                    thirdStreetNumber = 35;
                    fourthStreetNumber = 36;
                    break;
                case 4:
                    firstStreetNumber = 17;
                    secondStreetNumber = 2;
                    thirdStreetNumber = 5;
                    fourthStreetNumber = null;
                    break;
                case 5:
                    firstStreetNumber = 4;
                    secondStreetNumber = 38;
                    thirdStreetNumber = 39;
                    fourthStreetNumber = null;
                    break;
                case 6:
                    firstStreetNumber = 7;
                    secondStreetNumber = 43;
                    thirdStreetNumber = 44;
                    fourthStreetNumber = null;
                    break;
                case 7:
                    firstStreetNumber = 6;
                    secondStreetNumber = 8;
                    thirdStreetNumber = null;
                    fourthStreetNumber = null;
                    break;
                case 8:
                    firstStreetNumber = 30;
                    secondStreetNumber = 9;
                    thirdStreetNumber = 7;
                    fourthStreetNumber = null;
                    break;
                case 9:
                    firstStreetNumber = 10;
                    secondStreetNumber = 11;
                    thirdStreetNumber = 30;
                    fourthStreetNumber = 8;
                    break;
                case 10:
                    firstStreetNumber = 11;
                    secondStreetNumber = 9;
                    thirdStreetNumber = 44;
                    fourthStreetNumber = 45;
                    break;
                case 11:
                    firstStreetNumber = 27;
                    secondStreetNumber = 12;
                    thirdStreetNumber = 9;
                    fourthStreetNumber = 10;
                    break;
                case 12:
                    firstStreetNumber = 13;
                    secondStreetNumber = 14;
                    thirdStreetNumber = 27;
                    fourthStreetNumber = 11;
                    break;
                case 13:
                    firstStreetNumber = 12;
                    secondStreetNumber = 14;
                    thirdStreetNumber = 47;
                    fourthStreetNumber = 48;
                    break;
                case 14:
                    firstStreetNumber = 32;
                    secondStreetNumber = 15;
                    thirdStreetNumber = 12;
                    fourthStreetNumber = 13;
                    break;
                case 15:
                    firstStreetNumber = 34;
                    secondStreetNumber = 35;
                    thirdStreetNumber = 32;
                    fourthStreetNumber = 14;
                    break;
                case 16:
                    firstStreetNumber = 23;
                    secondStreetNumber = 31;
                    thirdStreetNumber = 28;
                    fourthStreetNumber = 29;
                    break;
                case 17:
                    firstStreetNumber = 18;
                    secondStreetNumber = 4;
                    thirdStreetNumber = 2;
                    fourthStreetNumber = null;
                    break;
                case 18:
                    firstStreetNumber = 17;
                    secondStreetNumber = 19;
                    thirdStreetNumber = null;
                    fourthStreetNumber = null;
                    break;
                case 19:
                    firstStreetNumber = 18;
                    secondStreetNumber = 20;
                    thirdStreetNumber = 21;
                    fourthStreetNumber = null;
                    break;
                case 20:
                    firstStreetNumber = 19;
                    secondStreetNumber = 21;
                    thirdStreetNumber = 1;
                    fourthStreetNumber = 33;
                    break;
                case 21:
                    firstStreetNumber = 19;
                    secondStreetNumber = 20;
                    thirdStreetNumber = 22;
                    fourthStreetNumber = null;
                    break;
                case 22:
                    firstStreetNumber = 21;
                    secondStreetNumber = 23;
                    thirdStreetNumber = 24;
                    fourthStreetNumber = null;
                    break;
                case 23:
                    firstStreetNumber = 22;
                    secondStreetNumber = 24;
                    thirdStreetNumber = 31;
                    fourthStreetNumber = 16;
                    break;
                case 24:
                    firstStreetNumber = 22;
                    secondStreetNumber = 23;
                    thirdStreetNumber = 25;
                    fourthStreetNumber = null;
                    break;
                case 25:
                    firstStreetNumber = 24;
                    secondStreetNumber = 26;
                    thirdStreetNumber = null;
                    fourthStreetNumber = null;
                    break;
                case 26:
                    firstStreetNumber = 25;
                    secondStreetNumber = 28;
                    thirdStreetNumber = 29;
                    fourthStreetNumber = null;
                    break;
                case 27:
                    firstStreetNumber = 16;
                    secondStreetNumber = 28;
                    thirdStreetNumber = 12;
                    fourthStreetNumber = 11;
                    break;
                case 28:
                    firstStreetNumber = 16;
                    secondStreetNumber = 27;
                    thirdStreetNumber = 26;
                    fourthStreetNumber = 29;
                    break;
                case 29:
                    firstStreetNumber = 26;
                    secondStreetNumber = 28;
                    thirdStreetNumber = 30;
                    fourthStreetNumber = null;
                    break;
                case 30:
                    firstStreetNumber = 29;
                    secondStreetNumber = 9;
                    thirdStreetNumber = 8;
                    fourthStreetNumber = null;
                    break;
                case 31:
                    firstStreetNumber = 32;
                    secondStreetNumber = 33;
                    thirdStreetNumber = 23;
                    fourthStreetNumber = 16;
                    break;
                case 32:
                    firstStreetNumber = 33;
                    secondStreetNumber = 31;
                    thirdStreetNumber = 14;
                    fourthStreetNumber = 15;
                    break;
                case 33:
                    firstStreetNumber = 20;
                    secondStreetNumber = 1;
                    thirdStreetNumber = 31;
                    fourthStreetNumber = 32;
                    break;
                case 34:
                    firstStreetNumber = 35;
                    secondStreetNumber = 15;
                    thirdStreetNumber = 50;
                    fourthStreetNumber = 51;
                    break;
                case 35:
                    firstStreetNumber = 3;
                    secondStreetNumber = 36;
                    thirdStreetNumber = 15;
                    fourthStreetNumber = 34;
                    break;
                case 36:
                    firstStreetNumber = 38;
                    secondStreetNumber = 37;
                    thirdStreetNumber = 3;
                    fourthStreetNumber = 35;
                    break;
                case 37:
                    firstStreetNumber = 38;
                    secondStreetNumber = 36;
                    thirdStreetNumber = 53;
                    fourthStreetNumber = 54;
                    break;
                case 38:
                    firstStreetNumber = 5;
                    secondStreetNumber = 39;
                    thirdStreetNumber = 36;
                    fourthStreetNumber = 37;
                    break;
                case 39:
                    firstStreetNumber = 40;
                    secondStreetNumber = 5;
                    thirdStreetNumber = 38;
                    fourthStreetNumber = null;
                    break;
                case 40:
                    firstStreetNumber = 39;
                    secondStreetNumber = 41;
                    thirdStreetNumber = null;
                    fourthStreetNumber = null;
                    break;
                case 41:
                    firstStreetNumber = 40;
                    secondStreetNumber = 54;
                    thirdStreetNumber = 55;
                    fourthStreetNumber = null;
                    break;
                case 42:
                    firstStreetNumber = 60;
                    secondStreetNumber = 57;
                    thirdStreetNumber = 43;
                    fourthStreetNumber = null;
                    break;
                case 43:
                    firstStreetNumber = 44;
                    secondStreetNumber = 6;
                    thirdStreetNumber = 42;
                    fourthStreetNumber = null;
                    break;
                case 44:
                    firstStreetNumber = 10;
                    secondStreetNumber = 45;
                    thirdStreetNumber = 6;
                    fourthStreetNumber = 43;
                    break;
                case 45:
                    firstStreetNumber = 46;
                    secondStreetNumber = 47;
                    thirdStreetNumber = 10;
                    fourthStreetNumber = 44;
                    break;
                case 46:
                    firstStreetNumber = 47;
                    secondStreetNumber = 45;
                    thirdStreetNumber = 60;
                    fourthStreetNumber = 61;
                    break;
                case 47:
                    firstStreetNumber = 13;
                    secondStreetNumber = 48;
                    thirdStreetNumber = 45;
                    fourthStreetNumber = 46;
                    break;
                case 48:
                    firstStreetNumber = 49;
                    secondStreetNumber = 50;
                    thirdStreetNumber = 13;
                    fourthStreetNumber = 47;
                    break;
                case 49:
                    firstStreetNumber = 50;
                    secondStreetNumber = 48;
                    thirdStreetNumber = 66;
                    fourthStreetNumber = 64;
                    break;
                case 50:
                    firstStreetNumber = 34;
                    secondStreetNumber = 51;
                    thirdStreetNumber = 48;
                    fourthStreetNumber = 49;
                    break;
                case 51:
                    firstStreetNumber = 52;
                    secondStreetNumber = 53;
                    thirdStreetNumber = 34;
                    fourthStreetNumber = 50;
                    break;
                case 52:
                    firstStreetNumber = 53;
                    secondStreetNumber = 51;
                    thirdStreetNumber = 70;
                    fourthStreetNumber = 69;
                    break;
                case 53:
                    firstStreetNumber = 37;
                    secondStreetNumber = 54;
                    thirdStreetNumber = 51;
                    fourthStreetNumber = 52;
                    break;
                case 54:
                    firstStreetNumber = 41;
                    secondStreetNumber = 55;
                    thirdStreetNumber = 37;
                    fourthStreetNumber = 53;
                    break;
                case 55:
                    firstStreetNumber = 41;
                    secondStreetNumber = 54;
                    thirdStreetNumber = 56;
                    fourthStreetNumber = null;
                    break;
                case 56:
                    firstStreetNumber = 55;
                    secondStreetNumber = 70;
                    thirdStreetNumber = 71;
                    fourthStreetNumber = null;
                    break;
                case 57:
                    firstStreetNumber = 60;
                    secondStreetNumber = 42;
                    thirdStreetNumber = 58;
                    fourthStreetNumber = null;
                    break;
                case 58:
                    firstStreetNumber = 59;
                    secondStreetNumber = 57;
                    thirdStreetNumber = null;
                    fourthStreetNumber = null;
                    break;
                case 59:
                    firstStreetNumber = 62;
                    secondStreetNumber = 63;
                    thirdStreetNumber = 58;
                    fourthStreetNumber = null;
                    break;
                case 60:
                    firstStreetNumber = 46;
                    secondStreetNumber = 61;
                    thirdStreetNumber = 42;
                    fourthStreetNumber = 57;
                    break;
                case 61:
                    firstStreetNumber = 63;
                    secondStreetNumber = 64;
                    thirdStreetNumber = 46;
                    fourthStreetNumber = 60;
                    break;
                case 62:
                    firstStreetNumber = 65;
                    secondStreetNumber = 63;
                    thirdStreetNumber = 59;
                    fourthStreetNumber = null;
                    break;
                case 63:
                    firstStreetNumber = 64;
                    secondStreetNumber = 61;
                    thirdStreetNumber = 62;
                    fourthStreetNumber = 59;
                    break;
                case 64:
                    firstStreetNumber = 49;
                    secondStreetNumber = 66;
                    thirdStreetNumber = 61;
                    fourthStreetNumber = 63;
                    break;
                case 65:
                    firstStreetNumber = 67;
                    secondStreetNumber = 68;
                    thirdStreetNumber = 62;
                    fourthStreetNumber = null;
                    break;
                case 66:
                    firstStreetNumber = 68;
                    secondStreetNumber = 69;
                    thirdStreetNumber = 49;
                    fourthStreetNumber = 64;
                    break;
                case 67:
                    firstStreetNumber = 0;
                    secondStreetNumber = 68;
                    thirdStreetNumber = 65;
                    fourthStreetNumber = null;
                    break;
                case 68:
                    firstStreetNumber = 69;
                    secondStreetNumber = 66;
                    thirdStreetNumber = 67;
                    fourthStreetNumber = 65;
                    break;
                case 69:
                    firstStreetNumber = 52;
                    secondStreetNumber = 70;
                    thirdStreetNumber = 66;
                    fourthStreetNumber = 68;
                    break;
                case 70:
                    firstStreetNumber = 71;
                    secondStreetNumber = 56;
                    thirdStreetNumber = 52;
                    fourthStreetNumber = 69;
                    break;
                case 71:
                    firstStreetNumber = 56;
                    secondStreetNumber = 70;
                    thirdStreetNumber = 0;
                    fourthStreetNumber = null;
                    break;
            }

            List<GameObject> neighbouredGameObjects = new();

            //Add neighboured street GameObjects to list and return it
            if (firstStreetNumber != null)
            {
                neighbouredGameObjects.Add(Game.GetGameObjectByName($"StreetPlacement{firstStreetNumber}"));
            }

            if (secondStreetNumber != null)
            {
                neighbouredGameObjects.Add(Game.GetGameObjectByName($"StreetPlacement{secondStreetNumber}"));
            }

            if (thirdStreetNumber != null)
            {
                neighbouredGameObjects.Add(Game.GetGameObjectByName($"StreetPlacement{thirdStreetNumber}"));
            }

            if (fourthStreetNumber != null)
            {
                neighbouredGameObjects.Add(Game.GetGameObjectByName($"StreetPlacement{fourthStreetNumber}"));
            }

            return neighbouredGameObjects;
        }

        /// <summary>
        /// Returns a list of all building gameobjects that are located next to a street
        /// </summary>
        /// <param name="streetNumber"></param>
        /// <returns></returns>
        public static List<GameObject> GetNeighbouredBuildingGOsOfStreet(int streetNumber)
        {
            List<GameObject> neighbouredBuidlings = new();

            foreach (GameObject building in Game.Instance.AvailableBuildingPlacements)
            {
                //When a available buidling has a neighboured street with the street number --> add building to list
                foreach (GameObject street in Game.GetNeighbouredStreetGOsOfBuilding(Game.GetNumberFromString(building.name)))
                {
                    if (Game.GetNumberFromString(street.name) == streetNumber)
                    {
                        neighbouredBuidlings.Add(building);
                    }
                }
            }

            return neighbouredBuidlings;
        }

        public void PrintGameStatus()
        {
            string debugLogFilePath = "C:/Users/anton/Pictures/Catan/DebugLogFile.txt";
            string debugLogText = $"GameStatus -- {DateTime.Now}";

            if (System.IO.File.Exists(debugLogFilePath))
            {
                Process.Start(debugLogFilePath);
            }

            foreach (Player player in this.AllPlayers)
            {
                debugLogText += "\n\n";
                debugLogText += player.IsAIPlayer ? "AIPlayer Resources:" : "Your resources:";

                foreach (FieldType ft in player.Resources.Keys)
                {
                    debugLogText += $"\n{ft}: {player.Resources[ft]}";
                }

                debugLogText += "\n\n";
            }

            System.IO.File.WriteAllText(debugLogFilePath, debugLogText);
        }

    }
}
