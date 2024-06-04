using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Assets;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{

    public void ResetGameButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitButtonPressed()
    {
        // Spiel beenden
        Application.Quit();

        // Hinweis für den Editor, da Application.Quit() dort nicht funktioniert
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void StartGameButtonPressed()
    {
        Game.GetGameObjectByName("Canvas").transform.Find("GameStartPanel").gameObject.SetActive(false);

        Game.Instance.Initialize();
    }

    public void StartTestGameButtonPressed()
    {
        Game.GetGameObjectByName("Canvas").transform.Find("GameStartPanel").gameObject.SetActive(false);

        Game.Instance.Initialize(isTestGame: true);
    }

    public void ShowQuickGuideButtonPressed()
    {
        Game.Instance.QuickGuidePanel.SetActive(true);
        Game.Instance.StartGamePanel.SetActive(false);
    }

    public void HideQuickGuideButtonPressed()
    {
        Game.Instance.QuickGuidePanel.SetActive(false);
        Game.Instance.StartGamePanel.SetActive(true);
    }

    public void OpenSettings()
    {
        Game.Instance.IsSettingsPanelOpen = true;
        Game.Instance.SettingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        Game.Instance.IsSettingsPanelOpen = false;
        Game.Instance.SettingsPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
