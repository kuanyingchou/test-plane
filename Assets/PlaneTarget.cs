using UnityEngine;
using System.Collections;

public class PlaneTarget : MonoBehaviour {
    public Vector2 v = new Vector2(1, 1);
    public float s = 1;
    private float rx, ry;
    public Transform plane;
    public float earthRadius = 50;

    public void Update()
    {

        v = new Vector2(v.x + -Input.GetAxis("Horizontal") * s,
                        v.y + Input.GetAxis("Vertical") * s);


        Transform coreTransform = transform.parent;
        float x = coreTransform.eulerAngles.x;
        float a = 60;
        if(x > 180) x = x - 360; 
        if(x > a || x < -a)
        {
            v = new Vector2(v.x, -v.y);
        }

        float longitude = x;
        float radiusAtLongitude = 
                earthRadius * Mathf.Cos(Mathf.Abs(
                longitude * Mathf.Deg2Rad));
        float circumferenceAtLongitude = 
                2 * radiusAtLongitude * Mathf.PI;
        float HorizontalDistancePerDegree = 
                circumferenceAtLongitude / 360f;
        //Debug.Log(string.Format("longitude: {0}, current radius: {1}", 
        //    longitude, radiusAtLongitude));
        float circumference = 2 * earthRadius * Mathf.PI;
        float VerticalDistancePerDegree = circumference / 360f;

        rx += v.y / VerticalDistancePerDegree * Time.deltaTime;
        ry += v.x / HorizontalDistancePerDegree *  
                Time.deltaTime;
        coreTransform.rotation = Quaternion.Euler(rx, ry, 0);
        UpdatePlane();
        //DebugRay(coreTransform);
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
