using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

using UnityEditor;


public class TimelineController : MonoBehaviour {

	[SerializeField]
	PlayableDirector playableDirector;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void PlayTimeline()
	{
		playableDirector.Play ();
	}
	void StopTimeline()
	{
		playableDirector.Stop ();
	}
	void PauseTimeline()
	{
		playableDirector.Pause ();
	}
	void ResumeTimeline()
	{
		playableDirector.Resume ();
	}

	[SerializeField] Rect drawRect = new Rect (10,10,100,100);
	[SerializeField] GUISkin skin;
	void OnGUI()
	{
		GUI.skin = skin;
		GUILayout.BeginArea (drawRect);
		GUILayout.Label ( ((float)(playableDirector.time / playableDirector.duration)).ToString() );
		GUILayout.Label ("time: "+playableDirector.time);
		playableDirector.time = GUILayout.HorizontalSlider ((float)playableDirector.time, 0, (float)playableDirector.duration);

//		if (GUILayout.Button ("PlayTimeline")) {
//			PlayTimeline ();
//		}
//		if (GUILayout.Button ("PauseTimeline")) {
//			PauseTimeline ();
//		}
//		if (GUILayout.Button ("ResumeTimeline")) {
//			ResumeTimeline ();
//		}
//		if (GUILayout.Button ("StopTimeline")) {
//			StopTimeline ();
//		}
		GUILayout.EndArea ();
	}
}
