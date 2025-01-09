using System.Collections.Generic;
using UnityEngine;

public class CityGeneratorController : MonoBehaviour
{
    //whole city attributes
    [Range(1, 255)]
    public int mapWidth;
    [Range(1, 255)]
    public int mapHeight;

    public float[,] popVals;
    public float[,] terrainVals;

    //road attributes
    [Header ("Road Parameters")]
    public RoadGraph roadGraph;
    RoadGenerator roadGenerator;
    public int maxRoads;
    [Range(0.1f, 1f)]
    public float maxTerrainDiff;
    [Range(0.1f, 1f)]
    public float maxTerrainHeight;
    public List<ProductionRule> rules;
    public float intersectDistance;
    public int sectionsInBendLimit;

    //building attributes
    BuildingGen buildingGenerator;
    [Header ("Building Parameters")]
    public float heightVariation;
    public Material buildingMat;
    public AnimationCurve buildingHeightCurve;
    public AnimationCurve buildingDepthCurve;

    //terrain attributes
    [Header ("Terrain Parameters")]
    public float noiseScale;
    [Range(1, 10)]
    public int octaves;
    [Range(0f, 1f)]
    public float gain;
    [Range(0, 5)]
    public float lacunarity;
    public float heightScalar;
    [Range(0f, 1f)]
    public float ridgeBlendFactor;
    public int seed;
    public Vector2 offset;
    public AnimationCurve terrainHeightCurve;

    //visualisation attributes
    public MeshData meshData;
    public Material lineMat;
    public MapDisplay[] displays;

    //population maps attributes
    public List<populationCenter> populationCenters;
    public float minPopValue;

    public bool autoUpdateTerrain;

    public void initializeVisualisation() {
        displays = GetComponents<MapDisplay>();
    }

    //generate terrain height map
    private void terrainGeneration() {
        terrainVals = terrainNoise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, gain, lacunarity, offset, terrainHeightCurve, ridgeBlendFactor);          
    }

    //generate terrain mesh and display
    public void visualiseTerrain() {
        terrainGeneration();

        GameObject terrainMesh = GameObject.Find("terrainMesh");

        displays[0].DrawTexture(textureGenerator.TextureFromHeightMap(terrainVals));
        
        meshData = MeshGeneration.GenerateTerrainMesh(terrainVals, heightScalar);
        terrainMesh.GetComponent<MapDisplay>().DrawMesh(meshData);
    }

    //generate road graph
    private void roadGeneration() {
        roadGraph = new RoadGraph();
        roadGenerator = new RoadGenerator(mapWidth, mapHeight, maxRoads, sectionsInBendLimit, intersectDistance, maxTerrainHeight, maxTerrainDiff, terrainVals, heightScalar, populationCenters, rules , roadGraph);

        roadGenerator.generateRoads();
        roadGenerator.setNodeTerrainHeight();
    }

    //visulaise the road graph 
    public void visualiseRoads() {

        roadGeneration();
        
        foreach(Edge edge in roadGraph.edges) {
            GameObject line = new GameObject("edge");
            line.transform.position = edge.parentNode.pos;

            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();

            lineRenderer.material = lineMat;
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;

            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;

            lineRenderer.SetPosition(0, edge.parentNode.pos);
            lineRenderer.SetPosition(1, edge.childNode.pos);
        }

    }

    //generate buildings
    public void buildingGeneration() {
        buildingGenerator = new BuildingGen(roadGraph, buildingHeightCurve, buildingDepthCurve, popVals, terrainVals, heightScalar, heightVariation, mapWidth, mapHeight, buildingMat);
        buildingGenerator.generateAllotments();
    }

    //generate population map
    public void populationGeneration() {
        popVals = population_values.GeneratePopulationMap(mapWidth, mapHeight, populationCenters, minPopValue);

        MapDisplay popDisplay = displays[1];

        popDisplay.DrawTexture(textureGenerator.TextureFromHeightMap(popVals));
    }

    //is called on execution validates that attributes are within values required for generation to occur
    public void OnValidate() {
        if(mapWidth <= 0) { mapWidth = 1; }
        if(mapHeight <= 0) { mapHeight = 1; }
        if(maxRoads < 10) { maxRoads = 10; }
        if(maxTerrainHeight < 0f) { maxTerrainHeight = 0.01f; }
        if(maxTerrainDiff < 0f) { maxTerrainDiff = 0.01f; }
    }
}
