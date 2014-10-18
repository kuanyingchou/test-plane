using UnityEngine;
using System.Collections;

public class PlaneTarget : MonoBehaviour {
    public Vector2 v = new Vector2(50, 50);
    private float rx, ry;
    //private Vector3 oldPos = Vector3.zero;
    public Transform plane;

    public void Start() { }

    public void Update()
    {
        Transform coreTransform = transform.parent;
        float x = coreTransform.eulerAngles.x;
        float a = 60;
        if(x > 180) x = x - 360; 
        if(x > a || x < -a)
        {
            v = new Vector2(v.x, -v.y);
        }

        rx += v.y * Time.deltaTime;
        ry += v.x * Time.deltaTime;
        coreTransform.rotation = Quaternion.Euler(rx, ry, 0);
        UpdatePlane();
        DebugRay(coreTransform);
    }

    private void DebugRay(Transform coreTransform)
    {
        RaycastHit hit;
        if(Physics.Raycast(
                transform.position, 
                coreTransform.position - transform.position,  
                out hit)) 
        {
            Debug.DrawRay(transform.position, 
                    hit.point - transform.position, Color.green, 5);
        }
    }

    private void UpdatePlane()
    {
        plane.LookAt(transform.position, plane.position - transform.parent.position);
        plane.position = transform.position;
        //Debug.DrawRay(transform.position, target.parent.position - transform.position, Color.green, 5);
    }
}
