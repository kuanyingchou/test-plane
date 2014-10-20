using UnityEngine;
using System.Collections;
[RequireComponent(typeof( AudioSource))]
public class KZAudioLoader : MonoBehaviour {

	public string  resPath;
	void Awake()
	{
		audio.clip = KZLocale.Load(resPath) as AudioClip;
	}
}


