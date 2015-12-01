using UnityEngine;
using System.Collections;

public class BezierPt : MonoBehaviour {

	public Transform inTrans;
	public Transform outTrans;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos () {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(this.transform.position, inTrans.position);
		Gizmos.DrawLine(this.transform.position, outTrans.position);
	}
}
