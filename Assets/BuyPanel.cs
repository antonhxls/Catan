using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuyButtonPressed()
    {
        if (GiveAwayResources.GiveAwayResourcePanel.activeSelf == true)
        {
            return;
        }

        foreach (GameObject street in Game.Instance.AvailableStreetPlacements)
        {
            street.SetActive(false);
        }

        foreach (GameObject building in Game.Instance.AvailableBuildingPlacements)
        {
            building.SetActive(false);
        }

        Game.Instance.BuyPanel.SetActive(true);
        Game.HideUIButtons();
    }

    public void CloseButtonPressed()
    {
        Game.Instance.BuyPanel.SetActive(false);
        Game.ShowUIButtons();
    }

    public void BuyHouseButtonPressed()
    {
        Game.Instance.BuyPanel.SetActive(false);
        Game.ShowUIButtons();

        if (Game.Instance.CurrentPlayer.Resources[FieldType.Brick] < 1 || Game.Instance.CurrentPlayer.Resources[FieldType.Wood] < 1 || Game.Instance.CurrentPlayer.Resources[FieldType.Wheat] < 1 || Game.Instance.CurrentPlayer.Resources[FieldType.Sheep] < 1)
        {
            Game.Instance.ShowInfoMessage("You do not have enough resources to buy a settlement", 2000);
            return;
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

        //Make houses visible
        foreach (GameObject street in possibleHouses)
        {
            street.SetActive(true);
        }
    }

    public void BuyStreetButtonPressed()
    {
        Game.Instance.BuyPanel.SetActive(false);
        Game.ShowUIButtons();

        if (Game.Instance.CurrentPlayer.Resources[FieldType.Brick] < 1 || Game.Instance.CurrentPlayer.Resources[FieldType.Wood] < 1)
        {
            Game.Instance.ShowInfoMessage("You do not have enough resources to buy a street", 2000);
            return;
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

        //Make streets visible
        foreach (GameObject street in possibleStreets)
        {
            street.SetActive(true);
        }
    }
}
