using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bezier;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BezierPathDrawer : MonoBehaviour
{
	public BezierPath path;

	[SerializeField] float precision = 0.05f;
	public List<BezierPt> bPoints;

    [SerializeField] bool useLocalSpace = false;
    
	// Use this for initialization
	void Awake()
	{
		if (bPoints == null || bPoints.Count < 2) { return; }
        path = GetBezierPath(precision);
	}
	

	[Header("debug")]
	public Color drawColor = Color.yellow;
	public int split = 30;

	[SerializeField, Header("処理が重い時はチェックを外すか、GameObjectを非表示に")] bool isDrawGizmos = true;
	[SerializeField] bool isDrawPoints = false;

    [SerializeField] public float _length = 0;// for Debug

	void OnDrawGizmos ()
	{
        if (!isDrawGizmos){ return; }
		//if(Application.isPlaying){ return; }

		if (bPoints == null || bPoints.Count < 2) { return; }
        
        bool isChange = false;
        // --------
        // delete point
        for (int i = bPoints.Count-1; i >= 0; i--) {
			BezierPt b = bPoints [i];
			if (b == null) {
				bPoints.RemoveAt (i);
                isChange = true;
            }
		}

        // transformの変更検知
        for (int i = 0; i < bPoints.Count; i++) {
            BezierPt b = bPoints[i];
            if (b.isTransformChange) {
                isChange = true;
                break;
            }
        }

        // --------
        // updatePath
        if (isChange || path == null){
            path = GetBezierPath(precision);
        }

        // --------
        // draw
        float delta = 1.0f / split;
		
		Gizmos.color = drawColor;
		for (int i = 0; i < split-1; i++) {
			if(i % 2 == 0){
    			float t0 = delta * i;
    			float t1 = delta * (i + 1);
    			Vector3 pos0 = GetPointAt (t0);
    			Vector3 pos1 = GetPointAt (t1);

    			Gizmos.DrawLine (pos0, pos1);
			}
		}

		if (isDrawPoints) {
			// point and tangent 
			for (int i = 0; i < split; i++) {

				float v = delta * i;
			
				BezierPointInfo pInfo = GetBezierPointInfo(v);
			
				Vector3 pt = pInfo.point;
				Gizmos.color = drawColor;
				Gizmos.DrawWireSphere (pt, 0.25f);

				// tangent
				Gizmos.color = Color.blue;
				Vector3 tangent = pInfo.tangent;
				Gizmos.DrawRay (pt, tangent);

				Quaternion quat = pInfo.rotation;
				Gizmos.color = Color.green;
				Gizmos.DrawRay (pt, quat * Vector3.up);
				Gizmos.color = Color.red;
				Gizmos.DrawRay (pt, quat * Vector3.right);
			}
		}
	}
	
	public float length {
		get {
			if (path != null) {
				return path.length;
			} else {
				return 0;
			}
		}
	}

	private Vector3 GetPointAt (float p)
	{
		if (path != null) {
			return path.point (p);
		} else {
			return Vector3.zero;
		}
	}

    public BezierPointInfo GetBezierPointInfo(float ratio, float tangentAddPer = 0.001f)
    {
        if (path != null){
            return path.GetBezierPointInfo(ratio, tangentAddPer);
        }else {
            return null;
        }
    }
    
    public BezierPath GetBezierPath(float precision = 0.05f)
	{
        List<Quaternion> rotList = new List<Quaternion>();
        for (int i = 0; i < bPoints.Count; i++) {
            BezierPt b = bPoints[i];
            if (useLocalSpace) {
                rotList.Add(b.transform.localRotation);
            } else {
                rotList.Add(b.transform.rotation);
            }
        }

        List<Vector3> nodes = new List<Vector3>();
        for (int i = 0; i < bPoints.Count - 1; i++) {
            BezierPt start = bPoints[i];
            BezierPt end = bPoints[i + 1];

            if (useLocalSpace) {
                nodes.Add(this.transform.InverseTransformPoint(start.transform.position));
                nodes.Add(this.transform.InverseTransformPoint(end.inTrans.position));
                nodes.Add(this.transform.InverseTransformPoint(start.outTrans.position));
                nodes.Add(this.transform.InverseTransformPoint(end.transform.position));
            } else {
                nodes.Add(start.transform.position);
                nodes.Add(end.inTrans.position);
                nodes.Add(start.outTrans.position);
                nodes.Add(end.transform.position);
            }
        }

        BezierPath bp = new BezierPath(nodes.ToArray(), rotList.ToArray(), precision);
        _length = bp.length;

        return bp;
    }
    
    // editor
    #if UNITY_EDITOR
    [SerializeField, HideInInspector] BezierPt pointPrefab;

	[ContextMenu("+ AddPoint")]
	void CreatePoint()
	{
        GameObject gObj = ((GameObject)PrefabUtility.InstantiatePrefab (pointPrefab.gameObject));
		gObj.transform.SetParent(this.transform);

		Transform trans = gObj.transform;

		if (bPoints == null || bPoints.Count < 1) {
			bPoints = new List<BezierPt> ();

			trans.localPosition = Vector3.zero;
			trans.localRotation = Quaternion.identity;
		}else{
			Transform preTrans = bPoints[bPoints.Count-1].transform;
			trans.localPosition = preTrans.localPosition;
			trans.localRotation = preTrans.localRotation;
		}

		BezierPt pt = gObj.GetComponent<BezierPt> ();
		bPoints.Add (pt);

		gObj.name = "Bezier ("+(bPoints.Count-1)+")";
	}

    #endif
}
