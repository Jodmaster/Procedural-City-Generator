using UnityEngine;
using System.Collections.Generic;
using System;

public class RoadGenerator 
{

    private List<RoadSection> globalGoalsRoads;
    private List<RoadSection> roadQueue;
    private List<RoadSection> roads;

    private int mapWidth;
    private int mapHeight;

    private int maxRoads;
    private float maxTerrainHeight;
    private float maxTerrainDifference;

    private float intersectDistance;
    private float sectionsInBend;

    private RoadGraph graph;

    private float[,] terrainMap;
    private float heightScalar;
    private List<populationCenter> centers;
    private List<ProductionRule> rules;

    public RoadGenerator(int width, int height, int maxRoad, int sectionBendLimit, float intersectDistance, float maxTerrainHeight, float maxTerrainDifference, float[,] terrainValues, float heightScalar, List<populationCenter> centers, List<ProductionRule> rules, RoadGraph buildGraph) {
        
        if(terrainValues == null) {
            throw new Exception("No terrain ");
        }

        if(centers.Count <= 0) {
            throw new Exception("no population values");
        }

        maxRoads = maxRoad;
        this.intersectDistance = intersectDistance;
        sectionsInBend = sectionBendLimit;
        mapWidth = width;
        mapHeight = height;
        graph = buildGraph;
        this.centers = centers;
        this.rules = rules;
        terrainMap = terrainValues;
        this.heightScalar = heightScalar;
        this.maxTerrainHeight = maxTerrainHeight;
        this.maxTerrainDifference = maxTerrainDifference;

        globalGoalsRoads = new List<RoadSection>();
        roadQueue = new List<RoadSection>();
        roads = new List<RoadSection>();
    }

    public void generateRoads() {
        
        generateStartRoads();

        while(roadQueue.Count != 0 && roads.Count < maxRoads) {

            RoadSection currentRoad = roadQueue[0];
            roadQueue.RemoveAt(0);

            //if the road section passes the local checks add it to the roads array
            if(!isValidLocal(currentRoad)) { continue; };

            //add the road section to the graph
            roads.Add(currentRoad);
            addSectionToRoadGraph(currentRoad);

            //grow the node with the global goals
            generateNewSections(currentRoad);
        }
       
    }

    //Check for local contraints - this is either the road node is too close to an existing node or is crossing another road
    //out of map boundries, either the parent or child has more than four connecting roads, or if the road already exists
    private bool isValidLocal(RoadSection roadSec) {

        foreach(RoadSection roadToCheck in roads) {
            
            //checks if the new end node is too close to another sections node, if it is change the end to it
            if(isTooCloseToExistingNode(roadSec.destinationNode, roadToCheck.destinationNode)) {
                
                roadSec.destinationNode = roadToCheck.destinationNode;
                roadSec.endOfSection = true;
            
            }

            //new road is intersecting existing road so not valid
            if(roadSec.isIntersectingRoad(roadToCheck)) { return false; }
        }

        //check that the road section is within the bounds of the map
        if(roadSec.originNode.pos.x > mapWidth - 10 || roadSec.originNode.pos.x < 10 || roadSec.originNode.pos.z > mapHeight - 10 || roadSec.originNode.pos.z < 10
        || roadSec.originNode.pos.x > mapHeight - 10 || roadSec.originNode.pos.x < 10 || roadSec.originNode.pos.z > mapWidth - 10 || roadSec.originNode.pos.z < 10)
        { return false; }

        //check that the road section is within valid terrain height
        if(sampleValue(roadSec.originNode.pos, terrainMap) > maxTerrainHeight || sampleValue(roadSec.destinationNode.pos, terrainMap) > maxTerrainHeight) { return false; }

        //check that the road section is not too steep
        if(Mathf.Abs(sampleValue(roadSec.originNode.pos, terrainMap) - sampleValue(roadSec.destinationNode.pos, terrainMap)) > maxTerrainDifference) { return false; }

        //check that the section doesn't fold in on itself
        if(roadSec.originNode.pos.x == roadSec.destinationNode.pos.x && roadSec.originNode.pos.z == roadSec.destinationNode.pos.z) { return false; }
        
        //check that the origin or destionation wouldn't have more than 4 connected nodes
        if(roadSec.destinationNode.connectedEdges.Count >= 4 || roadSec.originNode.connectedEdges.Count >= 4) { return false; }

        //check that the nodes aren't already connected
        foreach(Edge edge in roadSec.destinationNode.connectedEdges) {
            if(edge.parentNode == roadSec.originNode || edge.childNode == roadSec.originNode) { return false; }
        }
        
        return true;
    }

    //global goals to achieve
    private void generateNewSections(RoadSection section) {
        
        if(section.endOfSection) { return; }
        globalGoalsRoads.Clear();
        //get direction of road
        Vector3 directionVector = section.GetDirectionVector();
        ProductionRule rule = ruleToFollow(section.destinationNode);

        if(rule == null) { 
            section.endOfSection = true;
            return;
        }

        bool shouldBranch = false;
        int randDirection = 0;

        //randomly pick if the newsection should branch or not  
        if(UnityEngine.Random.Range(1, 101) <= rule.branchChance) {
            shouldBranch = true;
        }

        //if it should branch pick randomly between left branch. right branch or both 
        if(shouldBranch) {
            randDirection = UnityEngine.Random.Range(1, 4);          
        }
       
        //branch right
        if(randDirection == 1) {

            Vector3 newDirection = new Vector3(directionVector.z, 0, -directionVector.x);
            RoadSection branchedSection = generateNewRoadSection(section.destinationNode, newDirection, 0, rule);
            globalGoalsRoads.Add(branchedSection);

        }
        //branch left
        else if ( randDirection == 2) {

            Vector3 newDirection = new Vector3(-directionVector.z, 0, directionVector.x);
            RoadSection branchedSection = generateNewRoadSection(section.destinationNode, newDirection, 0, rule);
            globalGoalsRoads.Add(branchedSection);

        }
        //branch both directions
        else if ( randDirection == 3) {

            Vector3 newDirection1 = new Vector3(directionVector.z, 0, -directionVector.x);
            Vector3 newDirection2 = new Vector3(-directionVector.z, 0, directionVector.x);

            RoadSection branchedSection1 = generateNewRoadSection(section.destinationNode, newDirection1, 0, rule);
            RoadSection branchedSection2 = generateNewRoadSection(section.destinationNode, newDirection2, 0, rule);

            globalGoalsRoads.Add(branchedSection1);
            globalGoalsRoads.Add(branchedSection2);

        }
        
        globalGoalsRoads.Add(generateContinuingRoadSection(section, rule));

        foreach(RoadSection newlyGenerated in globalGoalsRoads) {
            roadQueue.Add(newlyGenerated);
        }
        
    }

    private void generateStartRoads() {

        //generate coordinates for inital node             
        Vector3[] startPosistions = getStartLocations(centers);

        foreach(Vector3 popLocation in startPosistions) {
            Node startNode = new Node(popLocation, nodeType.MainRoad, rules[0]);

            //generate the first starting direction vectors        
            int randXDirection = UnityEngine.Random.Range(-100, 100);
            int randZDirection = UnityEngine.Random.Range(-100, 100);

            Vector3 startDirection = new Vector3(randXDirection, 0, randZDirection).normalized;

            //create new nodes based on the direction generated

            Node newNode1 = new Node(new Vector3(startNode.pos.x + startDirection.x, 0, startNode.pos.z + startDirection.z), nodeType.MainRoad, rules[0]);
            Node newNode2 = new Node(new Vector3(startNode.pos.x - startDirection.x, 0, startNode.pos.z - startDirection.z), nodeType.MainRoad, rules[0]);

            //creates and adds the new road sections to the queue

            RoadSection road1 = new RoadSection(startNode, newNode1, 0);
            RoadSection road2 = new RoadSection(startNode, newNode2, 0);

            roadQueue.Add(road1);
            roadQueue.Add(road2);
        }
    }
    
    private RoadSection generateNewRoadSection (Node origin, Vector3 direction, int bendIteration, ProductionRule rule) {
        float newLength = UnityEngine.Random.Range(rule.minLength, rule.maxLength);
        Node newNode = new Node(new Vector3(origin.pos.x + direction.x * newLength, 0, origin.pos.z + direction.z * newLength), rule.newNodeTypeToSpawn, rule);
        return new RoadSection(origin, newNode, bendIteration);
    }

    private RoadSection generateContinuingRoadSection (RoadSection section, ProductionRule rule) {

        Vector3 direction = section.GetDirectionVector();

        if(rule.maxAngle < 1) { return generateNewRoadSection(section.destinationNode, direction, 0, rule); }

        //if the section has reached the max bend limit pick a new direction to bend in 
        if(section.bendIteration == sectionsInBend) {

            int randInt = UnityEngine.Random.Range(0, 3);

            if(randInt == 1) {
                

                Quaternion rotation = Quaternion.Euler(0, randomAngle(rule.maxAngle * 2), 0);
                direction = rotation * direction;

                RoadSection newSection = generateNewRoadSection(section.destinationNode, direction, 0, rule);
                newSection.bendLeft = true;
                return newSection;
            }
            else if (randInt == 2) {
                

                Quaternion rotation = Quaternion.Euler(0, -(randomAngle(rule.maxAngle * 2)), 0);
                direction = rotation * direction;

                RoadSection newSection = generateNewRoadSection(section.destinationNode, direction, 0, rule);
                newSection.bendRight = true;
                return newSection;
            
            } else {

                return generateNewRoadSection(section.destinationNode, direction, 0, rule);
            }
        
        }
        //sections are currently bending continue bend direction
        else {

            if(section.bendLeft) {

                Quaternion rotation = Quaternion.Euler(0, randomAngle(rule.maxAngle * 2), 0);
                direction = rotation * direction;

                RoadSection newSection = generateNewRoadSection(section.destinationNode, direction, section.bendIteration + 1, rule);
                newSection.bendLeft = true;
                return newSection;

            } else if(section.bendRight) {
                
                Quaternion rotation = Quaternion.Euler(0, -(randomAngle(rule.maxAngle * 2)), 0);
                direction = rotation * direction;              

                RoadSection newSection = generateNewRoadSection(section.destinationNode, direction, section.bendIteration + 1, rule);
                newSection.bendRight = true;
                return newSection;
            
            } else {
               
                return generateNewRoadSection(section.destinationNode, direction, section.bendIteration + 1, rule);

            }

        }
    }

    private float randomAngle(float max) {       
        //get range to pick from then get random int
        float randRot = UnityEngine.Random.Range(0, max);

        //return rotation angle in radians       
        return (float)(Math.PI / 180) * randRot;
    }

    //checks if a node is too close too an already exisiting node
    private bool isTooCloseToExistingNode(Node i, Node j) {
        float idealDistance = intersectDistance;

        if(Vector3.Distance(i.pos, j.pos) < idealDistance) {return true;}
        return false;
    }  
    
    private Vector3[] getStartLocations(List<populationCenter> centers) {
        Vector3[] centerLocations = new Vector3[centers.Count];
        int counter = 0;

        foreach(populationCenter center in centers) {
            centerLocations[counter] = new Vector3(center.location[0], 0, center.location[1]);
            counter++;
        }

        return centerLocations;
    }
    
    private float sampleValue(Vector3 locationToSample, float[,] arrayToSample) {
        
        int[] arrayVal = positionToArrayValue(locationToSample);

        if(arrayVal[0] < mapWidth - 1 && arrayVal[1] < mapHeight - 1 && arrayVal[0] > 0 && arrayVal[1] > 0) {
            return arrayToSample[arrayVal[0], arrayVal[1]];
        } else { return float.MinValue; }
        
    }

    private int[] positionToArrayValue(Vector3 worldPositionVector) {

        int[] arrayValue = new int[2];

        arrayValue[0] = Mathf.RoundToInt(worldPositionVector.x);
        arrayValue[1] = Mathf.RoundToInt(worldPositionVector.z);

        return arrayValue;
    }

    public void setNodeTerrainHeight() {
        foreach(Node node in graph.nodes) {
            int[] arrayPos = positionToArrayValue(node.pos);
            float yValue = terrainMap[arrayPos[0], arrayPos[1]];

            node.pos.y = yValue * heightScalar;
        }
    }

    private ProductionRule ruleToFollow(Node node) {
        
        List<ProductionRule> validRules = new List<ProductionRule>();
        float totalWeight = 0f;
        ProductionRule pickedRule = null;

        //get all the valid rules for the node, add their fire weights to the total weight
        foreach(ProductionRule rule in rules) {
            if(node.type == rule.matchType) {
                validRules.Add(rule);
                totalWeight += rule.fireChance;
            }
        }

        //pick a random number between 0 and the total weight of the rules,
        //if that number is smaller than the current rules firechance the rule becomes picked 
        foreach(ProductionRule potentialRule in validRules) {
            if(UnityEngine.Random.Range(0, totalWeight) <= potentialRule.fireChance) {
                pickedRule = potentialRule;
                break;
            }

            //if the rule wasn't picked remove its firechance from the total weight
            totalWeight -= potentialRule.fireChance;
        }

     

        return pickedRule;
    }

    private void addSectionToRoadGraph(RoadSection road) {
        if(!graph.nodes.Contains(road.originNode)) {graph.nodes.Add(road.originNode);}
        if(!graph.nodes.Contains(road.destinationNode)) { graph.nodes.Add(road.destinationNode);}
        graph.edges.Add(new Edge(road.originNode, road.destinationNode));
    }

}
