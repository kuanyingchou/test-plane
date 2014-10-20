using UnityEngine;
using System.Collections;

public class PlaneTarget : MonoBehaviour {
    public Vector2 velocity = new Vector2(1, 1);
    public float agility = 5;
    public float maxSpeed = 20;
    public Plane plane;
    public float earthRadius = 50;
    public float latitudeLimit = 60;
    private float rx, ry;
    private float aptitude = 60;

    public void Update()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        UpdateDirection(
                AccelerationController.GetAxisH, 
                AccelerationController.GetAxisV);
        #else
        UpdateDirection(
                Input.GetAxis("Horizontal"), 
                Input.GetAxis("Vertical"));
        #endif
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
        float a = latitudeLimit;
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
        float latitude = coreTransform.eulerAngles.x;
        float radiusAtLatitude = 
                earthRadius * Mathf.Cos(Mathf.Abs(
                latitude * Mathf.Deg2Rad));
        float circumferenceAtLatitude = 
                2 * radiusAtLatitude * Mathf.PI;
        return circumferenceAtLatitude / 360f;
    }

    private void UpdatePlane()
    {
        plane.Follow(transform.position, transform.parent.position);
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
