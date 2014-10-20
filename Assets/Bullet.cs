using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    private Vector3 velocity = Vector3.zero;
    private Vector3 center = Vector3.zero;
    private float gravity = 30f;
    private float bulletSpeed = 20;

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    public void Init(Vector3 v, Vector3 center)
    {
        this.velocity = v + v.normalized * bulletSpeed;
        this.center = center;
    }
    public void Update()
    {
        this.velocity += (center - transform.position).normalized * 
                gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
    }
}
