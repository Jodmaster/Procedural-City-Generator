using UnityEngine;

public class textureGenerator 
{
    //takes the generated color map and creates the texture
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height) {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    //loop through the noise map and set a color map to some shade between black and white based on the height
    public static Texture2D TextureFromHeightMap(float[,] heightMap) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];

        for(int heightCounter = 0; heightCounter < height; heightCounter++) {
            for(int widthCounter = 0; widthCounter < width; widthCounter++) {
                colorMap[heightCounter * width + widthCounter] = Color.Lerp(Color.black, Color.white, heightMap[widthCounter, heightCounter]);
            }
        }

        //return the texture created in TextureFromColorMap
        return TextureFromColorMap(colorMap, width, height);
    }
}
