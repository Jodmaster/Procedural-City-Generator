using UnityEditor;
using UnityEngine;

//The UI controls for population centers and map generation
[CustomEditor(typeof(populationGenerator))]
public class populationGeneratorEditor : Editor {

    public override void OnInspectorGUI() {
        populationGenerator popGen = (populationGenerator)target;


        if(DrawDefaultInspector()) {
            if(popGen.autoUpdate) {
                //deleteVisualisation();  
                popGen.generatePopMap();
            }
        }

        if(GUILayout.Button("new pop map")) {
            popGen.generatePopMap();
        }

       
    }
}
