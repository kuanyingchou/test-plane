using UnityEngine;
using System.Collections;
using System;

// adjust camera size that maximizes visible area within bounds denoted by user
// [ExecuteInEditMode]
[Obsolete("Please use KZCameraAdjuster2 instead!")]
public class KZCameraAdjuster : MonoBehaviour {

    private static KZSize DEFAULT_BOUNDS = new KZSize( 1280, 800 );

    public KZSize bounds = new KZSize(DEFAULT_BOUNDS);

    private KZCameraAdjuster2 cameraAdjuster2;

    public bool update;

    public void Awake()
    {
        cameraAdjuster2 = gameObject.AddComponent<KZCameraAdjuster2>();
        UpdateProperties();
        cameraAdjuster2.AdjustCameraSize();
    }

    public void Update()
    {
        if (update) UpdateProperties();
    }

    private void UpdateProperties()
    {
        cameraAdjuster2.fixedWidth = cameraAdjuster2.fixedHeight = false;
        cameraAdjuster2.targetSize = bounds.height / 2;
        cameraAdjuster2.targetAspectRatio = (float)bounds.width / bounds.height;
        cameraAdjuster2.update = update;
    }

    
}
