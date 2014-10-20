using UnityEngine;
using System.Collections;
using System;

//[ modified from pyu's clock project

public class KZAnalogClock {

    private static readonly float HOUR_TO_DEGREE_HOUR = 30.0f;
    private static readonly float MINUTE_TO_DEGREE_MINUTE = 6;
    private static readonly float SECOND_TO_DEGREE_SECOND = 6;
    private static readonly float DEGREE_HOUR_TO_MINUTE = 2;
    private static readonly float DEGREE_MINUTE_TO_SECOND = 10;
    private static readonly float MINUTE_TO_DEGREE_HOUR = .5f;
    private static readonly float SECOND_TO_DEGREE_HOUR = .5f / 60;
    private static readonly float SECOND_TO_DEGREE_MINUTE = .1f;

    public static readonly int HOURS_IN_A_DAY = 24;

    private int hour;
    private int minute;
    private int second;

    public int GetHour(){
        return hour;
    }
    public int GetMinute(){
        return minute;
    }
    public int GetSecond() {
        return second;
    }

    public void SetHour(int hour) {
        if (hour >= 24 || hour < 0) throw new ArgumentException();
        this.hour = hour;
    }

    public void SetMinute(int minute) {
        if (minute >= 60 || minute < 0) throw new ArgumentException();
        this.minute = minute;
    }

    public void SetSecond(int second) {
        if (second >= 60 || second < 0) throw new ArgumentException();
        this.second = second;
    }

    public int AddHour(int hour) {
        return SetHourCascade(this.hour + hour);
    }
    public int AddMinute(int minute) {
        return SetMinuteCascade(this.minute + minute);
    }
    public int AddSecond(int second) {
        return SetSecondCascade(this.second + second);
    }

    public void SetTime(int hour, int minute = 0, int second = 0){
        SetHour(hour); SetMinute(minute); SetSecond(second);
    }
    
    public int SetHourCascade(int hour) {
        KZPair<int, int> hourDay = GetNormalizedTimeElement(hour, 24);
        this.hour = hourDay.Item1;
        return hourDay.Item2;
    }
    public int SetMinuteCascade(int minute) {
        KZPair<int, int> minuteHour = GetNormalizedTimeElement(minute, 60);
        this.minute = minuteHour.Item1;
        return SetHourCascade(this.hour + minuteHour.Item2);
    }
    public int SetSecondCascade(int second) {
        KZPair<int, int> secondMinute = GetNormalizedTimeElement(second, 60);
        this.second = secondMinute.Item1;
        return SetMinuteCascade(secondMinute.Item2);
    }

    private KZPair<int, int> GetNormalizedTimeElement(int timeElement, int bound) {
        if(timeElement >= bound) {
            return new KZPair<int, int> (
                timeElement % bound, 
                timeElement / bound
            );
        } else if(timeElement < 0) {
            return new KZPair<int, int> (
                bound - (-timeElement % bound), 
                -1 - (-timeElement) / bound
            );
        } else {
            return new KZPair<int, int>(timeElement, 0);
        }
    }

    public bool IsMorning(){
        return hour < 12 ;
    }

    public float GetHourAngle(){
        return GetDegreeOfHourHand(hour, minute, second);
    }
    public float GetMinuteAngle(){
        return GetDegreeOfMinuteHand(minute, second);
    }
    public float GetSecondAngle(){
        return GetDegreeOfSecondHand(second);
    }

    private static float GetDegreeOfHourHand(
            int hour, int minute, int second) {
        return (hour % 12) * HOUR_TO_DEGREE_HOUR + 
                minute * MINUTE_TO_DEGREE_HOUR + 
                second * SECOND_TO_DEGREE_HOUR;
    }
    
    private static float GetDegreeOfMinuteHand(int minute, int second) {
        return minute * MINUTE_TO_DEGREE_MINUTE + 
                second * SECOND_TO_DEGREE_MINUTE;
    }

    private static float GetDegreeOfSecondHand(int second) {
        return second * SECOND_TO_DEGREE_SECOND;
    }

//======================================================

    public void SetHourAngle(float angle){
        SetHourCascade((int)(angle / HOUR_TO_DEGREE_HOUR));

        float extraHourAngle = angle % HOUR_TO_DEGREE_HOUR;
        SetMinuteAngle(extraHourAngle * DEGREE_HOUR_TO_MINUTE * 6);
    } 

    public void SetMinuteAngle(float angle){
        if (angle >= 360.0f){
            AddHour((int)(angle/360.0f));
        }
        angle = angle % 360.0f;
        minute =(int)(angle/6.0f);
        angle = angle % 6.0f;
        second = Mathf.RoundToInt(angle * DEGREE_MINUTE_TO_SECOND);
    } //minute effects hour


//  public void SetSecondAngle(float angle); //second effects minute & hour
    public void AddHourAngle(float angle){
        int minDiff = (int)(angle*2);
        hour += minDiff/60;
        minute += minDiff%60;
        if(minute<0){
            minute+=60;
            hour -=1;
        }else{
            hour += minute/60;
        }
        hour%=24;
        minute%=60;
        if(hour <0){
            hour+=24;
        }
    }

    public void AddMinuteAngle(float angle){
        minute += (int)(angle/6);
        if(minute < 0){
            hour +=(minute /60)-(minute%60 ==0 ?0:1);
        }else{
            hour += minute/60;
        }
        hour%=24;
        minute%=60;
        if(hour <0){
            hour+=24;
        }
        if(minute <0){
            minute+=60;
        }
    }

    public void SetMinuteSnap(){
        if((minute*6)%30 <15 ){
            minute =(5*(minute/5));
        }else{
            minute= (minute +(5-(minute%5)));
            if(minute ==60){
                hour+=1;
            }
        }
        hour%=24;
        minute%=60;
    }

    public void SetHourSnap(){
        if(minute>30){
            hour+=1;
        }
        minute=0;
        hour%=24;
        minute%=60;
    }
//  public float GetSecondAngle();
}


