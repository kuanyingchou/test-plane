using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Planet : MonoBehaviour {
    public float radius = 1f;
    private List<AerialObject> bodies = new List<AerialObject>();

    public void AddBody(AerialObject o)
    {
        bodies.Add(o);
    }

    public Vector3 GetYAxis()
    {
        return transform.TransformPoint(
                transform.localPosition + Vector3.up) - 
                transform.position;
    }
    public Vector3 GetXAxis()
    {
        return transform.TransformPoint(
                transform.localPosition + Vector3.right) - 
                transform.position;
    }
    public Vector3 GetZAxis()
    {
        return transform.TransformPoint(
                transform.localPosition + Vector3.forward) - 
                transform.position;
    }

    public Vector2 GetLocation(Vector3 position)
    {
        Vector3 yAxis = GetYAxis();
        Vector3 diff = position - transform.position;
        Vector3 projOnYAxis = Vector3.Project(diff, yAxis);
        Vector3 diffToYAxis = diff - projOnYAxis;
        float longitude = Vector3.Angle(diffToYAxis, GetXAxis());
        float latitude = Vector3.Angle(diffToYAxis, diff);
        float angleToZ = Vector3.Angle(diffToYAxis, GetZAxis());
        if(!(angleToZ > 0 && angleToZ < 90))
        {
            longitude = -longitude;
        }
        float angleToY = Vector3.Angle(diff, yAxis);
        if(!(angleToY > 0 && angleToY < 90))
        {
            latitude = -latitude;
        }
        return new Vector2(longitude, latitude);
    }

    public Vector3 GetPosition(Vector2 location, float altitude = 0)
    {
        float r = radius + altitude;
        float ra = r * Mathf.Cos(location.y * Mathf.Deg2Rad);
        float x = ra * Mathf.Cos(location.x * Mathf.Deg2Rad);
        float z = ra * Mathf.Sin(location.x * Mathf.Deg2Rad);
        float y = r * Mathf.Sin(location.y * Mathf.Deg2Rad);
        return new Vector3(x, y, z);
    }

    public float GetVerticalDistancePerDegree()
    {
        float circumference = 2 * radius * Mathf.PI;
        return circumference / 360f;
    }

    public float GetHorizontalDistancePerDegree(float latitude)
    {
        float radiusAtLatitude = 
                radius * Mathf.Cos(Mathf.Abs(
                latitude * Mathf.Deg2Rad));
        float circumferenceAtLatitude = 
                2 * radiusAtLatitude * Mathf.PI;
        return circumferenceAtLatitude / 360f;
    }


    public void OnTouchClicked(KZTouchEvent t)
    {
        Vector3 hitPoint = t.hit.point;
        //Debug.Log("hit "+hitPoint);
        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        o.transform.position = hitPoint;
        o.transform.localScale = new Vector3(.5f, .5f, .5f);
        o.GetComponent<Collider>().enabled = false;

        Vector2 rallyPoint = GetLocation(hitPoint);
        foreach(var p in bodies)
        {
            p.HeadTo(rallyPoint);
        }
    }
}
