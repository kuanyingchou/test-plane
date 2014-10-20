using UnityEngine;
using System.Collections;

public class KZColoredTexture : MonoBehaviour {

    public Color color = Color.white;

    public void Start() {
        KZTexture kzt = new KZTexture(1, 1);
        kzt.SetPixel(0, 0, color);
        renderer.material.mainTexture = kzt.ToTexture2D();
    }
}
