using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseGameObject : MonoBehaviour
{
    public static GameObject HouseToUpgrade { get; set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Check if player clicked a house gameobject
    void Update()
    {
        // Überprüfe, ob die linke Maustaste gedrückt wurde
        if (Input.GetMouseButtonDown(0))
        {
            // Erzeuge einen Ray von der Kamera durch den Mauszeiger
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Überprüfe, ob der Ray ein GameObject trifft
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Überprüfe, ob das getroffene GameObject das ist, auf das reagiert werden soll
                if (hit.collider.gameObject == gameObject)
                {
                    this.OnHouseClicked();
                }
            }
        }
    }

    /// <summary>
    /// Triggered when a player clicks on a house<br/>
    /// Allows the player to upgrade his house to a Veste
    /// </summary>
    public void OnHouseClicked()
    {
        if (Game.Instance.CurrentPlayer.IsAIPlayer == true)
        {
            //Do not allow the player to upgrade to Veste when it is not his turn
            return;
        }

        if (Game.Instance.CurrentPlayer.OwnedBuildings.Contains(this.gameObject) == false)
        {
            //Do not allow player to upgrade another players house
            return;
        }

        if (Game.Instance.PlayersHavePlacedTheirFirstTwoHouses == false || GiveAwayResources.GiveAwayResourcePanel.activeSelf == true)
        {
            //Cant upgrade to Veste on start or when giving away resources
            return;
        }

        //Remember which house has to be upgraded
        HouseGameObject.HouseToUpgrade = this.gameObject;

        //Show upgrade panel
        Game.Instance.UpgradeToVestePanel.SetActive(true);
        Game.HideUIButtons();
    }

    public void ButtonPressed()
    {
        Game.ShowUIButtons();
        //Hide upgrade panel
        Game.Instance.UpgradeToVestePanel.SetActive(false);
    }

    /// <summary>
    /// Upgrades a house to a Veste
    /// </summary>
    public static void UpgradeToVeste()
    {
        //Hide upgrade panel
        Game.Instance.UpgradeToVestePanel.SetActive(false);
        Game.ShowUIButtons();

        if (Game.Instance.CurrentPlayer.Resources[FieldType.Wheat] < 2 || Game.Instance.CurrentPlayer.Resources[FieldType.Ore] < 3)
        {
            //Player does not have enough materials to upgrade to Veste
            Game.Instance.ShowInfoMessage("You do not have enough resources to upgrade to Veste", 2000);
            return;
        }

        //Get Veste gameobject
        GameObject veste = Game.GetGameObjectByName($"VesteGameObject{Game.GetNumberFromString(HouseGameObject.HouseToUpgrade.name)}");

        //Set Veste position
        veste.transform.position = HouseGameObject.HouseToUpgrade.transform.position;

        //Colorize veste
        ColorizeVeste(veste);

        //Show veste
        veste.SetActive(true);

        //Override field-buildings to veste
        foreach (Field field in Game.Instance.AllFields)
        {
            if (field.TopBuildingGameObject == HouseGameObject.HouseToUpgrade)
            {
                field.TopBuildingGameObject = veste;
            }

            if (field.TopRightBuildingGameObject == HouseGameObject.HouseToUpgrade)
            {
                field.TopRightBuildingGameObject = veste;
            }

            if (field.BottomRightBuildingGameObject == HouseGameObject.HouseToUpgrade)
            {
                field.BottomRightBuildingGameObject = veste;
            }

            if (field.BottomBuildingGameObject == HouseGameObject.HouseToUpgrade)
            {
                field.BottomBuildingGameObject = veste;
            }

            if (field.BottomLeftBuildingGameObject == HouseGameObject.HouseToUpgrade)
            {
                field.BottomLeftBuildingGameObject = veste;
            }

            if (field.TopLeftBuildingGameObject == HouseGameObject.HouseToUpgrade)
            {
                field.TopLeftBuildingGameObject = veste;
            }
        }

        //Remove upgraded house from players owned buildings
        Game.Instance.CurrentPlayer.OwnedBuildings.Remove(HouseGameObject.HouseToUpgrade);

        //Add veste to owned buildings
        Game.Instance.CurrentPlayer.OwnedBuildings.Add(veste);

        //Remove resources
        if (Game.Instance.PlayersHavePlacedTheirFirstTwoHouses == true)
        {
            Game.Instance.CurrentPlayer.Resources[FieldType.Ore] -= 3;
            Game.Instance.CurrentPlayer.Resources[FieldType.Wheat] -= 2;
            Game.Instance.CurrentPlayer.RefreshDisplayedResourceAmount();
        }

        Game.Instance.CurrentPlayer.IncreaseVictoryPoints();

        //Hide upgraded house
        HouseGameObject.HouseToUpgrade.SetActive(false);
    }

    /// <summary>
    /// Colorizes the Veste to the players material
    /// </summary>
    /// <param name="veste"></param>
    public static void ColorizeVeste(GameObject veste)
    {
        // Zugriff auf alle Kindelemente des Prefabs
        Transform[] childTransforms = veste.GetComponentsInChildren<Transform>();

        // Durchlaufe alle Kindelemente und weise das neue Material zu
        foreach (Transform child in childTransforms)
        {
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();

            if (renderer == null)
            {
                UnityEngine.Debug.Log($"{child.name} doesnt have a renderer");
                renderer = child.gameObject.AddComponent<MeshRenderer>();
            }

            renderer.material = Game.Instance.CurrentPlayer.Material;
        }
    }
}
