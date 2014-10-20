using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//>>>> maybe an event-based model is more suitable, like:
// 1:
//   a=====
//         b=====
// 2:
//   a=====
//            b=====
// 3:
//   a=====
//      b=====

// Used to play audio clips from a list, will pause when Time.timeScale == 0 if
// ignoreTimeScale == false.

// States:
//   stopped, playing, paused

[RequireComponent (typeof(AudioSource))]
public class KZAudioPlayer : MonoBehaviour {

    public List<AudioClip> playlist;
    public bool ignoreTimeScale = false;

    public bool isPlaying = false;
    private int current = 0;
    private float elapsedTime = 0;
    public bool loop = false;
    private bool timeStopped = false;

    public bool autoPlaying = true;

    public void Start() {
        if(autoPlaying) {
            Play();
        }
    }
    
    public void Update() {
        if( ! audio.isPlaying) { // was not playing
            if(isPlaying) { // gotta play now
                if( ! timeStopped) {
                    if(elapsedTime == 0 && current == 0) { // came from stopped state
                        audio.clip = playlist[current]; // reset clip
                        KZDebug.Log("play "+current);
                    }
                    audio.Play();
                }
            } else {
                //: was not playing and does not intend to play, nothing to do
            }
        } else { // was playing
            if(isPlaying) { // is playing, update elapsed time
                float deltaTime = GetDeltaTime(ignoreTimeScale);
                if( ! ignoreTimeScale && deltaTime == 0) { // respect timescale
                    audio.Pause();
                    timeStopped=true;
                }
                elapsedTime += deltaTime;
KZDebug.Log("elapsedTime: " + elapsedTime + " / " + playlist[current].length);
                float overflow = elapsedTime - playlist[current].length;
                if(overflow >= 0) { 
                    elapsedTime = overflow;
                    audio.Stop();
KZDebug.Log("stop playing "+current);
                    if(current < playlist.Count - 1) { // still has unplayed clips
                        audio.clip = playlist[++current];
                        //audio.time = elapsedTime;
                        audio.Play();
KZDebug.Log("start playing "+current);
                    } else { // has no unplayed clips
                        if(loop) { // rewind
                            current = 0;
                            audio.clip = playlist[current];
                            audio.Play();
                        } else { // done playing
                            Stop();
                        }
                    }
                }
            } else { // do not want to play any more
                if(current == 0 && elapsedTime == 0) {
                    audio.Stop();
                } else {
                    audio.Pause();
                }
            }
        }


        //if(Time.timeScale == 0) {
        //    if(isPlaying) {
        //        isPlaying = false;
        //        audio.Pause();
        //    }
        //} else {
        //    if(isPlaying) {
        //        if(!audio.isPlaying) {
        //            if(elapsedTime == 0) {
        //                audio.clip = playlist[current];
        //            }
        //            audio.Play();
        //            //Debug.Log("start playing "+current+"("+playlist[current].length+")");
        //        } else {
        //            elapsedTime += GetDeltaTime(ignoreTimeScale);
        //            if(elapsedTime >= playlist[current].length) {
        //                audio.Stop();
        //                current++;
        //                elapsedTime = 0;
        //                if(current >= playlist.Count) {
        //                    current = 0;
        //                    if(!loop) {
        //                        isPlaying = false;
        //                    }
        //                }
        //                //Debug.Log("change index from " + old +" to "+current);
        //            }
        //        }
        //    } else {
        //        audio.Stop();
        //        //>>>> should handle pause
        //        //Debug.Log("stop playing "+current);
        //    }   
        //        
        //    
        //}
    }

    public KZAudioPlayer Add(AudioClip clip, float delay) {
        return this;
    }

    public void Play(){
        isPlaying = true;
    }
    public void Pause() {
        isPlaying = false;
    }
    public void Stop(){
        isPlaying = false;
        elapsedTime = 0;
        current = 0;
    }    

    public void SetVolume(float v) {
        audio.volume = v;
    }
    
    private static float lastTime = 0;
    private static float GetDeltaTime(bool ignoreTimeScale) {
        float d = Time.realtimeSinceStartup - lastTime; 
        lastTime += d;
        //] in case that ignoreTimeScale has changed
        if(ignoreTimeScale) {
            return d;
        } else {
            return Time.deltaTime; //>>>
        }
    }
}
