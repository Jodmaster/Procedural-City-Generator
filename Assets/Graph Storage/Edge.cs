using UnityEngine;

//This class stores edges between nodes in the road graph
public class Edge
{
    public Node parentNode;
    public Node childNode;

    public float directionFromParent;
    public float directionFromChild;

    public Edge (Node parent, Node child) {
        parentNode = parent;
        childNode = child;

        //gets direction in radians from both the parent and child node
        directionFromParent = Mathf.Atan2(child.pos.z - parent.pos.z, child.pos.x - parent.pos.x);
        directionFromChild = Mathf.Atan2(parent.pos.z - child.pos.z, parent.pos.x - child.pos.x);

        //adds the edge to the parent and child nodes
        parent.addEdge(this);
        child.addEdge(this);
    }
}
