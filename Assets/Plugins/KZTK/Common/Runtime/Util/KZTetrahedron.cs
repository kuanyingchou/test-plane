using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KZTetrahedron : MonoBehaviour {
    private static float HALF_PI = Mathf.PI / 2;
    private static float sqrt3 = Mathf.Sqrt(3);
    private static float a = HALF_PI - Mathf.Acos(1.0f / sqrt3);
        //Debug.Log(a * Mathf.Rad2Deg);
    private static float b = HALF_PI - 2 * a;

    //private List<GameObject> list=new List<GameObject>();

    public float length = 1;

    public void Start() {
        Vector3[] vertices = GetVertices(transform.position, length);
        //for(int i=0; i<vertices.Length; i++) {
        //    GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    o.transform.position = vertices[i];
        //    list.Add(o);
        //}

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = new int[] {0, 1, 2, 0, 3, 1, 0, 2, 3, 3, 2, 1};
        mesh.normals = GetNormals(vertices);

        gameObject.AddComponent<MeshRenderer>().material.shader = 
                Shader.Find("Diffuse");
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        
        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        o.transform.position = vertices[0];
        o.transform.localScale = new Vector3(.1f, .1f, .1f);
        o.transform.parent = transform;


    }
    public void Update() {
        //Debug.DrawRay(Vector3.zero, vertices[0], Color.red);
        //Vector3[] vertices = GetVertices(transform.position, length);
        //for(int i=0; i<vertices.Length; i++) {
        //    list[i].transform.position = vertices[i];
        //}
    }

    public static Vector3[] GetVertices(Vector3 center, float length) {
        //Debug.Log(b * Mathf.Rad2Deg);
        float bottomRadius = GetBottomRadius(length);
        float innerRadius = GetInnerRadius(bottomRadius, b);
        float lower = GetLowerHeight(bottomRadius, b);

        Vector3 p = new Vector3(0, innerRadius, 0);

        float rad30 = 30 * Mathf.Deg2Rad;

        Vector3 q = new Vector3(
                bottomRadius * Mathf.Cos(rad30), 
                - lower, 
                - bottomRadius * Mathf.Sin(rad30));

        Vector3 r = new Vector3(
                - q.x, 
                - lower, 
                q.z);

        Vector3 s = new Vector3(0, - lower, bottomRadius);

        p+=center;
        q+=center;
        r+=center;
        s+=center;

        return new Vector3[] {
            p, q, r, s
        };
    }

    private static float GetBottomRadius(float length) {
        return length / sqrt3;
    }
    private static float GetInnerRadius(float bottomRadius, float angle) {
        return bottomRadius / Mathf.Cos(angle);
    }
    private static float GetLowerHeight(float bottomRadius, float angle) {
        return bottomRadius * Mathf.Tan(angle);
        //or innerRadius * Mathf.Sin(angle);
    }
    private static float GetVerticalLength(float length) {
        return Mathf.Sqrt(length * length - Mathf.Pow(length / 2, 2));
    }

    public static Vector3[] GetNormals(Vector3[] v) {
        Vector3 a = Vector3.Cross(v[1]-v[0], v[2]-v[0]);
        Vector3 b = Vector3.Cross(v[3]-v[0], v[1]-v[0]);
        Vector3 c = Vector3.Cross(v[3]-v[0], v[2]-v[0]);
        Vector3 d = Vector3.Cross(v[2]-v[3], v[1]-v[3]);
        a.Normalize();
        b.Normalize();
        c.Normalize();
        d.Normalize();
        return new Vector3[] {a, b, c, d};
    }

    public void RotateRight() {
        Debug.Log("right");
        transform.Rotate(HorizontalRotate(), Space.World);
    }
    public void RotateLeft() {
        Debug.Log("left");
        transform.Rotate(-HorizontalRotate(), Space.World);
    }
    private Vector3 HorizontalRotate() {
        Vector3 rotation;
        if(phase == 1 || phase == 4) {
            rotation = new Vector3(0, -(Mathf.PI / 2)*Mathf.Rad2Deg, 0);
            phase = phase == 1? 4: 1;
        } else {
            rotation = new Vector3(0, -(2 * Mathf.PI / 3)*Mathf.Rad2Deg, 0);
        }
        return rotation;
    }

    int phase = 0;
    public void RotateDown() {
        Debug.Log("down");
        Vector3 rotation;
        //float angle = Mathf.Acos(length / sqrt3 / GetVerticalLength(length));
        if(phase == 0 || phase == 3) {
            rotation = new Vector3(-(HALF_PI - a)*Mathf.Rad2Deg, 0, 0);
        } else if(phase == 1 || phase == 4) { 
            rotation = new Vector3(-(a + b)*Mathf.Rad2Deg, 0, 0);
        } else { //phase == 2 || phase == 5
            rotation = new Vector3(-(HALF_PI - b)*Mathf.Rad2Deg, 0, 0);
        }
        transform.Rotate(rotation, Space.World);
        phase++;
        if(phase == 6) phase = 0;
    }
    public void RotateUp() {
        Debug.Log("up");
        Vector3 rotation;
        //float angle = Mathf.Acos(length / sqrt3 / GetVerticalLength(length));
        if(phase == 0 || phase == 3) {
            rotation = new Vector3((HALF_PI - b)*Mathf.Rad2Deg, 0, 0);
        } else if(phase == 1 || phase == 4) { 
            rotation = new Vector3((HALF_PI - a)*Mathf.Rad2Deg, 0, 0);
        } else { //phase == 2 || phase == 5
            rotation = new Vector3((a+b)*Mathf.Rad2Deg, 0, 0);
        }
        transform.Rotate(rotation, Space.World);
        phase--;
        if(phase == -1) phase = 5;
    }

}
