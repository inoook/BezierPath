// https://github.com/mrdoob/three.js/blob/master/src/extras/core/Shape.js
/**
 * @author zz85 / http://www.lab4games.net/zz85/blog
 * Defines a 2d shape plane using paths.
 **/

// STEP 1 Create a path.
// STEP 2 Turn path into shape.
// STEP 3 ExtrudeGeometry takes in Shape/Shapes
// STEP 3a - Extract points from each shape, turn to vertices
// STEP 3b - Triangulate each shape, add faces.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{

//	public class ShapeAndHoleObject
//	{
//		public Shape baseShape;
//
//		public List<Vector3> shapeVertices;
//		public List<List<Vector3>> holes;
//		public bool reverse;
//	}

	public class Shape
	{
//		//public List<Hole> holes;
//		public List<CurvePath> holes;
//
//		public List<Vector2> normals;
//
//		public Shape (List<Vector2> points, List<Vector2> normals = null) : base( points )
//	//public Shape (List<Vector3> points)
//		{
//			this.holes = new List<CurvePath> ();
//
//			if(normals != null){
//				this.normals = normals;
//			}
//		}
//
//		public Shape ()
//		{
//			holes = new List<CurvePath> ();
//		}
//
//		// Convenience method to return ExtrudeGeometry
//	
//		THREE.ExtrudeGeometry extrude (THREE.ExtrudeGeometry.Option options)
//		{
//			THREE.ExtrudeGeometry extruded = new THREE.ExtrudeGeometry (new List<Shape> (new Shape[]{ this }), options);
//			return extruded;
//		}
//
//		// Convenience method to return ShapeGeometry
//	
//		THREE.ShapeGeometry makeGeometry (THREE.ShapeGeometry.Option options)
//		{
//		
//			var geometry = new THREE.ShapeGeometry (new List<Shape> (new Shape[]{ this }), options);
//			return geometry;
//		}
//
//		// Get points of holes
//
//		List<List<Vector3>> getPointsHoles (float divisions)
//		{
//			int i, il = this.holes.Count;
//			List<List<Vector3>> holesPts = new List<List<Vector3>> ();
//		
//			for (i = 0; i < il; i ++) {
//				List<Vector3> pts = this.holes [i].getTransformedPoints (divisions, this.bends);
//				holesPts.Add (pts);
//			}
//			return holesPts;
//		}
//
//		// Get points of holes (spaced by regular distance)
//	
//		List<List<Vector3>> getSpacedPointsHoles (float divisions)
//		{
//		
//			int i, il = this.holes.Count;
//			List<List<Vector3>> holesPts = new List<List<Vector3>> ();
//		
//			for (i = 0; i < il; i ++) {
//				List<Vector3> pts = this.holes [i].getTransformedSpacedPoints (divisions, this.bends);
//				holesPts.Add (pts);
//			}
//
//			return holesPts;
//		}
//
//		// Get points of shape and holes (keypoints based on segments parameter)
//	
//		ShapeAndHoleObject extractAllPoints (float divisions)
//		{
//			ShapeAndHoleObject obj = new ShapeAndHoleObject ();
//			obj.shapeVertices = this.getTransformedPoints (divisions, null);
//			obj.holes = this.getPointsHoles (divisions);
//
//			return obj;
////		return {
////			shape: this.getTransformedPoints( divisions ),
////			holes: this.getPointsHoles( divisions )		
////		}
//		}
//	
//		public ShapeAndHoleObject extractPoints (float divisions)
//		{
//			if (this.useSpacedPoints) {
//				return this.extractAllSpacedPoints (divisions);
//			}
//			return this.extractAllPoints (divisions);
//		}
//
//		// Get points of shape and holes (spaced by regular distance)
//	
//		ShapeAndHoleObject extractAllSpacedPoints (float divisions)
//		{
//			ShapeAndHoleObject obj = new ShapeAndHoleObject ();
//			obj.shapeVertices = this.getTransformedSpacedPoints (divisions, null);
//			obj.holes = this.getSpacedPointsHoles (divisions);
//
//			return obj;
////		return {
////			
////			shape: this.getTransformedSpacedPoints( divisions ),
////			holes: this.getSpacedPointsHoles( divisions )
////				
////		}
//		}

		/**************************************************************
		 *	Utils
		 **************************************************************/
		public class Utils
		{
	
			public static List<List<int>> triangulateShape (List<Vector3> contour, List<List<Vector3>> holes)
			{
				//int i, il, f; 
				//List<int> face;
				string key; 
				int index;

				Dictionary<string, int> allPointsMap = new Dictionary<string, int> ();
			
				// To maintain reference to old shape, one must match coordinates, or offset the indices from original arrays. It's probably easier to do the first.
			
				//List<List<Vector3>> allpoints = contour.concat();
				List<Vector3> allpoints = new List<Vector3> ();
				allpoints.AddRange (contour);

				for (int h = 0, hl = holes.Count; h < hl; h ++) {
					//Array.prototype.push.apply( allpoints, holes[h] );
					for (int n = 0; n < holes[h].Count; n++) {
						allpoints.Add (holes [h] [n]);
					}
				}

				// prepare all points map

				for (int i = 0, il = allpoints.Count; i < il; i ++) {
				
					key = allpoints [i].x + ":" + allpoints [i].y;
				
					//if ( allPointsMap[ key ] != undefined ) {
					if (allPointsMap.ContainsKey (key)) {
						Debug.Log ("Duplicate point" + key);
					}
				
					allPointsMap [key] = i;
				}
			
				// remove holes by cutting paths to holes and adding them to the shape
				List<Vector3> shapeWithoutHoles = removeHoles (contour, holes);

				List<List<Vector3>> triangles = THREE.FontUtils.Triangulate (shapeWithoutHoles, false); // True returns indices for points of spooled shape
				//Debug.Log ("triangles: " + triangles.Count);

				// check all face vertices against all points map
			
				List<List<int>> resultIndexes = new List<List<int>> ();

				for (int i = 0, il = triangles.Count; i < il; i ++) {
				
					List<Vector3> face = triangles [i];

					List<int> indexs = new List<int> (new int[3]);

					for (int f = 0; f < 3; f ++) {
						key = face [f].x + ":" + face [f].y;
					
						index = allPointsMap [key];
					
						//if ( index != undefined ) {
						if (allPointsMap.ContainsKey (key)) {
							indexs [f] = index;
						}
					}

					resultIndexes.Add (indexs);
				}

				//return triangles;
				return resultIndexes;
			}

			static bool point_in_segment_2D_colin (Vector2 inSegPt1, Vector2 inSegPt2, Vector2 inOtherPt)
			{
				// inOtherPt needs to be colinear to the inSegment
				if (inSegPt1.x != inSegPt2.x) {
					if (inSegPt1.x < inSegPt2.x) {
						return	((inSegPt1.x <= inOtherPt.x) && (inOtherPt.x <= inSegPt2.x));
					} else {
						return	((inSegPt2.x <= inOtherPt.x) && (inOtherPt.x <= inSegPt1.x));
					}
				} else {
					if (inSegPt1.y < inSegPt2.y) {
						return	((inSegPt1.y <= inOtherPt.y) && (inOtherPt.y <= inSegPt2.y));
					} else {
						return	((inSegPt2.y <= inOtherPt.y) && (inOtherPt.y <= inSegPt1.y));
					}
				}
			}

			static List<Vector2> intersect_segments_2D (Vector2 inSeg1Pt1, Vector2 inSeg1Pt2, Vector2 inSeg2Pt1, Vector2 inSeg2Pt2, bool inExcludeAdjacentSegs)
			{
				List<Vector2> result = new List<Vector2> ();

				float EPSILON = THREE.Setting.EPSILON;
			
				float seg1dx = inSeg1Pt2.x - inSeg1Pt1.x, seg1dy = inSeg1Pt2.y - inSeg1Pt1.y;
				float seg2dx = inSeg2Pt2.x - inSeg2Pt1.x, seg2dy = inSeg2Pt2.y - inSeg2Pt1.y;
			
				float seg1seg2dx = inSeg1Pt1.x - inSeg2Pt1.x;
				float seg1seg2dy = inSeg1Pt1.y - inSeg2Pt1.y;
			
				float limit = seg1dy * seg2dx - seg1dx * seg2dy;
				float perpSeg1 = seg1dy * seg1seg2dx - seg1dx * seg1seg2dy;
			
				if (Mathf.Abs (limit) > EPSILON) {			// not parallel
				
					float perpSeg2;
					if (limit > 0) {
						if ((perpSeg1 < 0) || (perpSeg1 > limit)) {
							return result;
						}
						perpSeg2 = seg2dy * seg1seg2dx - seg2dx * seg1seg2dy;
						if ((perpSeg2 < 0) || (perpSeg2 > limit)) {
							return result;
						}
					} else {
						if ((perpSeg1 > 0) || (perpSeg1 < limit)) {
							return result;
						}
						perpSeg2 = seg2dy * seg1seg2dx - seg2dx * seg1seg2dy;
						if ((perpSeg2 > 0) || (perpSeg2 < limit)) {
							return result;
						}
					}
				
					// i.e. to reduce rounding errors
					// intersection at endpoint of segment#1?
					if (perpSeg2 == 0) {
						if ((inExcludeAdjacentSegs) &&
							((perpSeg1 == 0) || (perpSeg1 == limit))) {
							return result;
						}

						//return  [ inSeg1Pt1 ];
						result.Add (inSeg1Pt1);
						return result;
					}
					if (perpSeg2 == limit) {
						if ((inExcludeAdjacentSegs) &&
							((perpSeg1 == 0) || (perpSeg1 == limit))) {
							return result;
						}
						//return  [ inSeg1Pt2 ];
						result.Add (inSeg1Pt2);
						return result;
					}
					// intersection at endpoint of segment#2?
					if (perpSeg1 == 0) { 
						//return  [ inSeg2Pt1 ];
						result.Add (inSeg2Pt1);
						return result;
					}
					if (perpSeg1 == limit) { 
						//return  [ inSeg2Pt2 ];
						result.Add (inSeg2Pt2);
						return  result;
					}
				
					// return real intersection point
					float factorSeg1 = perpSeg2 / limit;
					/*return	[ { x: inSeg1Pt1.x + factorSeg1 * seg1dx,
					y: inSeg1Pt1.y + factorSeg1 * seg1dy } ];
					*/
					Vector2 p = new Vector2 (inSeg1Pt1.x + factorSeg1 * seg1dx,
				                        y: inSeg1Pt1.y + factorSeg1 * seg1dy);
					result.Add (p);
					return  result;
				
				} else {		// parallel or colinear
					if ((perpSeg1 != 0) ||
						(seg2dy * seg1seg2dx != seg2dx * seg1seg2dy))
						return result;
				
					// they are collinear or degenerate
					bool seg1Pt = ((seg1dx == 0) && (seg1dy == 0));	// segment1 ist just a point?
					bool seg2Pt = ((seg2dx == 0) && (seg2dy == 0));	// segment2 ist just a point?
					// both segments are points
					if (seg1Pt && seg2Pt) {
						if ((inSeg1Pt1.x != inSeg2Pt1.x) ||
							(inSeg1Pt1.y != inSeg2Pt1.y))
							return result;   	// they are distinct  points
						//return  [ inSeg1Pt1 ];                 					// they are the same point
						result.Add (inSeg1Pt1);
						return  result;               					// they are the same point
					}
					// segment#1  is a single point
					if (seg1Pt) {
						if (! point_in_segment_2D_colin (inSeg2Pt1, inSeg2Pt2, inSeg1Pt1))
							return result;		// but not in segment#2
						//return  [ inSeg1Pt1 ];
						result.Add (inSeg1Pt1);
						return  result;             
					}
					// segment#2  is a single point
					if (seg2Pt) {
						if (! point_in_segment_2D_colin (inSeg1Pt1, inSeg1Pt2, inSeg2Pt1))
							return result;		// but not in segment#1
						//return  [ inSeg2Pt1 ];
						result.Add (inSeg2Pt1);
						return  result;         
					}
				
					// they are collinear segments, which might overlap
					Vector2 seg1min, seg1max;
					float seg1minVal, seg1maxVal;
					Vector2 seg2min, seg2max;
					float seg2minVal, seg2maxVal;
					if (seg1dx != 0) {		// the segments are NOT on a vertical line
						if (inSeg1Pt1.x < inSeg1Pt2.x) {
							seg1min = inSeg1Pt1;
							seg1minVal = inSeg1Pt1.x;
							seg1max = inSeg1Pt2;
							seg1maxVal = inSeg1Pt2.x;
						} else {
							seg1min = inSeg1Pt2;
							seg1minVal = inSeg1Pt2.x;
							seg1max = inSeg1Pt1;
							seg1maxVal = inSeg1Pt1.x;
						}
						if (inSeg2Pt1.x < inSeg2Pt2.x) {
							seg2min = inSeg2Pt1;
							seg2minVal = inSeg2Pt1.x;
							seg2max = inSeg2Pt2;
							seg2maxVal = inSeg2Pt2.x;
						} else {
							seg2min = inSeg2Pt2;
							seg2minVal = inSeg2Pt2.x;
							seg2max = inSeg2Pt1;
							seg2maxVal = inSeg2Pt1.x;
						}
					} else {				// the segments are on a vertical line
						if (inSeg1Pt1.y < inSeg1Pt2.y) {
							seg1min = inSeg1Pt1;
							seg1minVal = inSeg1Pt1.y;
							seg1max = inSeg1Pt2;
							seg1maxVal = inSeg1Pt2.y;
						} else {
							seg1min = inSeg1Pt2;
							seg1minVal = inSeg1Pt2.y;
							seg1max = inSeg1Pt1;
							seg1maxVal = inSeg1Pt1.y;
						}
						if (inSeg2Pt1.y < inSeg2Pt2.y) {
							seg2min = inSeg2Pt1;
							seg2minVal = inSeg2Pt1.y;
							seg2max = inSeg2Pt2;
							seg2maxVal = inSeg2Pt2.y;
						} else {
							seg2min = inSeg2Pt2;
							seg2minVal = inSeg2Pt2.y;
							seg2max = inSeg2Pt1;
							seg2maxVal = inSeg2Pt1.y;
						}
					}
					if (seg1minVal <= seg2minVal) {
						if (seg1maxVal < seg2minVal)
							return result;
						if (seg1maxVal == seg2minVal) {
							if (inExcludeAdjacentSegs)
								return result;
							//return [ seg2min ];
							result.Add (seg2min);
							return  result;   
						}
						if (seg1maxVal <= seg2maxVal) { 
							//return [ seg2min, seg1max ];
							result.Add (seg2min);
							result.Add (seg1max);
							return result;
						}
						//return	[ seg2min, seg2max ];
						result.Add (seg2min);
						result.Add (seg2max);
						return result;
					} else {
						if (seg1minVal > seg2maxVal)
							return result;
						if (seg1minVal == seg2maxVal) {
							if (inExcludeAdjacentSegs)
								return result;
							//return [ seg1min ];
							result.Add (seg1min);
							return result;
						}
						if (seg1maxVal <= seg2maxVal) {
							//return [ seg1min, seg1max ];
							result.Add (seg1min);
							result.Add (seg1max);
							return result;
						}
						//return	[ seg1min, seg2max ];
						result.Add (seg1min);
						result.Add (seg2max);
						return result;
					}
				}
			}

			static bool isPointInsideAngle (Vector3 inVertex, Vector3 inLegFromPt, Vector3 inLegToPt, Vector3 inOtherPt)
			{
				// The order of legs is important
			
				float EPSILON = THREE.Setting.EPSILON;
			
				// translation of all points, so that Vertex is at (0,0)
				float legFromPtX = inLegFromPt.x - inVertex.x, legFromPtY = inLegFromPt.y - inVertex.y;
				float legToPtX = inLegToPt.x - inVertex.x, legToPtY = inLegToPt.y - inVertex.y;
				float otherPtX = inOtherPt.x - inVertex.x, otherPtY = inOtherPt.y - inVertex.y;
			
				// main angle >0: < 180 deg.; 0: 180 deg.; <0: > 180 deg.
				float from2toAngle = legFromPtX * legToPtY - legFromPtY * legToPtX;
				float from2otherAngle = legFromPtX * otherPtY - legFromPtY * otherPtX;
			
				if (Mathf.Abs (from2toAngle) > EPSILON) {			// angle != 180 deg.
				
					float other2toAngle = otherPtX * legToPtY - otherPtY * legToPtX;
					// console.log( "from2to: " + from2toAngle + ", from2other: " + from2otherAngle + ", other2to: " + other2toAngle );
				
					if (from2toAngle > 0) {				// main angle < 180 deg.
						return	((from2otherAngle >= 0) && (other2toAngle >= 0));
					} else {								// main angle > 180 deg.
						return	((from2otherAngle >= 0) || (other2toAngle >= 0));
					}
				} else {										// angle == 180 deg.
					// console.log( "from2to: 180 deg., from2other: " + from2otherAngle  );
					return	(from2otherAngle > 0);
				}
			}

			static List<Vector3> removeHoles (List<Vector3> contour, List<List<Vector3>> holes)
			{
				//List<Vector3> shape = contour; 
				List<Vector3> shape = new List<Vector3> ();// work on this shape
				shape.AddRange (contour);

				List<Vector3> hole;

				// js 351
				List<int> indepHoles = new List<int> ();

				// js 368
				int holeIndex;
				//int shapeIndex;
				Vector3 shapePt, holePt;
				int holeIdx;
				string cutKey;
				Dictionary<string, bool> failedCuts = new Dictionary<string, bool> ();

				for (int h = 0, hl = holes.Count; h < hl; h ++) {
					indepHoles.Add (h);
				}

				// js 380
				int minShapeIndex = 0;
				int counter = indepHoles.Count * 2;
			 	
				while (indepHoles.Count > 0) {
					counter --;
					if (counter < 0) {
						Debug.LogError ("Infinite Loop! Holes left:" + indepHoles.Count + ", Probably Hole outside Shape!");
						break;
					}
					
					// search for shape-vertex and hole-vertex,
					// which can be connected without intersections
					for (int shapeIndex = minShapeIndex; shapeIndex < shape.Count; shapeIndex++) {
					
						shapePt = shape [shapeIndex];
						holeIndex = -1;

						//Debug.LogWarning("indepHoles.Count: "+indepHoles.Count);
						// search for hole which can be reached without intersections
						for (int h = 0; h < indepHoles.Count; h ++) {
							holeIdx = indepHoles [h];
						
							// prevent multiple checks
							cutKey = (shapePt.x) + ":" + (shapePt.y) + ":" + holeIdx;
							//if ( failedCuts[cutKey] !== undefined )			continue;
							if (failedCuts.ContainsKey (cutKey))
								continue;


							hole = holes [holeIdx];
							//Debug.Log("hole.Count: "+holeIdx + " / "+ hole.Count);
							for (int h2 = 0; h2 < hole.Count; h2 ++) {
								holePt = hole [h2];
								if (! isCutLineInsideAngles (shapeIndex, h2, shape, hole))
									continue;
								if (intersectsShapeEdge (shapePt, holePt, shape))
									continue;
								if (intersectsHoleEdge (shapePt, holePt, shape, holes, indepHoles))
									continue;
							
								holeIndex = h2;
								//indepHoles.splice(h,1);
								indepHoles.RemoveAt (h);
								
								List<Vector3> tmpShape1 = shape.GetRange (0, shapeIndex + 1);
								List<Vector3> tmpShape2 = shape.GetRange (shapeIndex, shape.Count - shapeIndex);
								List<Vector3> tmpHole1 = hole.GetRange (holeIndex, hole.Count - holeIndex);
								List<Vector3> tmpHole2 = hole.GetRange (0, holeIndex + 1);
								
								shape = new List<Vector3> ();
								shape.AddRange (tmpShape1);
								shape.AddRange (tmpHole1);
								shape.AddRange (tmpHole2);
								shape.AddRange (tmpShape2);
								
								minShapeIndex = shapeIndex;
							
								// Debug only, to show the selected cuts
								// glob_CutLines.push( [ shapePt, holePt ] );
							
								break;
							}
							if (holeIndex >= 0)
								break;		// hole-vertex found
						
							failedCuts [cutKey] = true;			// remember failure
						}
						if (holeIndex >= 0)
							break;		// hole-vertex found
					}
				}
				
				return shape; 			/* shape with no holes */
			}

			static bool isCutLineInsideAngles (int inShapeIdx, int inHoleIdx, List<Vector3> shape, List<Vector3> hole)
			{
				// Check if hole point lies within angle around shape point
				int lastShapeIdx = shape.Count - 1;
			
				int prevShapeIdx = inShapeIdx - 1;
				if (prevShapeIdx < 0)
					prevShapeIdx = lastShapeIdx;
			
				int nextShapeIdx = inShapeIdx + 1;
				if (nextShapeIdx > lastShapeIdx)
					nextShapeIdx = 0;
			
				bool insideAngle = isPointInsideAngle (shape [inShapeIdx], shape [prevShapeIdx], shape [nextShapeIdx], hole [inHoleIdx]);
				if (! insideAngle) {
					//Debug.Log( "Vertex (Shape): " + inShapeIdx + ", Point: " + hole[inHoleIdx].x + "/" + hole[inHoleIdx].y );
					return	false;
				}
			
				// Check if shape point lies within angle around hole point
				int lastHoleIdx = hole.Count - 1;
			
				int prevHoleIdx = inHoleIdx - 1;
				if (prevHoleIdx < 0)
					prevHoleIdx = lastHoleIdx;
			
				int nextHoleIdx = inHoleIdx + 1;
				if (nextHoleIdx > lastHoleIdx)
					nextHoleIdx = 0;
			
				insideAngle = isPointInsideAngle (hole [inHoleIdx], hole [prevHoleIdx], hole [nextHoleIdx], shape [inShapeIdx]);
				if (! insideAngle) {
					//Debug.Log ("Vertex (Hole): " + inHoleIdx + ", Point: " + shape [inShapeIdx].x + "/" + shape [inShapeIdx].y);
					return	false;
				}
			
				return	true;
			}

			static bool intersectsShapeEdge (Vector3 inShapePt, Vector3 inHolePt, List<Vector3> shape)
			{
				// checks for intersections with shape edges
				int sIdx, nextIdx;
				List<Vector2> intersection;
				for (sIdx = 0; sIdx < shape.Count; sIdx++) {
					nextIdx = sIdx + 1;
					nextIdx %= shape.Count;
					intersection = intersect_segments_2D (inShapePt, inHolePt, shape [sIdx], shape [nextIdx], true);
					if (intersection.Count > 0) {
						return	true;
					}
				}
			
				return	false;
			}

			static bool intersectsHoleEdge (Vector3 inShapePt, Vector3 inHolePt, List<Vector3> shape, List<List<Vector3>> holes, List<int> indepHoles)
			{
				// checks for intersections with hole edges
				int ihIdx;
				List<Vector3> chkHole;
				int hIdx, nextIdx;
				List<Vector2> intersection;
				for (ihIdx = 0; ihIdx < indepHoles.Count; ihIdx++) {
					chkHole = holes [indepHoles [ihIdx]];
					for (hIdx = 0; hIdx < chkHole.Count; hIdx++) {
						nextIdx = hIdx + 1;
						nextIdx %= chkHole.Count;
						intersection = intersect_segments_2D (inShapePt, inHolePt, chkHole [hIdx], chkHole [nextIdx], true);
						if (intersection.Count > 0)
							return	true;
					}
				}
				return	false;
			}
			//


			public static bool isClockWise (List<Vector3> pts)
			{
				return THREE.FontUtils.area (pts) < 0.0f;
			}

			// Bezier Curves formulas obtained from
			// http://en.wikipedia.org/wiki/B%C3%A9zier_curve
		
			// Quad Bezier Functions
			
			public static float b2p0 (float t, float p)
			{
				float k = 1.0f - t;
				return k * k * p;
			}
			
			public static float b2p1 (float t, float p)
			{
				return 2.0f * (1.0f - t) * t * p;
			}
			
			public static float b2p2 (float t, float p)
			{
				return t * t * p;
			}

			public static float b2 (float t, float p0, float p1, float p2)
			{
				return b2p0 (t, p0) + b2p1 (t, p1) + b2p2 (t, p2);
			}

			// Cubic Bezier Functions
			
			public static float b3p0 (float t, float p)
			{
				float k = 1.0f - t;
				return k * k * k * p;
			}
			
			public static float b3p1 (float t, float p)
			{
				float k = 1.0f - t;
				return 3 * k * k * t * p;
			}
			
			public static float b3p2 (float t, float p)
			{	
				float k = 1.0f - t;
				return 3 * k * t * t * p;	
			}

			public static float b3p3 (float t, float p)
			{
				return t * t * t * p;	
			}
			
			public static float b3 (float t, float p0, float p1, float p2, float p3)
			{
				return b3p0 (t, p0) + b3p1 (t, p1) + b3p2 (t, p2) + b3p3 (t, p3);	
			}
		}
	}
}
