using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Bezier;

public class ProtByDistance : MonoBehaviour {

    [SerializeField] BezierPathDrawer bezierPathDrawer;
    [SerializeField] float dist;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float ratio = dist / bezierPathDrawer.length;
        BezierPointInfo info = bezierPathDrawer.GetBezierPointInfo(ratio);

        //this.transform.position = info.point;
        //this.transform.rotation = info.rotation;

        info.point = bezierPathDrawer.transform.TransformPoint(info.point);
        info.rotation = bezierPathDrawer.transform.rotation * (info.rotation);
        this.transform.position = info.point;
        this.transform.rotation = info.rotation;
	}
}
