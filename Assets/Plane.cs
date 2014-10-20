using UnityEngine;
using System.Collections;

public class Plane : MonoBehaviour {
    private bool isFiring = false;
    private Vector3 velocity = Vector3.zero;
    private bool isLeftGun = false;
    public float fireInterval = .05f;
    public float warmUp = .1f;
    public float bulletSpeed = .1f;
    private Vector3 center = Vector3.zero;

    public IEnumerator Start()
    {
        while(true)
        {
            if(isFiring)
            {
                Fire();
                yield return new WaitForSeconds(fireInterval);
            }
            yield return new WaitForSeconds(warmUp);
        }
    }

    public void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            if(! isFiring) StartFiring();
            //Fire();
            //Debug.Log("shoot!");
        }
        else
        {
            if(isFiring) StopFiring();
        }
    }

    public void Follow(Vector3 target, Vector3 center)
    {
        transform.LookAt(
                target, 
                target - center);
        this.center = center;
        this.velocity = target - transform.position;
        transform.position = target;
    }

    private void Fire()
    {
        GameObject bullet = 
                GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.name = "Bullet";
        bullet.transform.position = transform.TransformPoint(new Vector3(
                .5f * (isLeftGun?-1:1), -.5f, 0));;
        bullet.transform.localScale = new Vector3(.1f, .1f, .1f);
        Bullet b = bullet.AddComponent<Bullet>();
        b.Init(velocity, center);
        isLeftGun = !isLeftGun;
    }


    public void StartFiring()
    {
        isFiring = true;
    }
    public void StopFiring()
    {
        isFiring = false;
    }
}
