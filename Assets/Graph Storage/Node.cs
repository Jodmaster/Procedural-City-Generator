using System.Collections.Generic;
using UnityEngine;

//used for storing the nodes of the road graph
public class Node
{
    public Vector3 pos;
    public nodeType type;
    public List<Edge> connectedEdges;

    public Node(Vector3 pos, nodeType type, ProductionRule rule) {
        this.pos = pos;
        this.type = type;
        connectedEdges = new List<Edge> ();
       
    }
   
    public void addEdge(Edge edgeToAdd) {
        connectedEdges.Add (edgeToAdd);
    }

}

//node type is used to match production rules to nodes
public enum nodeType{
    MainRoad,
    SideRoad
}