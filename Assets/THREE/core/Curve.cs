/**
 * @author zz85 / http://www.lab4games.net/zz85/blog
 * Extensible curve object
 *
 * Some common of Curve methods
 * .getPoint(t), getTangent(t)
 * .getPointAt(u), getTagentAt(u)
 * .getPoints(), .getSpacedPoints()
 * .getLength()
 * .updateArcLengths()
 *
 * This following classes subclasses THREE.Curve:
 *
 * -- 2d classes --
 * THREE.LineCurve
 * THREE.QuadraticBezierCurve
 * THREE.CubicBezierCurve
 * THREE.SplineCurve
 * THREE.ArcCurve
 * THREE.EllipseCurve
 *
 * -- 3d classes --
 * THREE.LineCurve3
 * THREE.QuadraticBezierCurve3
 * THREE.CubicBezierCurve3
 * THREE.SplineCurve3
 * THREE.ClosedSplineCurve3
 *
 * A series of curves can be represented as a THREE.CurvePath
 *
 **/

/**************************************************************
 *	Abstract Curve base class
 **************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
	public class Curve
	{

		public Curve ()
		{
			needsUpdate = true;
		}

		// Virtual base class method to overwrite and implement in subclasses
		//	- t [0 .. 1]
		public virtual Vector3 getPoint (float t)
		{
			Debug.Log ("Warning, getPoint() not implemented!");
			return Vector3.zero;
		}

		// Get point at relative position in curve according to arc length
		// - u [0 .. 1]
		public virtual Vector3 getPointAt (float u)
		{
			float t = this.getUtoTmapping (u);
			return this.getPoint (t);
		}

		// Get sequence of points using getPoint( t )

		//public virtual List<Vector3> getPoints (float divisions = 5, bool closedPath = false)
		public virtual List<Vector3> getPoints (float divisions = 12, bool closedPath = false)
		{
			List<Vector3> pts = new List<Vector3> ();
		
			for (int d = 0; d <= divisions; d ++) {
				pts.Add (this.getPoint ((float)d / divisions));
			}
			return pts;
		
		}

		// Get sequence of points using getPointAt( u )

		public virtual List<Vector3> getSpacedPoints (float divisions = 5, bool closedPath = false)
		{
			List<Vector3> pts = new List<Vector3> ();
		
			for (int d = 0; d <= divisions; d ++) {
				pts.Add (this.getPointAt ((float)d / divisions));
			}
			return pts;	
		}

		// Get total curve arc length

		public virtual float getLength ()
		{
		
			List<float> lengths = this.getLengths ();
			return lengths [lengths.Count - 1];	
		}

		// Get list of cumulative segment lengths

		float __arcLengthDivisions;
		List<float> cacheArcLengths;
		bool needsUpdate = true;

		public List<float> getLengths (float divisions = 200)
		{
			//if ( !divisions ) divisions = (this.__arcLengthDivisions) ? (this.__arcLengthDivisions): 200;
			/*
		if(this.__arcLengthDivisions != null){
			divisions = this.__arcLengthDivisions;
		}
		*/
		
			if (this.cacheArcLengths != null
				&& (this.cacheArcLengths.Count == divisions + 1)
				&& !this.needsUpdate) {
			
				//console.log( "cached", this.cacheArcLengths );
				return this.cacheArcLengths;

			}
		
			this.needsUpdate = false;
		
			List<float> cache = new List<float> ();
			Vector3 current;
			Vector3 last = this.getPoint (0);
			float sum = 0;
		
			cache.Add (0);
		
			for (int p = 1; p <= divisions; p ++) {
			
				current = this.getPoint ((float)p / divisions);

				//sum += current.distanceTo( last );
				float distance = Vector3.Distance (current, last);
				sum += distance;
				cache.Add (sum);
				last = current;
			}
		
			this.cacheArcLengths = cache;
		
			return cache; // { sums: cache, sum:sum }; Sum is in the last element.
		
		}

		void updateArcLengths ()
		{
			this.needsUpdate = true;
			this.getLengths ();
		}

		// Given u ( 0 .. 1 ), get a t to find p. This gives you points which are equi distance

		//float getUtoTmapping ( float u, float distance ) {
		public float getUtoTmapping (float u)
		{
			List<float> arcLengths = this.getLengths ();
			int il = arcLengths.Count;
			float dist = u * arcLengths [il - 1];
			return getUtoTmapping (u, dist);
		}

		public float getUtoTmapping (float u, float distance)
		{
			List<float> arcLengths = this.getLengths ();
		
			int i = 0;
			int il = arcLengths.Count;
		
			float targetArcLength; // The targeted u distance value to get

			/*
		if ( distance ) {
			targetArcLength = distance;
		} else {
			targetArcLength = u * arcLengths[ il - 1 ];
		}
		*/
			//targetArcLength = u * arcLengths[ il - 1 ];
			targetArcLength = distance;
			
			// binary search for the index with largest value smaller than target u distance
		
			int low = 0, high = il - 1;
			float comparison;
		
			while (low <= high) {
			
				i = Mathf.FloorToInt (low + (high - low) / 2.0f); // less likely to overflow, though probably not issue here, JS doesn't really have integers, all numbers are floats
			
				comparison = arcLengths [i] - targetArcLength;
			
				if (comparison < 0) {
				
					low = i + 1;
					continue;
				
				} else if (comparison > 0) {
				
					high = i - 1;
					continue;
				
				} else {
				
					high = i;
					break;
					// DONE
				}
			
			}
		
			i = high;
		
			//console.log('b' , i, low, high, Date.now()- time);
			float t;
			if (arcLengths [i] == targetArcLength) {
				if (il - 1 == 0) {
					return 0;
				}

				t = (float)i / (float)(il - 1);
				return t;
			
			}
		
			// we could get finer grain at lengths, or use simple interpolatation between two points
		
			float lengthBefore = arcLengths [i];
			float lengthAfter = arcLengths [i + 1];
		
			float segmentLength = lengthAfter - lengthBefore;
		
			// determine where we are between the 'before' and 'after' points
		
			float segmentFraction = (targetArcLength - lengthBefore) / segmentLength;
		
			// add that fractional amount to t
		
			t = (float)(i + segmentFraction) / (float)(il - 1);
		
			return t;
		
		}

		// Returns a unit vector tangent at t
		// In case any sub curve does not implement its tangent derivation,
		// 2 points a small delta apart will be used to find its gradient
		// which seems to give a reasonable approximation
	
		public virtual Vector3 getTangent (float t)
		{
		
			float delta = THREE.Setting.EPSILON_S;
			float t1 = t - delta;
			float t2 = t + delta;
		
			// Capping in case of danger
		
			if (t1 < 0)
				t1 = 0;
			if (t2 > 1)
				t2 = 1;
		
			Vector3 pt1 = this.getPoint (t1);
			Vector3 pt2 = this.getPoint (t2);
		
			//Vector3 vec = pt2.clone().sub(pt1);
			Vector3 vec = pt2 - pt1;
			return vec.normalized;
		
		}
	
		public Vector3 getTangentAt (float u)
		{
			float t = this.getUtoTmapping (u);
			return this.getTangent (t);
		
		}


		/**************************************************************
	 *	Utils
	 **************************************************************/

		public class Utils
		{

			public static float tangentQuadraticBezier (float t, float p0, float p1, float p2)
			{
				return 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
			}
		
			// Puay Bing, thanks for helping with this derivative!
		
			public static Vector3 tangentCubicBezier (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
			{
			
				return -3 * p0 * (1 - t) * (1 - t) +
					3 * p1 * (1 - t) * (1 - t) - 6 * t * p1 * (1 - t) +
					6 * t * p2 * (1 - t) - 3 * t * t * p2 +
					3 * t * t * p3;
			}

			public static float tangentSpline (float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
			{ // TODO: CHECK//////////
			
				// To check if my formulas are correct
			
				float h00 = 6 * t * t - 6 * t; 	// derived from 2t^3 − 3t^2 + 1
				float h10 = 3 * t * t - 4 * t + 1; // t^3 − 2t^2 + t
				float h01 = -6 * t * t + 6 * t; 	// − 2t3 + 3t2
				float h11 = 3 * t * t - 2 * t;	// t3 − t2
			
				return h00 + h10 + h01 + h11;
			
			}
		
			// Catmull-Rom
			public static float interpolate (float p0, float p1, float p2, float p3, float t)
			{
			
				float v0 = (p2 - p0) * 0.5f;
				float v1 = (p3 - p1) * 0.5f;
				float t2 = t * t;
				float t3 = t * t2;
				return (2 * p1 - 2 * p2 + v0 + v1) * t3 + (- 3 * p1 + 3 * p2 - 2 * v0 - v1) * t2 + v0 * t + p1;
			}
		}

	}
}
