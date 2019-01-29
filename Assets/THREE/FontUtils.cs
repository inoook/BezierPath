// https://github.com/mrdoob/three.js/blob/master/src/extras/FontUtils.js
/**
 * @author zz85 / http://www.lab4games.net/zz85/blog
 * @author alteredq / http://alteredqualia.com/
 *
 * For Text operations in three.js (See TextGeometry)
 *
 * It uses techniques used in:
 *
 * 	typeface.js and canvastext
 * 		For converting fonts and rendering with javascript
 *		http://typeface.neocracy.org
 *
 *	Triangulation ported from AS3
 *		Simple Polygon Triangulation
 *		http://actionsnippet.com/?p=1462
 *
 * 	A Method to triangulate shapes with holes
 *		http://www.sakri.net/blog/2009/06/12/an-approach-to-triangulating-polygons-with-holes/
 *
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace THREE
{
	public class FontUtils
	{
		static float EPSILON = Setting.EPSILON;
		
		public static List<List<Vector3>> Triangulate (List<Vector3> contour, bool indices)
		{
			int n = contour.Count;

			if (n < 3)
				return null;

			List<List<Vector3>> result = new List<List<Vector3>> ();
			List<int> verts = new List<int> ();
			List<List<int>> vertIndices = new List<List<int>>();

			/* we want a counter-clockwise polygon in verts */
			
			int u, v, w;
			
			if (area (contour) > 0.0f) {
				
				for (v = 0; v < n; v++) {
					if (v < verts.Count) {
						Debug.LogWarning ("!!!!!");
						verts [v] = v;
					} else {
						verts.Add (v);
					}
				}
				
			} else {
				
				for (v = 0; v < n; v++) {
					if (v < verts.Count) {
						Debug.LogWarning ("!!!!!");
						verts [v] = (n - 1) - v;
					} else {
						verts.Add ((n - 1) - v);
					}
				}
				
			}

			int nv = n;

			int snipCount = 0;// debug
			/*  remove nv - 2 vertices, creating 1 triangle every time */
			
			int count = 2 * nv;   /* error detection */

			for (v = nv - 1; nv > 2;) {
				
				/* if we loop, it is probably a non-simple polygon */
				
				if ((count--) <= 0) {
					
					//** Triangulate: ERROR - probable bad polygon!
					
					//throw ( "Warning, unable to triangulate polygon!" );
					//return null;
					// Sometimes warning is fine, especially polygons are triangulated in reverse.
					Debug.LogError ("Warning, unable to triangulate polygon! / snipCount: " + snipCount);
					
					//if ( indices ) return vertIndices;
					return result;
					
				}
				/* three consecutive vertices in current polygon, <u,v,w> */
				
				u = v;
				if (nv <= u){
					u = 0;     /* previous */
				}
				v = u + 1;
				if (nv <= v){
					v = 0;     /* new v    */
				}
				w = v + 1;
				if (nv <= w){
					w = 0;     /* next     */
				}
				
				if (snip (contour, u, v, w, nv, verts)) {
					snipCount ++;
					int a, b, c, s, t;
					
					/* true names of the vertices */
					
					a = verts [u];
					b = verts [v];
					c = verts [w];
					
					/* output Triangle */
					
					result.Add (new List<Vector3> (new Vector3[]{ 
						contour [a],
					    contour [b],
						contour [c] }));

					vertIndices.Add( new List<int>( new int[]{ verts[ u ], verts[ v ], verts[ w ] } ));
					
					/* remove v from the remaining polygon */
					
					for (s = v, t = v + 1; t < nv; s++, t++) {
						
						verts [s] = verts [t];
						
					}
					
					nv--;
					
					/* reset error detection counter */
					
					count = 2 * nv;
				}
				
			}
			
			return result;
		}

		public static float area (List<Vector3> contour)
		{
			int n = contour.Count;
			float a = 0.0f;
			
			for (int p = n - 1, q = 0; q < n; p = q++) {
				a += contour [p].x * contour [q].y - contour [q].x * contour [p].y;
			}
			
			return a * 0.5f;
		}

		public static bool snip (List<Vector3> contour, int u, int v, int w, int n, List<int> verts)
		{
			//float EPSILON = 0.0000000001f;

			int p;
			float ax, ay, bx, by;
			float cx, cy, px, py;
			
			ax = contour [verts [u]].x;
			ay = contour [verts [u]].y;
			
			bx = contour [verts [v]].x;
			by = contour [verts [v]].y;
			
			cx = contour [verts [w]].x;
			cy = contour [verts [w]].y;
			
			if (EPSILON > (((bx - ax) * (cy - ay)) - ((by - ay) * (cx - ax))))
				return false;
			
			float aX, aY, bX, bY, cX, cY;
			float apx, apy, bpx, bpy, cpx, cpy;
			float cCROSSap, bCROSScp, aCROSSbp;
			
			aX = cx - bx;
			aY = cy - by;
			bX = ax - cx;
			bY = ay - cy;
			cX = bx - ax;
			cY = by - ay;
			
			for (p = 0; p < n; p++) {
				
				px = contour [verts [p]].x;
				py = contour [verts [p]].y;
						
				if (((px == ax) && (py == ay)) ||
					((px == bx) && (py == by)) ||
					((px == cx) && (py == cy))){
					continue;
				}
				
				apx = px - ax;
				apy = py - ay;
				bpx = px - bx;
				bpy = py - by;
				cpx = px - cx;
				cpy = py - cy;
				
				// see if p is inside triangle abc
				
				aCROSSbp = aX * bpy - aY * bpx;
				cCROSSap = cX * apy - cY * apx;
				bCROSScp = bX * cpy - bY * cpx;
				
				if ((aCROSSbp >= -EPSILON) && (bCROSScp >= -EPSILON) && (cCROSSap >= -EPSILON)){
					return false;
				}
			}
			
			return true;
		}
		//}
	}
}
