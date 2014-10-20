using UnityEngine;
using System.Collections;

public class Propeller : MonoBehaviour {
    public void Update()
    {
        transform.Rotate(new Vector3(0, 0, 10)); 
    }
}
