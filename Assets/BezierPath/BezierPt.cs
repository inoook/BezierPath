using UnityEngine;
using System.Collections;

public class BezierPt : MonoBehaviour {

	[HideInInspector] public Transform inTrans;
    [HideInInspector] public Transform outTrans;

	[HideInInspector, SerializeField] Transform pivot;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

	void OnDrawGizmos () {
		pivot.hideFlags = HideFlags.HideInHierarchy;
		pivot.localPosition = Vector3.zero;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(this.transform.position, inTrans.position);
		Gizmos.DrawLine(this.transform.position, outTrans.position);

        //
        Vector3 pt = this.transform.position;
        Quaternion quat = this.transform.rotation;
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(pt, quat * Vector3.forward);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(pt, quat * Vector3.up);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(pt, quat * Vector3.right);

        if (transform.hasChanged || inTrans.hasChanged || outTrans.hasChanged || pivot.hasChanged)
        {
            isTransformChange = true;
            transform.hasChanged = false;
            inTrans.hasChanged = false;
            outTrans.hasChanged = false;
            pivot.hasChanged = false;
        }else{
            isTransformChange = false;
        }
    }

    [HideInInspector] public bool isTransformChange = false;

}
