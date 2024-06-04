using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Trading : MonoBehaviour
{
    public static TMP_Dropdown GiveAwayCombobox = null;
    public static TMP_Dropdown GetCombobox = null;

    // Start is called before the first frame update
    void Start()
    {
        Trading.GiveAwayCombobox = Game.GetGameObjectByName("Canvas").transform.Find("TradePanel").transform.Find("GiveAwayDropDown").gameObject.GetComponent<TMP_Dropdown>();
        Trading.GiveAwayCombobox.onValueChanged.AddListener(delegate { GiveAwayDropdownItemSelected(); });
        Trading.GetCombobox = Game.GetGameObjectByName("Canvas").transform.Find("TradePanel").transform.Find("GetDropDown").gameObject.GetComponent<TMP_Dropdown>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenTradingMenu()
    {
        if (GiveAwayResources.GiveAwayResourcePanel.activeSelf == true)
        {
            return;
        }

        //Clear possible resources to give away
        Trading.GiveAwayCombobox.options.Clear();

        //Add all resources the player has at least 4 of
        foreach (FieldType type in Game.Instance.CurrentPlayer.Resources.Keys)
        {
            if (Game.Instance.CurrentPlayer.Resources[type] >= 4)
            {
                GiveAwayCombobox.options.Add(new TMP_Dropdown.OptionData(type.ToString()));
            }
        }

        if (GiveAwayCombobox.options.Count == 0)
        {
            Game.Instance.ShowInfoMessage("There is no resource the player has 4 of - can not trade", 3000);
            return;
        }
        Trading.GiveAwayCombobox.value = 0;
        Trading.GiveAwayCombobox.captionText.text = Trading.GiveAwayCombobox.options[0].text;

        this.GiveAwayDropdownItemSelected();

        Game.HideUIButtons();
        Game.Instance.TradePanel.SetActive(true);
    }

    public void CloseTradingMenu()
    {
        Game.ShowUIButtons();
        Game.Instance.TradePanel.SetActive(false);
    }

    public void ConfirmTrade()
    {
        Enum.TryParse(GiveAwayCombobox.options[GiveAwayCombobox.value].text, out FieldType giveAwaySelection);
        Enum.TryParse(GetCombobox.options[GetCombobox.value].text, out FieldType getSelection);

        //Remove resource that have been given away
        Game.Instance.CurrentPlayer.Resources[giveAwaySelection] -= 4;
        Game.Instance.CurrentPlayer.Resources[getSelection] += 1;

        //Refresh displayed resource amounts
        Game.Instance.CurrentPlayer.RefreshDisplayedResourceAmount();

        //Close trade panel
        Game.ShowUIButtons();
        Game.Instance.TradePanel.SetActive(false);
    }

    /// <summary>
    /// Executed when the player selects an item in the giveawaydropdown --> can trade f.e. 4 wood against 1 wood
    /// </summary>
    /// <param></param>
    private void GiveAwayDropdownItemSelected()
    {
        UnityEngine.Debug.Log("GetDropDown content adjusted");

        Trading.GetCombobox.options.Clear();

        foreach (FieldType field in Game.Instance.CurrentPlayer.Resources.Keys)
        {
            if (Trading.GiveAwayCombobox.options[GiveAwayCombobox.value].text != field.ToString())
            {
                Trading.GetCombobox.options.Add(new TMP_Dropdown.OptionData(field.ToString()));
            }
        }

        Trading.GetCombobox.value = 0;
        Trading.GetCombobox.captionText.text = Trading.GetCombobox.options[0].text;
    }
}
