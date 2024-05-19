using UnityEngine;

//Production rule for the L-system stores all the needed data for generation
[CreateAssetMenu(menuName = "production rule")]
public class ProductionRule : ScriptableObject
{
    public nodeType matchType;
    public nodeType typeToChangeTo;
    public nodeType newNodeTypeToSpawn;

    public float maxLength;
    public float minLength;
    public int maxAngle;

    public float branchChance;
    public float fireChance;
}
