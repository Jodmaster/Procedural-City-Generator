using UnityEngine;

//this class stores the positions of generated allotment vertices and the road that the building is attached to
public class Allotment 
{
    public Vector3[] corners;
    public Edge parentEdge;

    public Allotment(Vector3[] corners, Edge parentEdge) {
        this.corners = corners;
        this.parentEdge = parentEdge;
    }
}
