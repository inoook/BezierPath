using UnityEngine;
using System.Collections;

namespace ShapePath2{
	public interface IPath{

		float x0{ get; }
		float y0{ get; }
		float z0{ get; }
		float x1{ get; }
		float y1{ get; }
		float z1{ get; }
		float length{ get; }
		Point3D f( float t );
		Point3D diff( float t );
		VPoint3D vector( float t );
		IPath[] split(float t);
		float lengthToValue( float length );
		Point3D lengthToPoint( float length );
		bool isLine{ get; }
	}

	//
	public class QQBezier
	{
		static Vector3 getPosOnBezier( Vector3[] points, float t ) {
		  float tp = 1.0f - t;
		  Vector3 p0 = points[0];
		  Vector3 c  = points[1];
		  Vector3 p1 = points[2];
		  return new Vector3( p0.x*tp*tp + 2*c.x*t*tp + p1.x*t*t ,
		  					  p0.y*tp*tp + 2*c.y*t*tp + p1.y*t*t ,
		  					  p0.z*tp*tp + 2*c.z*t*tp + p1.z*t*t );
		}
	}

	//
	public class Point3D 
	{
		public float x;
		public float y;
		public float z;
		
		public Point3D( float x, float y, float z ) 
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
	}
	//public class PathVector : PathPoint
	public class VPoint3D : Point3D
	{
		public float vx;
		public float vy;
		public float vz;
		
		public VPoint3D( float x, float y, float z, float vx, float vy, float vz) : base( x, y, z)
		{
			this.vx = vx;
			this.vy = vy;
			this.vz = vz;
		}
	}

}