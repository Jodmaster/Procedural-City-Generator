using System.Collections.Generic;
using UnityEngine;

//this class is responsible for generating the population map generation uses based on supplied population centers
public class population_values {
    public static float[,] GeneratePopulationMap(int mapWidth, int mapHeight, List<populationCenter> popCenters, float lowestPopValue) {

        //intialize the return array
        float[,] populationValues = new float[mapWidth, mapHeight];

        float maxValue = float.MinValue;
        

        //find the max value for normalisation
        for(int x = 0; x < popCenters.Count; x++) {
            if(popCenters[x].populationValue > maxValue) { maxValue = popCenters[x].populationValue; }
        }              

        //loop through every cell in the array to get pop value
        for(int heightCount = 0; heightCount < mapHeight; heightCount++) {
            for(int widthCount = 0; widthCount < mapWidth; widthCount++) {                               

                float valueForCell = 0f;

                //for each population center get the distance and calc population contribution based on distance and fall off value of center
                foreach(populationCenter cent in popCenters) {
                    float distanceToCent = getDistance(widthCount, heightCount, cent);                   
                    float amountToAdd = (cent.populationValue * (1 / distanceToCent * cent.falloffValue) ) ;
                    
                    valueForCell += amountToAdd;                    
                }
                
                //if the value ends up being lower than the min required set to zero
                if(valueForCell < lowestPopValue) {
                    valueForCell = 0f;
                }

                populationValues[widthCount, heightCount] = valueForCell;
            }
        }
        
        //normalise the map to between 0 and 1
        for(int y = 0; y < mapHeight; y++) {
            for(int x = 0; x < mapWidth; x++) {
                populationValues[x, y] = Mathf.InverseLerp(0, maxValue, populationValues[x, y]);
            }
        }
        
        return populationValues;
                
    }

    //get distance from population centers
    private static float getDistance(int row, int column, populationCenter cent) {
        return Mathf.Sqrt(Mathf.Pow(row - cent.location[0], 2) + Mathf.Pow(column - cent.location[1], 2));
    }

}

