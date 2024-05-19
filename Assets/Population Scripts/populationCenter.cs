using UnityEngine;

//holder for population centers allows the user to create and execute during edit mode
[CreateAssetMenu(menuName = "population center")]
[ExecuteInEditMode]
public class populationCenter : ScriptableObject {
    public int[] location = new int[2];
    public float populationValue;
    public float falloffValue;
}