using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bezier;

[ExecuteInEditMode]
public class BezierPathTimeline : MonoBehaviour {

	Transform target;
	[SerializeField] BezierPathDrawer bezierPathDrawer;

	[Range(0, 1)]
	public float ratio = 0;

	[SerializeField] bool rotate = true;

	BezierPath path;

	// Use this for initialization
	void Start () {
		target = this.transform;

		if (bezierPathDrawer != null) {
			path = bezierPathDrawer.GetBezierPath ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (target == null) {
			target = this.transform;
		}

		if (bezierPathDrawer != null && Application.isEditor && !Application.isPlaying) {
			path = bezierPathDrawer.path;
		}
		if (bezierPathDrawer == null || path == null || target == null) {
			return;
		}
		// update
		BezierPointInfo info = path.GetBezierPointInfo (ratio);
		target.transform.position = info.point;
		if (rotate) {
			target.transform.rotation = info.rotation;
		}
	}
}
