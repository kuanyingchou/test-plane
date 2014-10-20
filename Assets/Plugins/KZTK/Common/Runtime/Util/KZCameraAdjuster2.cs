using UnityEngine;
using System.Collections;

// adjust camera size that maximizes visible area within bounds denoted by user
[ExecuteInEditMode]
public class KZCameraAdjuster2 : MonoBehaviour {

    public float targetSize = 666;
    public float targetAspectRatio = 2048f / 1332;
    public Rect targetViewportRect = new Rect(0, 0, 1, 1);

    public bool fixedWidth = false;
    public bool fixedHeight = true;

    public Color backdropColor = Color.black; 
    private string backdropName = "_backdrop";
    public bool update;
    //private GameObject backDrop;

    public void Awake () {
        AdjustCameraSize();
    }

    public void Update () {
        if(update) {
            AdjustCameraSize();
        }
    }

    public void AdjustCameraSize(){
        float vr = GetViewportRatio();
        if(fixedWidth) {
            if(fixedHeight) {
                FixedWidthFixedHeight(vr);
                CreateBackdrop();
            } else {
                FixedWidthFlexibleHeight(vr);
            }
        } else {
            if(fixedHeight) {
                FlexibleWidthFixedHeight();
            } else {
                FlexibleHeightTopBottom(vr);
            }
        }
    }

    private void FlexibleHeightTopBottom(float viewportRatio) {
        if(viewportRatio > targetAspectRatio) {
            camera.orthographicSize = 
                    targetSize * targetAspectRatio / 
                    viewportRatio;
        } else {
            camera.orthographicSize = targetSize;
        }
        camera.rect = targetViewportRect;
    }

    private void FixedWidthFlexibleHeight(float viewportRatio) {
        if(viewportRatio < targetAspectRatio) {
            camera.orthographicSize = 
                    targetSize * targetAspectRatio / 
                    viewportRatio;
            //targetSize * 2 * targetAspectRatio / viewportRatio / 2;
        } else {
            FlexibleHeightTopBottom(viewportRatio);
        }
        camera.rect = targetViewportRect;
    }
    private void FlexibleWidthFixedHeight() {
        camera.orthographicSize = targetSize;
        camera.rect = targetViewportRect;
    }

    private float GetViewportRatio() {
        KZSize screenSize = new KZSize( 
                Screen.width * targetViewportRect.width, 
                Screen.height * targetViewportRect.height);  
        return screenSize.width / screenSize.height;
    }

    public void FixedWidthFixedHeight(float viewportRatio) {
        //camera.aspect = targetAspectRatio; //will distort the view
        camera.orthographicSize = targetSize;
        Rect original = targetViewportRect;

        //[ >>> don't know how I ended up with this, but it works!
        if(targetAspectRatio > viewportRatio) {
            SetCameraRect(
                original.x,
                original.y * (viewportRatio / targetAspectRatio) + 
                        ((targetAspectRatio - viewportRatio) / 
                        targetAspectRatio * .5f),
                original.width,
                original.height * (viewportRatio / targetAspectRatio)
            );
        } else if(targetAspectRatio < viewportRatio) {
            SetCameraRect(
                original.x * (targetAspectRatio / viewportRatio) + 
                        (viewportRatio - targetAspectRatio) / 
                        viewportRatio * .5f,
                original.y,
                original.width * (targetAspectRatio / viewportRatio),
                original.height
            );
        } else {
            SetCameraRect(original);
        }
    }
    private void SetCameraRect(float x, float y, float w, float h) {
        camera.rect = new Rect(x, y, w, h);
    }
    private void SetCameraRect(Rect r) {
        camera.rect = r;
    }

    private void CreateBackdrop() {
        GameObject o = GameObject.Find(backdropName);
        if(o == null) {
            o = new GameObject();
            o.name = backdropName;
        }
        Camera c = o.GetComponent<Camera>();
        if(c == null) {
            c = o.AddComponent<Camera>();
        }
        c.depth = -2; //>>> how about conflicts?
        c.cullingMask = 0; //I'm blind
        c.backgroundColor = backdropColor;
        c.orthographic = true; //maybe orthographic is cheaper

        //backDrop = o;
    }

    private float GetScreenAspectRatio() {
        return (float)Screen.width / Screen.height; //e.g. 1.7
    }

}
