using UnityEngine;
using System.Collections;

public class KZChequeredTexture : MonoBehaviour {
    public int width = 2;
    public int height = 2;
    public Color colorA = Color.white;
    public Color colorB = Color.black;

    public void Start() {
        KZTexture texture = KZTexture.GetChequeredTexture(
                width, height, colorA, colorB);
        Texture2D t2d = texture.ToTexture2D();
        t2d.filterMode = FilterMode.Point;
        t2d.wrapMode = TextureWrapMode.Repeat;
        renderer.material.mainTexture = t2d;
    }
}
