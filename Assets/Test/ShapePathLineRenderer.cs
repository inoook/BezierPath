using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ShapePath2;

[ExecuteInEditMode]
public class ShapePathLineRenderer : MonoBehaviour {

//	public AnimationPath animPath;
	private PathInfo pathInfo;
	public Vector3[] path;

	// Use this for initialization
	void Start () {

	}

	void Update (){
//		List<Vector3> pathPos = new List<Vector3>();
//		for (int n = 0; n < animPath.keyframes.Length; n++){
//			Vector3 pos = animPath.keyframes[n].position;
//			pathPos.Add(pos);
//		}
//		path = pathPos.ToArray();

		if(path == null || path.Length < 1){ return; }

		Shape sp = new Shape();
		
		sp.moveTo( path[0] );
		for(int n = 1; n < path.Length-1; n++)
		{
			Vector3 pos = Vector3.Lerp( path[n], path[n+1], 0.5f);
			sp.curveTo( path[n],  pos );
		}
		
		Vector3 posC = Vector3.Lerp( path[path.Length-2], path[path.Length-1], 0.5f);
		Vector3 posEnd = path[path.Length-1];
		sp.curveTo( posC,  posEnd );
		
		pathInfo = new PathInfo(sp.commands, sp.data);

		DrawLinePoints();

		//
		Point3D _p3d = pathInfo.lengthToPoint(t_length);
		p3D = new Vector3(_p3d.x, _p3d.y, _p3d.z);
	}

	public float t_length = 0;
	public Vector3 p3D;
	public float length;

	public int sliceNum = 200;
	
	private void DrawLinePoints()
	{
		length = pathInfo.length;
		float split = pathInfo.length / (sliceNum-1);
		
		LineRenderer lineRenderer = this.gameObject.GetComponent<LineRenderer>();
		lineRenderer.SetVertexCount(sliceNum);

		for(int i = 0; i < sliceNum; i++){
			//Vector3 sPos = sp.getPointInfoVec3(split * i);
			//Point3D p3d = pathInfo.lengthToPoint( split * i );
//			Debug.Log(i+" > "+split * i);
			VPoint3D p3d = pathInfo.lengthToVector( split * i );

			if(p3d == null){
				Debug.LogWarning("null: "+ i);
			}else{
				Vector3 sPos = new Vector3(p3d.x, p3d.y, p3d.z );
				lineRenderer.SetPosition(i, sPos);
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(p3D, 0.25f);
	}

}
