using System;
using UnityEngine;

public class RoadSection {
    public Node originNode;
    public Node destinationNode;

    public bool endOfSection;

    public int bendIteration;
    public bool bendRight;
    public bool bendLeft;

    public RoadSection(Node origin, Node destination, int bendCount) {
        originNode = origin;
        destinationNode = destination;
        endOfSection = false;

        bendIteration = bendCount;
        bendLeft = false;
        bendRight = false;
    }

    public bool isIntersectingRoad(RoadSection testRoad) {
        //if one of the nodes of the two roads is the same it can't be an intersection
        if(originNode == testRoad.originNode || destinationNode == testRoad.originNode || originNode == testRoad.destinationNode || destinationNode == testRoad.destinationNode) { return false; }

        //get the direction of each node relative to the other road
        int d1 = getDirectionOfRoad(originNode, destinationNode, testRoad.originNode);
        int d2 = getDirectionOfRoad(originNode, destinationNode, testRoad.destinationNode);
        int d3 = getDirectionOfRoad(testRoad.originNode, testRoad.destinationNode , originNode);
        int d4 = getDirectionOfRoad(testRoad.originNode, testRoad.destinationNode , destinationNode);

        //the pairs should be opposite (1, -1) if they cross over the line created by the two nodes
        //hence if they are both opposite the roads must intersect
        if(d1 != d2 && d3 != d4) { return true; }

        //can all be zero if the two roads are collinear to one another in this case we check if the two roads overlap
        if(d1 == 0 && d2 == 0 && d3 == 0 && d4 == 0) {
            if(liesOnRoad(originNode, destinationNode, testRoad.originNode)) { return true; }
            if(liesOnRoad(originNode, destinationNode, testRoad.destinationNode)) { return true; }
            if(liesOnRoad(testRoad.originNode, testRoad.destinationNode, originNode)) { return true; }
            if(liesOnRoad(testRoad.originNode, testRoad.destinationNode, destinationNode)) { return true; }
        }

        //otherwise they do not cross
        return false ;
    }

    private int getDirectionOfRoad(Node start, Node end, Node nodeToTest) {
        //get the cross product of the vectors from the node to test to the start and the end nodes
        float crossProd = (start.pos.z - nodeToTest.pos.z) * (end.pos.x - nodeToTest.pos.x) - (start.pos.x - nodeToTest.pos.x) * (end.pos.z - nodeToTest.pos.z);

        //the new node is clockwise from the current road
        if(crossProd > 0.001f) return 1;
        //the new node is anti clockwise from the current road
        if(crossProd < -0.001f) return -1;
        
        //the new node lies on the same line as the start and end node (collinear)
        else return 0;
    }

    //this method is only called if the points are collinear
    private bool liesOnRoad(Node start, Node end, Node nodeToTest) {

        //check that the node to check lies on the line made between the start and end node
        if(nodeToTest.pos.x <= Math.Max(start.pos.x, end.pos.x) && nodeToTest.pos.x >= Math.Min(start.pos.x, end.pos.x) && nodeToTest.pos.z <= Math.Max(start.pos.z, end.pos.z) && nodeToTest.pos.z >= Math.Min(start.pos.z, end.pos.z)){
            return true;
        }

        return false;
    }

    public Vector3 GetDirectionVector() {
        return new Vector3(destinationNode.pos.x - originNode.pos.x, 0, destinationNode.pos.z - originNode.pos.z).normalized;
    }
}
