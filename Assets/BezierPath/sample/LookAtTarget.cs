using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LookAtTarget : MonoBehaviour {

	[SerializeField] Transform targetTrans;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		this.transform.LookAt (targetTrans);
	}

	[SerializeField] bool isDrawGizmos = false;
	[SerializeField] Color color = Color.red;
	[SerializeField] float arrowSize = 1.0f;

	void OnDrawGizmos ()
	{
		if(!isDrawGizmos){ return; }

		Gizmos.color = color;
		Gizmos.DrawLine (this.transform.position, targetTrans.position);

		DrawArrow (targetTrans.position, arrowSize);
	}

	void DrawArrow(Vector3 pos, float size)
	{
		Gizmos.DrawLine (pos, pos + this.transform.rotation * (new Vector3(0.5f, 0, -1) * size));
		Gizmos.DrawLine (pos, pos + this.transform.rotation * (new Vector3(-0.5f, 0, -1) * size));
	}
}
