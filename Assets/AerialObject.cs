using UnityEngine;
using System.Collections;

public class AerialObject : MonoBehaviour
{
    public Planet planet;
    public float altitude = 1;
    public Vector2 direction = new Vector2(0, 0);
    public float speed = 1f;
    public Vector2 location = new Vector2(0, 0); //in degrees

    public void Start()
    {
        planet.AddBody(this);
        HeadTo(new Vector2(0, 45));
    }

    public void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 diff = direction * speed * Time.deltaTime;
        location = new Vector2(
                location.x + 
                diff.x / planet.GetHorizontalDistancePerDegree(location.y),
                location.y + 
                diff.y / planet.GetVerticalDistancePerDegree());
        transform.position = planet.GetPosition(location, altitude);
        Debug.Log("current location: "+location);
    }

    public void HeadTo(Vector2 destination)
    {
        Vector2 diff = destination - location;
        float h = diff.x * 
                planet.GetHorizontalDistancePerDegree(location.y);
        float v = diff.y * planet.GetVerticalDistancePerDegree();
        direction = new Vector2(h, v).normalized;
        Debug.Log("at "+location+", head to "+destination+", dir = "+direction);
    }
}
