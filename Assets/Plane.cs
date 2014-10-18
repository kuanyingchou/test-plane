using UnityEngine;
using System.Collections;

public class Plane : MonoBehaviour {

    public Transform target;
    
    void Start () {
    
    }

    void Update () {
        transform.LookAt(target.position, transform.position - target.parent.position);
        transform.position = target.position;
        Debug.DrawRay(transform.position, target.parent.position - transform.position, Color.green, 5);
    }
}
