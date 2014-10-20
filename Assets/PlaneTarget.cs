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
        //UpdateDirection(
        //        AccelerationController.GetAxisH, 
        //        AccelerationController.GetAxisV);
        UpdateDirection(
                Input.GetAxis("Horizontal"), 
                Input.GetAxis("Vertical"));
        UpdateRotation();
        UpdatePlane();
    }

    private void UpdateDirection(float h, float v)
    {
        velocity = Vector2.ClampMagnitude(new Vector2(
                velocity.x + -h * agility,
                velocity.y + v * agility), maxSpeed);
        CheckLimit();
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

    private void UpdateRotation()
    {
        rx += velocity.y / GetVerticalDistancePerDegree() * Time.deltaTime;
        ry += velocity.x / GetHorizontalDistancePerDegree() * Time.deltaTime;
        Transform coreTransform = transform.parent;
        coreTransform.rotation = Quaternion.Euler(rx, ry, 0);
    }

    private float GetVerticalDistancePerDegree()
    {
        float circumference = 2 * earthRadius * Mathf.PI;
        return circumference / 360f;
    }

    private float GetHorizontalDistancePerDegree()
    {
        Transform coreTransform = transform.parent;
        float longitude = coreTransform.eulerAngles.x;
        float radiusAtLongitude = 
                earthRadius * Mathf.Cos(Mathf.Abs(
                longitude * Mathf.Deg2Rad));
        float circumferenceAtLongitude = 
                2 * radiusAtLongitude * Mathf.PI;
        return circumferenceAtLongitude / 360f;
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
