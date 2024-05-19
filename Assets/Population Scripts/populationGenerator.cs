using System.Collections.Generic;
using UnityEngine;

//generates thte final pop map and displays it on the texture 
public class populationGenerator : MonoBehaviour {
    
    public int mapWidth;
    public int mapHeight;

    public List<populationCenter> populationCenters;

    public float minPopValue;

    public float[,] popValues;

    public bool autoUpdate;

    public void generatePopMap() {

        popValues = population_values.GeneratePopulationMap(mapWidth, mapHeight, populationCenters, minPopValue);

        MapDisplay display= GetComponent<MapDisplay>();

        display.DrawTexture(textureGenerator.TextureFromHeightMap(popValues));
    }
}
