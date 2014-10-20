using UnityEngine;
using System.Collections;

public class Planet : MonoBehaviour {
    public float _radius = 1f;

    public float radius
    {
        get { return _radius; }
        set { 
            _radius = value; 
            //transform.localScale = new Vector3(_radius, _radius, _radius);
        }
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

    public Vector2 GetLocation(Vector3 position)
    {
        Vector3 diff = position - transform.position;
        Vector3 projOnYAxis = Vector3.Project(diff, GetYAxis());
        Vector3 diffToYAxis = diff - projOnYAxis;
        float longitude = Vector3.Angle(diffToYAxis, GetXAxis());
        float latitude = Vector3.Angle(diffToYAxis, diff);
        return new Vector2(longitude, latitude);
    }

    public Vector3 GetPosition(Vector2 location, float altitude = 0)
    {
        float r = _radius + altitude;
        float ra = r * Mathf.Cos(location.y * Mathf.Deg2Rad);
        float x = ra * Mathf.Cos(location.x * Mathf.Deg2Rad);
        float z = ra * Mathf.Sin(location.x * Mathf.Deg2Rad);
        float y = r * Mathf.Sin(location.y * Mathf.Deg2Rad);
        return new Vector3(x, y, z);
    }

    public float GetVerticalDistancePerDegree()
    {
        float circumference = 2 * _radius * Mathf.PI;
        return circumference / 360f;
    }

    public float GetHorizontalDistancePerDegree(float latitude)
    {
        float radiusAtLatitude = 
                _radius * Mathf.Cos(Mathf.Abs(
                latitude * Mathf.Deg2Rad));
        float circumferenceAtLatitude = 
                2 * radiusAtLatitude * Mathf.PI;
        return circumferenceAtLatitude / 360f;
    }

}
