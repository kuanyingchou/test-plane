using UnityEngine;
using System.Collections;

public class AerialObject : MonoBehaviour
{
    public Planet planet;
    public float altitude = 1;
    public Vector2 direction = new Vector2(0, 0);
    public float speed = 1f;
    public Vector2 location = new Vector2(0, 0);

    public void Start()
    {
    }

    public void Update()
    {
        Vector2 diff = direction * speed * Time.deltaTime;
        location = new Vector2(
                location.x + 
                diff.x / planet.GetHorizontalDistancePerDegree(location.y),
                location.y + 
                diff.y / planet.GetVerticalDistancePerDegree());
        transform.position = planet.GetPosition(location, altitude);
    }

}
