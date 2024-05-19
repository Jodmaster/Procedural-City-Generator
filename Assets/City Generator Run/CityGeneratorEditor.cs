using UnityEditor;
using UnityEngine;

//this class handles all UI requirements of the cityGeneratorController class
[CustomEditor(typeof(CityGeneratorController))]
public class CityGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        CityGeneratorController cityController = (CityGeneratorController)target;
        cityController.initializeVisualisation();       

        GUILayout.Space(16);

        if(DrawDefaultInspector()) {
            if(cityController.autoUpdateTerrain) {
                cityController.visualiseTerrain();
            }
        }

        if(GUILayout.Button("Generate Terrain")) {

            cityController.visualiseTerrain();
        }

        if(GUILayout.Button("Toggle terrain noise map")) {
            GameObject terrainNoise = GameObject.Find("terrainDisplay");
            terrainNoise.GetComponent<MeshRenderer>().enabled = !terrainNoise.GetComponent<MeshRenderer>().enabled;
        }

        if(GUILayout.Button("Toggle terrain mesh")) {
            GameObject terrainMesh = GameObject.Find("terrainMesh");
            terrainMesh.GetComponent<MeshRenderer>().enabled = !terrainMesh.GetComponent<MeshRenderer>().enabled;
        }

        GUILayout.Space(16);

        if(GUILayout.Button("Generate Pop Map")) {
            cityController.populationGeneration();
        }

        if(GUILayout.Button("Toggle pop map")) {
            GameObject popMap = GameObject.Find("popDisplay");
            popMap.GetComponent<MeshRenderer>().enabled = !popMap.GetComponent<MeshRenderer>().enabled;
        }

        GUILayout.Space(16);

        if(GUILayout.Button("Generate Roads")) {
            cityController.visualiseRoads();
        }

        if(GUILayout.Button("Delete Roads")) {
            LineRenderer[] edges = FindObjectsOfType<LineRenderer>();
            cityController.roadGraph.nodes.Clear();
            cityController.roadGraph.edges.Clear();

            foreach(LineRenderer line in edges) {
                DestroyImmediate(line.transform.gameObject);
            }
        }

        GUILayout.Space(16);

        if(GUILayout.Button("Generate Buildings")) {
            cityController.buildingGeneration();
        }

        if(GUILayout.Button("Delete Buildings")) {
            Object[] buildings = GameObject.FindGameObjectsWithTag("building");
            

            foreach(Object building in buildings) {
                DestroyImmediate(building);
            }
        }

    }
}
