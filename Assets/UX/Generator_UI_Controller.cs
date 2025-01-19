using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Generator_UI_Controller : MonoBehaviour
{


    [SerializeField] CityGeneratorController cityGenerator;
    Generator_Button_Controller button_Controller;

    int UILayer;
    
    public GameObject cam;

    public Button Terrain_Settings;
    public GameObject Terrain_Settings_Container;
    public GameObject Terrain_Mesh;
    public GameObject Terrain_Noise;

    MeshRenderer terrainNoise;
    MeshRenderer terrainMesh;

    [Header("Terrain Settings Sliders")]
    [SerializeField] public Slider mapSizeXSlider;
    [SerializeField] public Slider mapSizeYSlider;
    [SerializeField] Slider terrainNoiseScale;
    [SerializeField] Slider octaveCountSlider;
    [SerializeField] Slider gainSlider;
    [SerializeField] Slider lacunaritySlider;
    [SerializeField] Slider heightScalarSlider;
    [SerializeField] Slider xOffsetSlider;
    [SerializeField] Slider yOffsetSlider;
    [SerializeField] Slider ridgeSlider;
    [SerializeField] Slider seedSlider;
    [SerializeField] Toggle displayTypeToggle;

    // Start is called before the first frame update
    void Start() {
        button_Controller = new Generator_Button_Controller(Terrain_Settings_Container);
        UILayer = LayerMask.NameToLayer("UI");

        Terrain_Settings_Container.SetActive(false);
        Terrain_Settings_Container.GetComponent<Image>().enabled = false;

        terrainNoise = Terrain_Noise.GetComponent<MeshRenderer>();
        terrainMesh = Terrain_Mesh.GetComponent<MeshRenderer>();

        mapSizeXSlider.onValueChanged.AddListener(updateTerrain);
        mapSizeYSlider.onValueChanged.AddListener(updateTerrain);

        terrainNoiseScale.onValueChanged.AddListener(updateTerrain);
        octaveCountSlider.onValueChanged.AddListener(updateTerrain);
        gainSlider.onValueChanged.AddListener(updateTerrain);
        lacunaritySlider.onValueChanged.AddListener(updateTerrain);
        heightScalarSlider.onValueChanged.AddListener(updateTerrain);
        xOffsetSlider.onValueChanged.AddListener(updateTerrain);
        yOffsetSlider.onValueChanged.AddListener(updateTerrain);
        ridgeSlider.onValueChanged.AddListener(updateTerrain);
        seedSlider.onValueChanged.AddListener(updateTerrain);     

        displayTypeToggle.onValueChanged.AddListener(changeTerrainDisplayType);

        updateTerrain(1f);
    }

    private void changeTerrainDisplayType(bool arg0) {

        if(terrainNoise.enabled) {
            terrainNoise.enabled = false;
            terrainMesh.enabled = true;
        } else {
            terrainNoise.enabled = true;
            terrainMesh.enabled = false;
        }
    }

    private void updateTerrain(float arg) {

        cityGenerator.mapWidth = (int)mapSizeXSlider.value;
        cityGenerator.mapHeight = (int)mapSizeYSlider.value;
       
        cityGenerator.noiseScale = (int)terrainNoiseScale.value;
        cityGenerator.octaves = (int)octaveCountSlider.value;
        cityGenerator.gain = gainSlider.value;
        cityGenerator.lacunarity = lacunaritySlider.value;
        cityGenerator.heightScalar = heightScalarSlider.value;
        cityGenerator.offset.x = xOffsetSlider.value;
        cityGenerator.offset.y = yOffsetSlider.value;
        cityGenerator.ridgeBlendFactor = ridgeSlider.value;
        cityGenerator.seed = (int)seedSlider.value;

        cityGenerator.visualiseTerrain();
       
    }

    // Update is called once per frame
    void Update() {        
        if(IsPointerOverUIElement()) { lockCamera(); } else { unlockCamera(); }
    }


    public void lockCamera() {
        cam.GetComponent<Camera_Movement>().enabled = false;
    }
    
    private void unlockCamera() {
        cam.GetComponent<Camera_Movement>().enabled = true;
    }

    public bool IsPointerOverUIElement() {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }


    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults) {
        for(int index = 0; index < eventSystemRaysastResults.Count; index++) {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if(curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults() {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}



