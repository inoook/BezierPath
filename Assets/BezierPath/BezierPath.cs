using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Bezier{
	// original LeanTween: https://www.assetstore.unity3d.com/jp/#!/content/3595
	public class BezierPath{
		public Vector3[] pts;
		public float length;
		public bool orientToPath;
		public bool orientToPath2d;
		
		private Bezier[] beziers;
		private float[] lengthRatio;
		private int currentBezier=0,previousBezier=0;
		
		public BezierPath(){ }
		public BezierPath( Vector3[] pts_ , Quaternion[] rots){
			setPoints( pts_ , rots);
		}
		
		public void setPoints( Vector3[] pts_, Quaternion[] rots ){
			if(pts_.Length<4)
				Debug.LogError( "LeanTween - When passing values for a vector path, you must pass four or more values!" );
			if(pts_.Length%4!=0)
				Debug.LogError( "LeanTween - When passing values for a vector path, they must be in sets of four: controlPoint1, controlPoint2, endPoint2, controlPoint2, controlPoint2..." );
			
			pts = pts_;
			
			int k = 0;
			beziers = new Bezier[ pts.Length / 4 ];
			lengthRatio = new float[ beziers.Length ];
			int i;
			length = 0;
			for(i = 0; i < pts.Length; i+=4){
				beziers[k] = new Bezier(pts[i+0],pts[i+2],pts[i+1],pts[i+3],0.05f, rots[i/4], rots[i/4+1]);
				length += beziers[k].length;
				k++;
			}
			// Debug.Log("beziers.Length:"+beziers.Length + " beziers:"+beziers);
			for(i = 0; i < beziers.Length; i++){
				lengthRatio[i] = beziers[i].length / length;
			}
		}
		
		/**
		* Retrieve a point along a path
		* 
		* @method point
		* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
		* @return {Vector3} Vector3 position of the point along the path
		* @example
		* transform.position = ltPath.point( 0.6f );
		*/
		public Vector3 point( float ratio ){
			float added = 0.0f;
			for(int i = 0; i < lengthRatio.Length; i++){
				added += lengthRatio[i];
				if(added >= ratio)
					return beziers[i].point( (ratio-(added-lengthRatio[i])) / lengthRatio[i] );
			}
			return beziers[lengthRatio.Length-1].point( 1.0f );
		}
		public Quaternion rot( float ratio ){
			float added = 0.0f;
			for(int i = 0; i < lengthRatio.Length; i++){
				added += lengthRatio[i];
				if(added >= ratio)
					return beziers[i].rot( (ratio-(added-lengthRatio[i])) / lengthRatio[i] );
			}
			return beziers[lengthRatio.Length-1].rot( 1.0f );
		}

		public BezierPointInfo GetBezierPointInfo( float ratio ){
			float added = 0.0f;
			for(int i = 0; i < lengthRatio.Length; i++){
				added += lengthRatio[i];
				if(added >= ratio){
					float t = (ratio-(added-lengthRatio[i])) / lengthRatio[i] ;
					return beziers[i].GetPointInfo(t);
				}
			}
			return beziers[lengthRatio.Length-1].GetPointInfo( 1.0f );
		}
		
		public void place2d( Transform transform, float ratio ){
			transform.position = point( ratio );
			ratio += 0.001f;
			if(ratio<=1.0f){
				Vector3 v3Dir = point( ratio ) - transform.position;
				float angle = Mathf.Atan2(v3Dir.y, v3Dir.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0, 0, angle);
			}
		}
		
		public void placeLocal2d( Transform transform, float ratio ){
			transform.localPosition = point( ratio );
			ratio += 0.001f;
			if(ratio<=1.0f){
				Vector3 v3Dir = transform.parent.TransformPoint( point( ratio ) ) - transform.localPosition;
				float angle = Mathf.Atan2(v3Dir.y, v3Dir.x) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0, 0, angle);
			}
		}
		
		/**
		* Place an object along a certain point on the path (facing the direction perpendicular to the path)
		* 
		* @method place
		* @param {Transform} transform:Transform the transform of the object you wish to place along the path
		* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
		* @example
		* ltPath.place( transform, 0.6f );
		*/
		public void place( Transform transform, float ratio ){
			place( transform, ratio, Vector3.up );
			
		}
		
		/**
		* Place an object along a certain point on the path, with it facing a certain direction perpendicular to the path
		* 
		* @method place
		* @param {Transform} transform:Transform the transform of the object you wish to place along the path
		* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
		* @param {Vector3} rotation:Vector3 the direction in which to place the transform ex: Vector3.up
		* @example
		* ltPath.place( transform, 0.6f, Vector3.left );
		*/
		public void place( Transform transform, float ratio, Vector3 worldUp ){
			transform.position = point( ratio );
			ratio += 0.001f;
			if(ratio<=1.0f)
				transform.LookAt( point( ratio ), worldUp );
			
		}
		
		/**
		* Place an object along a certain point on the path (facing the direction perpendicular to the path) - Local Space, not world-space
		* 
		* @method placeLocal
		* @param {Transform} transform:Transform the transform of the object you wish to place along the path
		* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
		* @example
		* ltPath.placeLocal( transform, 0.6f );
		*/
		public void placeLocal( Transform transform, float ratio ){
			placeLocal( transform, ratio, Vector3.up );
		}
		
		/**
		* Place an object along a certain point on the path, with it facing a certain direction perpendicular to the path - Local Space, not world-space
		* 
		* @method placeLocal
		* @param {Transform} transform:Transform the transform of the object you wish to place along the path
		* @param {float} ratio:float ratio of the point along the path you wish to receive (0-1)
		* @param {Vector3} rotation:Vector3 the direction in which to place the transform ex: Vector3.up
		* @example
		* ltPath.placeLocal( transform, 0.6f, Vector3.left );
		*/
		public void placeLocal( Transform transform, float ratio, Vector3 worldUp ){
			transform.localPosition = point( ratio );
			ratio += 0.001f;
			if(ratio<=1.0f)
				transform.LookAt( transform.parent.TransformPoint( point( ratio ) ), worldUp );
		}
		
		public void gizmoDraw(float t = -1.0f)
		{
			Vector3 prevPt = point(0);
			
			for (int i = 1; i <= 120; i++)
			{
				float pm = (float)i / 120f;
				Vector3 currPt2 = point(pm);
				//Gizmos.color = new Color(UnityEngine.Random.Range(0f,1f),UnityEngine.Random.Range(0f,1f),UnityEngine.Random.Range(0f,1f),1);
				Gizmos.color = (previousBezier == currentBezier) ? Color.magenta : Color.grey;
				Gizmos.DrawLine(currPt2, prevPt);
				prevPt = currPt2;
				previousBezier = currentBezier;
			}
		}
	}

	
	public class Bezier{
		public float length;
		
		private Vector3 a;
		private Vector3 aa;
		private Vector3 bb;
		private Vector3 cc;
		private float len;
		private float[] arcLengths;

		private Quaternion startRot;
		private Quaternion endRot;
		
		public Bezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float precision, Quaternion startRot_, Quaternion endRot_){
			this.a = a;
			aa = (-a + 3*(b-c) + d);
			bb = 3*(a+c) - 6*b;
			cc = 3*(b-a);
			
			this.len = 1.0f / precision;
			arcLengths = new float[(int)this.len + (int)1];
			arcLengths[0] = 0;
			
			Vector3 ov = a;
			Vector3 v;
			float clen = 0.0f;
			for(int i = 1; i <= this.len; i++) {
				v = bezierPoint(i * precision);
				clen += (ov - v).magnitude;
				this.arcLengths[i] = clen;
				ov = v;
			}
			this.length = clen;

			//
			this.startRot = startRot_;
			this.endRot = endRot_;
		}
		
		private float map(float u) {
			float targetLength = u * this.arcLengths[(int)this.len];
			int low = 0;
			int high = (int)this.len;
			int index = 0;
			while (low < high) {
				index = low + ((int)((high - low) / 2.0f) | 0);
				if (this.arcLengths[index] < targetLength) {
					low = index + 1;
				} else {
					high = index;
				}
			}
			if(this.arcLengths[index] > targetLength)
				index--;
			if(index<0)
				index = 0;
			
			return (index + (targetLength - arcLengths[index]) / (arcLengths[index + 1] - arcLengths[index])) / this.len;
		}
		
		private Vector3 bezierPoint(float t){
			return ((aa* t + (bb))* t + cc)* t + a;
		}
		
		public Vector3 point(float t){ 
			return bezierPoint( map(t) ); 
		}

		public Quaternion rot(float t){ 
			return Quaternion.Lerp(startRot, endRot, map(t)); 
		}

		public BezierPointInfo GetPointInfo(float t)
		{
			BezierPointInfo pInfo = new BezierPointInfo();
			float v = map(t);
			pInfo.point = bezierPoint( v ); 

			Vector3 tangent = (this.point (v + 0.001f) - this.point (v)).normalized;
			pInfo.tangent = tangent;

			Quaternion q = Quaternion.Lerp(startRot, endRot, v);
			Vector3 up = Vector3.Cross (tangent, q * Vector3.right);
			pInfo.rotation = Quaternion.LookRotation(tangent, up); 

			return pInfo;
		}
	}

	/// <summary>
	/// Bezier point info.
	/// </summary>
	public class BezierPointInfo
	{
		public Vector3 point;
		public Quaternion rotation;
		public Vector3 tangent;
	}
}