using UnityEngine;

//class used for generating a mesh from the terrain height map
public class MeshGeneration {
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightScale, int vertexIndex = 0) {

        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        MeshData meshData = new MeshData(width, height);

        //loop through all points and assign a vertice to the correct height
        for(int heightCount = 0; heightCount < height; heightCount++) {
            for(int widthCount = 0; widthCount < width; widthCount++) {

                //for the y value we use the heightcurve to adjust the height value as well as taking into account the set heightscale 
                meshData.vertices[vertexIndex] = new Vector3(widthCount, heightMap[widthCount, heightCount] * heightScale, heightCount);
                meshData.uvs[vertexIndex] = new Vector2(widthCount / (float)width, heightCount / (float)height);

                //triangles need to be added to the mesh clockwise, we use the top tight vertex to make two triangles and the move on to the next vertex
                if(widthCount < width - 1 && heightCount < height - 1) {
                    meshData.addTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.addTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1); ;
                }

                vertexIndex++;
            }
        }
        return meshData;
    }
}

/// <summary>
/// Helper class for containing the vertices, triangles and uvs for the mesh 
/// as well as methods for adding triangles and handling mesh creation with the given data.
/// </summary>
public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    //Constructor
    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth) * (meshHeight) * 6];
    }

    //helper method for adding triangles
    public void addTriangle(int vert1, int vert2, int vert3) {

        triangles[triangleIndex] = vert3;
        triangles[triangleIndex + 1] = vert2;
        triangles[triangleIndex + 2] = vert1;

        triangleIndex += 3;
    }

    public Mesh meshCreation() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}

