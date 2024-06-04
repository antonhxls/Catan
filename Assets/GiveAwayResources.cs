using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GiveAwayResources : MonoBehaviour
{
    public static GameObject GiveAwayResourcePanel;

    public static int AmountToGiveAway = 0;

    public static int AmountSelected = 0;

    public static bool ResourcesNeedToBeGivenAway = false;

    public enum AddRemoveMethod
    {
        Add,
        Remove,
    }

    // Start is called before the first frame update
    void Start()
    {
        GiveAwayResources.GiveAwayResourcePanel = Game.GetGameObjectByName("Canvas").transform.Find("GiveAwayResourcePanel").gameObject;
        GiveAwayResources.GiveAwayResourcePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void ShowGiveAwayResourcePanel()
    {
        GiveAwayResources.GiveAwayResourcePanel.SetActive(true);
        GiveAwayResources.AmountSelected = 0;
        GiveAwayResources.ResourcesNeedToBeGivenAway = true; 
        GiveAwayResources.GiveAwayResourcePanel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"Resources to give away: {GiveAwayResources.AmountSelected}/{GiveAwayResources.AmountToGiveAway}:";


        foreach (FieldType field in Game.Instance.CurrentPlayer.Resources.Keys)
        {
           GiveAwayResources.GiveAwayResourcePanel.transform.Find($"{field}Image").transform.Find("Amount").GetComponent<TextMeshProUGUI>().text = "0x";
        }
    }

    #region ButtonCLick Methods

    public void AddWood()
    {
        this.AddOrRemoveResource(AddRemoveMethod.Add, FieldType.Wood);
    }
    public void RemoveWood()
    {
        this.AddOrRemoveResource(AddRemoveMethod.Remove, FieldType.Wood);
    }

    public void AddBrick()
    {
        this.AddOrRemoveResource(AddRemoveMethod.Add, FieldType.Brick);
    }

    public void RemoveBrick()
    {
        this.AddOrRemoveResource(AddRemoveMethod.Remove, FieldType.Brick);
    }

    public void AddSheep()
    {
        this.AddOrRemoveResource(AddRemoveMethod.Add, FieldType.Sheep);
    }

    public void RemoveSheep()
    {
        this.AddOrRemoveResource(AddRemoveMethod.Remove, FieldType.Sheep);
    }

    public void AddWheat()
    {
        this.AddOrRemoveResource(AddRemoveMethod.Add, FieldType.Wheat);
    }

    public void RemoveWheat()
    {
        this.AddOrRemoveResource(AddRemoveMethod.Remove, FieldType.Wheat);
    }

    public void AddOre()
    {
        this.AddOrRemoveResource(AddRemoveMethod.Add, FieldType.Ore);
    }

    public void RemoveOre()
    {
        this.AddOrRemoveResource(AddRemoveMethod.Remove, FieldType.Ore);
    }

    #endregion

    public void AddOrRemoveResource(AddRemoveMethod method, FieldType resource)
    {
        TextMeshProUGUI tmp = GiveAwayResources.GiveAwayResourcePanel.transform.Find($"{resource}Image").transform.Find("Amount").GetComponent<TextMeshProUGUI>();
        int currentAmount = Game.GetNumberFromString(tmp.text);

        if (currentAmount == 0 && method == AddRemoveMethod.Remove)
        {
            return;
        }

        if (currentAmount == Game.Instance.AllPlayers.Where(x => x.IsAIPlayer == false).FirstOrDefault().Resources[resource] && method == AddRemoveMethod.Add)
        {
            Game.Instance.ShowInfoMessage("You do not have that many resources", 1500);
            return;
        }

        if (method == AddRemoveMethod.Add)
        {
            currentAmount++;
            GiveAwayResources.AmountSelected++;
        }
        else if (method == AddRemoveMethod.Remove)
        {
            currentAmount--;
            GiveAwayResources.AmountSelected--;
        }

        tmp.text = $"{currentAmount}x";

        GiveAwayResources.GiveAwayResourcePanel.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"Resources to give away: {GiveAwayResources.AmountSelected}/{GiveAwayResources.AmountToGiveAway}:";
    }

    public void ConfirmGiveAwayResources()
    {
        if (GiveAwayResources.AmountSelected < GiveAwayResources.AmountToGiveAway)
        {
            Game.Instance.ShowInfoMessage("Not enough resources selected!", 2000);
            return;
        }

        if (GiveAwayResources.AmountSelected > GiveAwayResources.AmountToGiveAway)
        {
            Game.Instance.ShowInfoMessage("Selected more resources than needed!", 2000);
            return;
        }

        Player player = Game.Instance.AllPlayers.Where(x => x.IsAIPlayer == false).FirstOrDefault();
        List<FieldType> fieldTypes = player.Resources.Keys.ToList();

        foreach (FieldType field in fieldTypes)
        {
            int amount = Game.GetNumberFromString(GiveAwayResources.GiveAwayResourcePanel.transform.Find($"{field}Image").transform.Find("Amount").GetComponent<TextMeshProUGUI>().text);

            player.Resources[field] -= amount;
        }

        player.RefreshDisplayedResourceAmount();

        GiveAwayResources.GiveAwayResourcePanel.SetActive(false);
        GiveAwayResources.ResourcesNeedToBeGivenAway = false;
    }

}
