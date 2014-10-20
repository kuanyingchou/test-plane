using UnityEngine;
using System.Collections;
using System;

public class KZAnalogClockDisplay : MonoBehaviour {

    private KZAnalogClock clock;
    public int radius = 1;

    GameObject hourBase; 
    GameObject minuteBase;
    //GameObject secondBase;

    public GameObject CreateHand(
            GameObject origin, string name, float length, 
            float thickness, Color color) {
        GameObject hand = GameObject.CreatePrimitive(
                PrimitiveType.Quad);
        hand.transform.parent = origin.transform;
        hand.name = name;
        hand.transform.localPosition = KZUtil.V3(y: length * .5f);
        hand.transform.localScale = new Vector3(length, thickness, 1);
        hand.transform.eulerAngles = KZUtil.V3(z:90);
        KZColoredTexture hourTex = 
                hand.AddComponent<KZColoredTexture>();
        hourTex.color = color;
        return hand;
    }

    public void Start() {

        clock = new KZAnalogClock();

        Debug.Log(clock.GetHour());

        KZCircularTexture circularTex = 
                gameObject.AddComponent<KZCircularTexture>();
        circularTex.radius = radius;
        circularTex.color = Color.white;
        circularTex.UpdateProperties();

        hourBase = new GameObject();
        hourBase.name = "HourBase";
        hourBase.transform.parent = transform;
        hourBase.transform.position = 
                KZUtil.V3(z:-1); 
        transform.localScale = new Vector3(radius * 2, radius * 2, 1);

        GameObject hourHand = CreateHand(
                hourBase,
                "HourHand", .6f * radius, .05f, 
                Color.black);
        hourHand.transform.parent = hourBase.transform;

        minuteBase = new GameObject();
        minuteBase.name = "MinuteBase";
        minuteBase.transform.parent = transform;
        minuteBase.transform.position = 
                KZUtil.V3(z:-2); 

        GameObject minuteHand = CreateHand(
                minuteBase,
                "MinuteHand", .8f * radius, .04f, 
                Color.black);
        minuteHand.transform.parent = minuteBase.transform;
        //GameObject minuteHand = GameObject.CreatePrimitive(
        //        PrimitiveType.Quad);
        //minuteHand.transform.parent = minuteBase.transform;

        //secondBase = GameObject.Find("SecondBase");
    }

    public void Update() {
    /*
        DateTime now = DateTime.Now;
        clock.SetTime(now.Hour, now.Minute, now.Second);

        hourBase.transform.eulerAngles = 
                new Vector3(0, 0, -clock.GetHourAngle());
        minuteBase.transform.eulerAngles = 
                new Vector3(0, 0, -clock.GetMinuteAngle());
        //secondBase.transform.eulerAngles = 
        //        new Vector3(0, 0, -clock.GetSecondAngle());
        */
    }
}
