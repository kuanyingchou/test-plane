using UnityEngine;
using System.Collections;

public class KZCircularTexture : MonoBehaviour {
    public int radius = 128;
    public Color color = new Color(1, 1, 1, 1);

    void Start () {
        UpdateProperties();
    }
    public void UpdateProperties() {
        renderer.material.mainTexture = 
                KZTexture.GetCircle(radius, color).ToTexture2D();
        renderer.material.shader = Shader.Find("Transparent/Diffuse"); 
        renderer.material.color = Color.white;
        //] TODO note that this shader has to be used in the scene
    }
}
