/**
 * @author zz85 / http://www.lab4games.net/zz85/blog
 *
 **/

/**************************************************************
 *	Curved Path - a curve path is simply a array of connected
 *  curves, but retains the api of a curve
 **************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
	public class BoundingBox
	{
		public float minX;
		public float minY;
		public float maxX;
		public float maxY;
		public float minZ;
		public float maxZ;
	}

	public class Bend : Curve
	{

	}

	public class CurvePath : Curve
	{

		public List<Curve> curves;
		public List<Bend> bends;
	
		//bool autoClose = false; // Automatically closes the path

		public CurvePath ()
		{
			//autoClose = false;
			curves = new List<Curve> ();
			bends = new List<Bend> ();
		}

		public void add (Curve curve)
		{
			curves.Add (curve);
		}

		public void checkConnection ()
		{
			// TODO
			// If the ending of curve is not connected to the starting
			// or the next curve, then, this is not a real path
		}

		public void closePath ()
		{
			// TODO Test
			// and verify for vector3 (needs to implement equals)
			// Add a line curve if start and end of lines are not connected
			Vector3 startPoint = this.curves [0].getPoint (0);
			Vector3 endPoint = this.curves [this.curves.Count - 1].getPoint (1);

			if (!startPoint.Equals (endPoint)) {
				this.curves.Add (new LineCurve (endPoint, startPoint));
			}
		}

		// To get accurate point with reference to
		// entire path distance at time t,
		// following has to be done:
	
		// 1. Length of each sub path have to be known
		// 2. Locate and identify type of curve
		// 3. Get t for the curve
		// 4. Return curve.getPointAt(t')
		public override Vector3 getPoint (float t)
		{
		
			float d = t * this.getLength ();
			List<float> curveLengths = this.getCurveLengths ();
			int i = 0;
			float diff;
			Curve curve;
		
			// To think about boundaries points.
		
			while (i < curveLengths.Count) {
			
				if (curveLengths [i] >= d) {
				
					diff = curveLengths [i] - d;
					curve = this.curves [i];
				
					float u = 1.0f - diff / curve.getLength ();
				
					return curve.getPointAt (u);

					//break;
				}
			
				i ++;
			
			}

			//return null;
			return Vector3.zero;//TODO: CHECK//////
		
			// loop where sum != 0, sum > d , sum+1 <d
		}

		/*
		THREE.CurvePath.prototype.getTangent = function( t ) {
		};*/
	
		// We cannot use the default THREE.Curve getPoint() with getLength() because in
		// THREE.Curve, getLength() depends on getPoint() but in THREE.CurvePath
		// getPoint() depends on getLength
	
		public override float getLength ()
		{
			List<float> lens = this.getCurveLengths ();
			return lens [lens.Count - 1];	
		}

		// Compute lengths and cache them
		// We cannot overwrite getLengths() because UtoT mapping uses it.
		List<float> cacheLengths;

		List<float> getCurveLengths ()
		{
		
			// We use cache values if curves and cache array are same length
		
			if (this.cacheLengths != null && this.cacheLengths.Count == this.curves.Count) { // TODO: CHECK/////////
			
				return this.cacheLengths;
			
			}
			;
		
			// Get length of subsurve
			// Push sums into cached array
		
			List<float> lengths = new List<float> ();
			float sums = 0;
			int i, il = this.curves.Count;
		
			for (i = 0; i < il; i ++) {
			
				sums += this.curves [i].getLength ();
				lengths.Add (sums);
			
			}
		
			this.cacheLengths = lengths;
		
			return lengths;
		
		}

		// Returns min and max coordinates
	
		public BoundingBox getBoundingBox ()
		{
		
			List<Vector3> points = this.getPoints ();
		
			float maxX, maxY, maxZ;
			float minX, minY, minZ;
		
			maxX = maxY = maxZ = Mathf.NegativeInfinity;
			minX = minY = minZ = Mathf.Infinity;

			Vector3 p;
			int i, il;
			Vector3 sum;

			//var v3 = points[0] instanceof THREE.Vector3;
			//Vector3 v3 = points[0];
		
			//sum = v3 ? new THREE.Vector3() : new THREE.Vector2();
			//sum = v3 ? new Vector3() : new Vector2();
			sum = Vector3.zero;
		
			for (i = 0, il = points.Count; i < il; i ++) {
			
				p = points [i];
			
				if (p.x > maxX)
					maxX = p.x;
				else if (p.x < minX)
					minX = p.x;
			
				if (p.y > maxY)
					maxY = p.y;
				else if (p.y < minY)
					minY = p.y;

				if (p.z > maxZ)
					maxZ = p.z;
				else if (p.z < minZ)
					minZ = p.z;
			
			
				//sum.add( p );
				sum += p;
			
			}
		
//		var ret = {
//			
//		minX: minX,
//		minY: minY,
//		maxX: maxX,
//		maxY: maxY
//			
//		};
//		
//		if ( v3 ) {
//			
//			ret.maxZ = maxZ;
//			ret.minZ = minZ;
//			
//		}

			BoundingBox ret = new BoundingBox ();
			ret.minX = minX;
			ret.minY = minY;
			ret.minZ = minZ;
			ret.maxX = maxX;
			ret.maxY = maxY;
			ret.maxZ = maxZ;

			return ret;	
		}

/**************************************************************
*	Create Geometries Helpers
**************************************************************/
	
		/// Generate geometry from path points (for Line or ParticleSystem objects)
//
//		public Geometry createPointsGeometry (float divisions)
//		{
//			//List<Vector3> pts = this.getPoints( divisions, true );
//			List<Vector3> pts = this.getPoints (divisions, true);
//			return this.createGeometry (pts);
//		}
//	
//		// Generate geometry from equidistance sampling along the path
//	
//		public Geometry createSpacedPointsGeometry (float divisions)
//		{
//		
//			//List<Vector3> pts = this.getSpacedPoints( divisions, true );
//			List<Vector3> pts = this.getSpacedPoints (divisions);
//			return this.createGeometry (pts);
//		
//		}
//	
//		Geometry createGeometry (List<Vector3> points)
//		{
//		
//			Geometry geometry = new Geometry ();
//		
//			for (int i = 0; i < points.Count; i ++) {
//				geometry.vertices.Add (new Vector3 (points [i].x, points [i].y, points [i].z));
//			}
//		
//			return geometry;
//		
//		}


		/**************************************************************
		 *	Bend / Wrap Helper Methods
		 **************************************************************/
	
		// Wrap path / Bend modifiers?
		// TODO: ///////// Check Bend TYPE
		void addWrapPath (Bend bendpath)
		{
			this.bends.Add (bendpath);	
		}
	
		public List<Vector3> getTransformedPoints (float segments, List<Bend> bends)
		{
			List<Vector3> oldPts = this.getPoints (segments); // getPoints getSpacedPoints
			//int i, il;

			if (bends != null) {
				bends = this.bends;
			}

			if (bends != null) {
				for (int i = 0, il = bends.Count; i < il; i ++) {
					oldPts = this.getWrapPoints (oldPts, bends [i]);
				}
			} else {
				//Debug.LogWarning ("bend is null");
			}
			return oldPts;
		
		}
	
		public List<Vector3> getTransformedSpacedPoints (float segments, List<Bend> bends)
		{
		
			List<Vector3> oldPts = this.getSpacedPoints (segments);
		
			int i, il;
		
			if (bends != null) {
			
				bends = this.bends;
			
			}
		
			for (i = 0, il = bends.Count; i < il; i ++) {
			
				oldPts = this.getWrapPoints (oldPts, bends [i]);
			
			}
		
			return oldPts;
		
		}
	
		// This returns getPoints() bend/wrapped around the contour of a path.
		// Read http://www.planetclegg.com/projects/WarpingTextToSplines.html
	
		List<Vector3> getWrapPoints (List<Vector3> oldPts, Curve path)
		{
		
			BoundingBox bounds = this.getBoundingBox ();
		
			int i, il;
			Vector3 p;
			float oldX, oldY, xNorm;
		
			for (i = 0, il = oldPts.Count; i < il; i ++) {
			
				p = oldPts [i];
			
				oldX = p.x;
				oldY = p.y;
			
				xNorm = oldX / bounds.maxX;
			
				// If using actual distance, for length > path, requires line extrusions
				//xNorm = path.getUtoTmapping(xNorm, oldX); // 3 styles. 1) wrap stretched. 2) wrap stretch by arc length 3) warp by actual distance
			
				xNorm = path.getUtoTmapping (xNorm, oldX);
			
				// check for out of bounds?
			
				Vector3 pathPt = path.getPoint (xNorm);
				Vector3 normal = path.getTangent (xNorm);
				//normal.set( -normal.y, normal.x ).multiplyScalar( oldY );
				normal.x = -normal.y;
				normal.y = normal.x;
				normal *= oldY;
			
				p.x = pathPt.x + normal.x;
				p.y = pathPt.y + normal.y;
			
			}
		
			return oldPts;
		
		}

	}
}
