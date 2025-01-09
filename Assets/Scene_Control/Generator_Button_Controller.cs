using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Generator_Button_Controller {

    public bool menuOpen;

    public Button exitButton;
    public Button Terrain_Settings;

    GameObject Terrain_Settings_Container;

    // Start is called before the first frame update
    public Generator_Button_Controller(GameObject terrainSettingsUI)
    {
        Terrain_Settings_Container = terrainSettingsUI;
        menuOpen = false;

        exitButton = GameObject.Find("Exit").GetComponent<Button>();
        Terrain_Settings = GameObject.Find("Terrain_Settings").GetComponent<Button>();

        exitButton.onClick.AddListener(exitGenerator);
        Terrain_Settings.onClick.AddListener(toggleTerrainSettings);
    }

    private void exitGenerator() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("S_Start");
    }

    private void toggleTerrainSettings() {
        if(Terrain_Settings_Container.activeSelf) {
            Terrain_Settings_Container.GetComponent<Image>().enabled = false;
            Terrain_Settings_Container.SetActive(false);
            menuOpen = false;
        } else {
            Terrain_Settings_Container.GetComponent<Image>().enabled = true;
            Terrain_Settings_Container.SetActive(true);
            menuOpen = true;
        }
    }
}
