using UnityEngine;
using System.Collections;

public class PlaneTarget : MonoBehaviour {
    public Vector2 velocity = new Vector2(1, 1);
    public float agility = 5;
    public float maxSpeed = 20;
    public Transform plane;
    public float earthRadius = 50;
    public float longitudeLimit = 60;
    private float rx, ry;

    public void Update()
    {
        UpdateDirection();
        UpdateRotation();
        UpdatePlane();
        //DebugRay(coreTransform);
    }

    private void UpdateDirection()
    {
        velocity = Vector2.ClampMagnitude(new Vector2(
                velocity.x + -Input.GetAxis("Horizontal") * agility,
                velocity.y + Input.GetAxis("Vertical") * agility), maxSpeed);
        CheckLimit();
    }

    private void UpdateRotation()
    {
        Transform coreTransform = transform.parent;
        float longitude = coreTransform.eulerAngles.x;
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

        rx += velocity.y / VerticalDistancePerDegree * Time.deltaTime;
        ry += velocity.x / HorizontalDistancePerDegree *  
                Time.deltaTime;
        coreTransform.rotation = Quaternion.Euler(rx, ry, 0);
    }

    private void CheckLimit()
    {
        Transform coreTransform = transform.parent;
        float x = coreTransform.eulerAngles.x;
        float a = longitudeLimit;
        if(x > 180) x = x - 360; 
        if(x > a || x < -a)
        {
            velocity = new Vector2(velocity.x, -velocity.y);
        }
    }

    private void UpdatePlane()
    {
        plane.LookAt(
                transform.position, 
                plane.position - transform.parent.position);
        plane.position = transform.position;
        //Debug.DrawRay(transform.position, target.parent.position - transform.position, Color.green, 5);
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

}
