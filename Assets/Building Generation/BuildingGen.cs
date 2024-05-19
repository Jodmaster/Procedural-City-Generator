using System;
using System.Collections.Generic;
using UnityEngine;

//this class generates the allotments and then meshes for buildings
public class BuildingGen 
{
    private List<Edge> roads;
    private float minBuildingHeight = 0.35f;
    private float[,] popValues;
    private float[,] terrainValues;
    private float heightScalar;
    private float heightVariation;
    private AnimationCurve buildingHeightCurve;
    private AnimationCurve buildingDepthCurve;

    public List<Allotment> allotments;
    private List<Allotment> allomentTemp;

    private Material buildingMaterial;

    private float mapWidth;
    private float mapHeight;

    public BuildingGen(RoadGraph roadGraph, AnimationCurve buildingHeightCurve, AnimationCurve buildingDepthCurve, float[,] popMap, float[,] terrainMap, float heightScalar, float heightVariation, float mapWidth, float mapHeight, Material buildingMat) {
        
        //we check that all the prerequisite data structures exist 
        if(popMap == null) {
            throw new Exception("No Population Map");
        }

        if(terrainMap == null) {
            throw new Exception("No Terrain Map");
        }

        if(roadGraph == null) {
            throw new Exception("No roads");
        }

        roads = roadGraph.edges;
     
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        popValues = popMap;
        terrainValues = terrainMap;
        this.heightScalar = heightScalar;
        this.heightVariation = heightVariation;

        buildingMaterial = buildingMat;

        allotments = new List<Allotment>();
        this.buildingHeightCurve = buildingHeightCurve;
        this.buildingDepthCurve = buildingDepthCurve;


    }

    public void generateAllotments() {
        
        //loop through all the roads and generate new allotments on either side
        foreach(Edge road in roads) {

            Allotment newLot = generateAllotment(road, true);
            Allotment newLot2 = generateAllotment(road, false);

                        
            if(allotmentIsValid(newLot2)) {
                allotments.Add(newLot2);
            } else {
                continue;
            }

            if(allotmentIsValid(newLot)) {
                allotments.Add(newLot);
            } else {
                continue;
            }            

        }

        generateMeshes();
    }

    public Allotment generateAllotment(Edge road, bool side) {

        Vector3[] corners = new Vector3[4];        

        float rotationFromParent;
        float rotationFromChild;

        float popVal = getPopValue((road.parentNode.pos + road.childNode.pos) / 2);
        float depth;

        //evaluate the population value on the depth curve 
        depth = buildingDepthCurve.Evaluate(popVal);
        

        //pick a random depth for the allotment       
        float nudgeAngle = UnityEngine.Random.Range(0, 10f) * (Mathf.PI / 180);
        float sideNudge = UnityEngine.Random.Range(0.1f, 0.25f);

        //checks which side of the road to generate on 
        //then gets the direction of the road and calculates the direction to go for the next vertices
        if(side) {

            //nudge is a small randomisation to the position of initial corner vertices 
            float childNudge;
            float parentNudge;

            if(UnityEngine.Random.Range(0f,1f) > 0.5f) {
                childNudge = -nudgeAngle;
            } else { childNudge = nudgeAngle; }

            if(UnityEngine.Random.Range(0f, 1f) > 0.5f) {
                parentNudge = -nudgeAngle;
            } else {
                parentNudge = nudgeAngle;
            }

            //new position vectors for vertices are found and set 
            rotationFromParent = road.directionFromParent - (Mathf.PI / 2) + parentNudge;
            rotationFromChild = road.directionFromChild + (Mathf.PI / 2) + childNudge;

            float parentNudgeX = sideNudge * Mathf.Cos(road.directionFromParent);
            float parentNudgeZ = sideNudge * Mathf.Sin(road.directionFromParent);

            float childNudgeX = sideNudge * Mathf.Cos(road.directionFromChild);
            float childNudgeZ = sideNudge * Mathf.Sin(road.directionFromChild);

            corners[0] = new Vector3(road.parentNode.pos.x, 0, road.parentNode.pos.z) - new Vector3(parentNudgeX, 0, parentNudgeZ);          
            corners[1] = new Vector3(road.childNode.pos.x, 0, road.childNode.pos.z) - new Vector3(childNudgeX, 0, childNudgeZ);            

        } else {

            float childNudge;
            float parentNudge;

            if(UnityEngine.Random.Range(0f, 1f) > 0.5f) {
                childNudge = -nudgeAngle;
            } else { childNudge = nudgeAngle; }

            if(UnityEngine.Random.Range(0f, 1f) > 0.5f) {
                parentNudge = -nudgeAngle;
            } else {
                parentNudge = nudgeAngle;
            }

            rotationFromParent = road.directionFromChild - (Mathf.PI / 2) + childNudge;
            rotationFromChild = road.directionFromParent + (Mathf.PI / 2) + parentNudge;

            float parentNudgeX = sideNudge * Mathf.Cos(road.directionFromParent);
            float parentNudgeZ = sideNudge * Mathf.Sin(road.directionFromParent);

            float childNudgeX = sideNudge * Mathf.Cos(road.directionFromChild);
            float childNudgeZ = sideNudge * Mathf.Sin(road.directionFromChild);

            corners[0] = new Vector3(road.childNode.pos.x, 0, road.childNode.pos.z) - new Vector3(parentNudgeX, 0, parentNudgeZ);
            corners[1] = new Vector3(road.parentNode.pos.x, 0, road.parentNode.pos.z) - new Vector3(childNudgeX, 0, childNudgeZ);

        }

        //get the amount to increase x and z coordinates based on depth and direction
        float xIncreaseParent = depth * Mathf.Cos(rotationFromParent);
        float zIncreaseParent = depth * Mathf.Sin(rotationFromParent);
        
        float xIncreaseChild = depth * Mathf.Cos(rotationFromChild);
        float zIncreaseChild = depth * Mathf.Sin(rotationFromChild); 

        //generate the positions of new vertices
        Vector3 backLeft = corners[0] + new Vector3(xIncreaseParent, 0, zIncreaseParent);
        Vector3 backRight = corners[1] + new Vector3(xIncreaseChild, 0, zIncreaseChild);

        //we then nudge the original edge vectors slightly back from the road
        corners[0].x += Mathf.Cos(rotationFromParent) * 0.1f;
        corners[0].z += Mathf.Sin(rotationFromParent) * 0.1f;
        
        corners[1].x += Mathf.Cos(rotationFromChild) * 0.1f;
        corners[1].z += Mathf.Sin(rotationFromChild) * 0.1f;

        //set vertices
        corners[2] = backLeft;
        corners[3] = backRight;     

        return new Allotment(corners, road);

    }

    public bool isIntersectingEdge(Vector3 startPos, Vector3 endPos, Vector3 edgeStart, Vector3 edgeEnd) {

        //check that it's not the same edge
        if(startPos == edgeStart || endPos == edgeStart || startPos == edgeEnd || endPos == edgeEnd) { return false; }

        //get determinants of lines
        int d1 = getDirection(startPos, endPos, edgeStart);
        int d2 = getDirection(startPos, endPos, edgeEnd);
        int d3 = getDirection(edgeStart, edgeEnd, startPos);
        int d4 = getDirection(edgeStart, edgeEnd, endPos);

        if(d1 != d2 && d3 != d4) { return true; }

        if(d1 == 0 && d2 == 0 && d3 == 0 && d4 == 0) {
            if(liesOnSameLine(startPos, endPos, edgeStart)) { return true; }
            if(liesOnSameLine(startPos, endPos, edgeEnd)) { return true; }
            if(liesOnSameLine(edgeStart, edgeEnd, startPos)) { return true; }
            if(liesOnSameLine(edgeStart, edgeEnd, endPos)) { return true; }    
        }

        return false;

        //gets the direction of the test point to the line 
        int getDirection(Vector3 start, Vector3 end, Vector3 testPos) {
            
            float cross = (start.z - testPos.z) * (end.x - testPos.x) - (start.x - testPos.x) * (end.z - testPos.z);

            if(cross > 0.001f)  return 1; 
            if(cross < -0.001f)  return -1; 
            else return 0;
        }

        //helper method to check if the point lies on the same line 
        bool liesOnSameLine(Vector3 start, Vector3 end, Vector3 testPos) { 
            
            if(testPos.x <= Math.Max(start.x, end.x) && testPos.x >= Math.Min(start.x, end.x) && testPos.z <= Math.Max(start.z, end.z) && testPos.z >= Math.Min(start.z, end.z)){
                return true;
            }

            return false;
        }
    }

    private bool allotmentIsValid(Allotment alloment) {
       
        //check that the allotment is not intersecting with a road
        foreach(Edge edge in roads) {
            
            if((alloment.corners[0] == edge.parentNode.pos || alloment.corners[1] == edge.parentNode.pos) && (alloment.corners[0] == edge.childNode.pos || alloment.corners[1] == edge.childNode.pos)) {
                continue;
            }

            if(isIntersectingEdge(alloment.corners[0], alloment.corners[2], edge.parentNode.pos, edge.childNode.pos)) {
                
                return false;
            }
            
            if(isIntersectingEdge(alloment.corners[1], alloment.corners[3], edge.parentNode.pos, edge.childNode.pos)) {
                
                return false;
            }

            if(isIntersectingEdge(alloment.corners[2], alloment.corners[3], edge.parentNode.pos, edge.childNode.pos)) {
                
                return false;
            }

        }

        //check that the allotment is not intersecting with another allotment
        foreach(Allotment allotmentToCheck in allotments) {

            Vector3[] frontEdge = new Vector3[] { allotmentToCheck.corners[0], allotmentToCheck.corners[1] };
            Vector3[] leftEdge  = new Vector3[] { allotmentToCheck.corners[0], allotmentToCheck.corners[2] };
            Vector3[] rightEdge = new Vector3[] { allotmentToCheck.corners[1], allotmentToCheck.corners[3] };
            Vector3[] backEdge  = new Vector3[] { allotmentToCheck.corners[2], allotmentToCheck.corners[3] };

            //check left edge with all edges on allotmentto check
            if(isIntersectingEdge(alloment.corners[0], alloment.corners[2], frontEdge[0], frontEdge[1]) || isIntersectingEdge(alloment.corners[0], alloment.corners[2], leftEdge[0], leftEdge[1]) || isIntersectingEdge(alloment.corners[0], alloment.corners[2], rightEdge[0], rightEdge[1]) || isIntersectingEdge(alloment.corners[0], alloment.corners[2], backEdge[0], backEdge[1])) {
                return false;
            }

            //check right edge with all edges on allotment to check
            if(isIntersectingEdge(alloment.corners[1], alloment.corners[3], frontEdge[0], frontEdge[1]) || isIntersectingEdge(alloment.corners[1], alloment.corners[3], leftEdge[0], leftEdge[1]) || isIntersectingEdge(alloment.corners[1], alloment.corners[3], rightEdge[0], rightEdge[1]) || isIntersectingEdge(alloment.corners[1], alloment.corners[3], backEdge[0], backEdge[1])) {
                return false;
            }

            //check back edges with all edges on allotment to check
            if(isIntersectingEdge(alloment.corners[2], alloment.corners[3], frontEdge[0], frontEdge[1]) || isIntersectingEdge(alloment.corners[2], alloment.corners[3], leftEdge[0], leftEdge[1]) || isIntersectingEdge(alloment.corners[2], alloment.corners[3], rightEdge[0], rightEdge[1]) || isIntersectingEdge(alloment.corners[2], alloment.corners[3], backEdge[0], backEdge[1])) {
                return false;
            }
        }

        return true;
    }

    private void generateMeshes() {
        
        foreach(Allotment allotment in allotments) {

            bool varyHeight;

            //first we generate the top vertices of the building
            List<Vector3> vertexPositions = new List<Vector3>();
            vertexPositions.AddRange(allotment.corners);           
            
            float height = buildingHeightCurve.Evaluate(getPopValue(getCenterValue(allotment.corners)));
            height = UnityEngine.Random.Range(height - (height * heightVariation), height + (height * heightVariation));

            foreach(Vector3 corner in allotment.corners) {
                vertexPositions.Add(new Vector3(corner.x, minBuildingHeight + height * heightScalar, corner.z));
            }

            //set up the gameobject that will hold and display the mesh information
            GameObject building = new GameObject("building");
            Mesh mesh = new Mesh();

            MeshFilter filter = building.AddComponent<MeshFilter>();
            MeshRenderer renderer = building.AddComponent<MeshRenderer>();

            filter.mesh = mesh;

            //we store the vertices of each face
            //done this way to make the calculation of tris easier to read and calculate
            Vector3[] vertexes = new Vector3[] {
                //bottom face
                vertexPositions[0], vertexPositions[1], vertexPositions[2], vertexPositions[3], 
                //front face
                vertexPositions[0], vertexPositions[4], vertexPositions[5], vertexPositions[1],
                //left face
                vertexPositions[2], vertexPositions[6], vertexPositions[4], vertexPositions[0],
                //right face
                vertexPositions[1], vertexPositions[5], vertexPositions[7], vertexPositions[3],
                //back face
                vertexPositions[3], vertexPositions[7], vertexPositions[6], vertexPositions[2],
                //top face
                vertexPositions[5], vertexPositions[4], vertexPositions[6], vertexPositions[7]
            };

            mesh.vertices = vertexes;
            
            //set triangles
            int[] triangles = new int[] {

                3, 1, 0,   2, 3, 0,
                6, 5, 4,   7, 6, 4,
                11,10, 9,  8, 11, 9,
                15,14,12,  14,13,12,
                19,18,16,  18,17,16,
                21,20,22,  20,23,22

            };
            
            mesh.triangles = triangles;

            float yHeight = getTerrainValues(getCenterValue(vertexPositions.ToArray())) * 0.9f;
            building.transform.position = new Vector3(building.transform.position.x, yHeight * heightScalar, building.transform.position.z);

            renderer.material = buildingMaterial;
            building.tag = "building";
            mesh.RecalculateNormals();
        }       
    }

    //gets the center position of the generated allotment
    private Vector3 getCenterValue(Vector3[] corners) {

        Vector3 center = Vector3.zero;

        foreach(Vector3 corner in corners) {
            center += corner;
        }

        center /= corners.Length;

        return center;
    }

    //gets the value of pop map at a given position
    private float getPopValue(Vector3 position) {

        int[] arrayPos = new int[] { Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z) };

        if(arrayPos[0] < mapWidth - 1 && arrayPos[1] < mapHeight - 1 && arrayPos[0] > 0 && arrayPos[1] > 0) {
            return popValues[arrayPos[0], arrayPos[1]];
        } else { return float.MinValue; }

    }

    //gets the value of the terrain map at a given position
    private float getTerrainValues(Vector3 position) {
        int[] arrayPos = new int[] { Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z) };

        if(arrayPos[0] < mapWidth - 1 && arrayPos[1] < mapHeight - 1 && arrayPos[0] > 0 && arrayPos[1] > 0) {
            return terrainValues[arrayPos[0], arrayPos[1]];
        } else { return float.MinValue; }

    }

}
