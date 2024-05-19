using UnityEngine;

//this class is used for the visualisation of textures and terrain meesh
public class MapDisplay : MonoBehaviour {
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    //draws supplied texture onto the texture renderer
    public void DrawTexture(Texture2D texture) {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width / 10f, 1, texture.height / 10f);
        textureRender.transform.position = new Vector3(texture.width / 2, 0, texture.height / 2);
    }
    
    public void DrawMesh(MeshData meshData) {
        meshFilter.sharedMesh = meshData.meshCreation();
    }
    
}
