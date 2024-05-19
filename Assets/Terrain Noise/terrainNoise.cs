using UnityEngine;
using System;

public class terrainNoise
{
    /// <summary>
    /// Generates a noise map based on perlin noise for initial terrain generation for the city
    /// </summary>
    /// 
    /// <param name="mapWidth"></param> the width of the map to be generated
    /// <param name="mapHeight"></param> the height of the map to be generated
    /// <param name="scale"></param> the scale of the noise used (the larger the scale the larger terrain features)
    /// <param name="octaves"></param> the number of layers used to generate the final noise map 
    /// <param name="gain"></param> controls how much of later ocataves "show through" on the noise map, increase for many smaller terrain features, decrease for less.
    /// <param name="lacunarity"></param> controls the scale for each octave, meaning finer detail for each subsequent octave
    /// 
    /// <returns>
    /// 2D float array 
    /// </returns>
    
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float gain, float lacunarity, Vector2 offset, AnimationCurve terrainHeightCurve, float ridgeBlendFactor) {
        
        float[,] noiseMap = new float[mapWidth, mapHeight];
        float[,] ridgeMap = new float[mapWidth, mapHeight];

        System.Random offs = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for(int i = 0; i < octaves; i++) {
            float offsetX = offs.Next(-10000, 10000) + offset.x;
            float offsetY = offs.Next(-10000, 10000) + offset.y;

            //we offset the octaves to avoid repeation of perlin points which can lead to very spiky terrain
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        //clamp the scale factor to above zero to avoid division by zero errors
        if(scale <= 0) {
            scale = 0.001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        
        float maxRidgeHeight = float.MinValue;
        float minRidgeHeight = float.MaxValue;
        
        for(int heightCount = 0; heightCount < mapHeight; heightCount++) {
            for(int widthCount = 0; widthCount < mapWidth; widthCount++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                float ridgedHeight = 0;                
                
                //different octaves are used for finer details in the terrain essentially layering the noise with varying amplitudes and frequencies
                for(int i = 0; i < octaves; i++) {
                    
                    //sample perlin at height and width
                    float sampleX = (widthCount / scale) * frequency + octaveOffsets[i].x;
                    float sampleY = (heightCount / scale) * frequency + octaveOffsets[i].y;

                    //the noiseheight value is calculated for the given octave
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    float ridgidValue = 2 * (0.5f - Mathf.Abs(0.5f - (Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1))); 
                    
                    noiseHeight += perlinValue * amplitude;
                    ridgedHeight += ridgidValue * amplitude;

                    //frequency will increase with each loop providing the finer details
                    //amplitude will decrease with each loop (provided between 0 - 1) meaining later layers have less of an impact on noise height
                    amplitude *= gain;
                    frequency *= lacunarity;
                }

                //update max or min depending on if noiseheight value is larger or smaller 
                if(noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if(noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                if(ridgedHeight > maxRidgeHeight) {
                    maxRidgeHeight = ridgedHeight;
                } else if(ridgedHeight < minRidgeHeight) {
                    minRidgeHeight = ridgedHeight;
                }

                noiseMap[widthCount, heightCount] = noiseHeight;
                ridgeMap[widthCount, heightCount] = ridgedHeight; 
            }
        }
       
        //normalise both the ridge and height map 
        for(int heightLimit = 0; heightLimit < mapHeight; heightLimit++) {
            for(int widthLimit = 0; widthLimit < mapWidth; widthLimit++) {
                noiseMap[widthLimit, heightLimit] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[widthLimit, heightLimit]);
                ridgeMap[widthLimit, heightLimit] = Mathf.InverseLerp(minRidgeHeight, maxRidgeHeight, ridgeMap[widthLimit, heightLimit]);
            }
        }

        maxNoiseHeight = float.MinValue;
        minNoiseHeight = float.MaxValue;

        for(int heightLimit = 0; heightLimit < mapHeight; heightLimit++) {
            for(int widthLimit = 0; widthLimit < mapWidth; widthLimit++) {                

                float finalHeight = ((1 - ridgeBlendFactor) * noiseMap[widthLimit, heightLimit]) + (ridgeBlendFactor * ridgeMap[widthLimit, heightLimit]);

                if(finalHeight > maxNoiseHeight) {
                    maxNoiseHeight = finalHeight;
                } else if(finalHeight < minNoiseHeight) {
                    minNoiseHeight = finalHeight;
                }

                noiseMap[widthLimit, heightLimit] = finalHeight;                              
            }
        }

        //renormalize the height map and apply the terrain height curve
        for(int heightLimit = 0; heightLimit < mapHeight; heightLimit++) {
            for(int widthLimit = 0; widthLimit < mapWidth; widthLimit++) {
                noiseMap[widthLimit, heightLimit] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[widthLimit, heightLimit]);
                noiseMap[widthLimit, heightLimit] = terrainHeightCurve.Evaluate(noiseMap[widthLimit, heightLimit]);
            }
        }

        return noiseMap;
    }   
}
