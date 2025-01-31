using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main_Menu_Control : MonoBehaviour
{

    public Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        startButton = GameObject.Find("Start_Button").GetComponent<Button>();

        startButton.onClick.AddListener(loadGenerator);
    }

    private void loadGenerator() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("S_Generator");
    }
}
