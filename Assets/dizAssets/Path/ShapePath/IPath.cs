using UnityEngine;
using System.Collections;

public interface IPath{

	float length{ get; }
	PathPoint f( float t );
	Vector3 fVec3( float t );
	PathPoint diff( float t );
	PathVector vector( float t );
	float lengthToValue( float length );
	PathPoint lengthToPoint( float length );
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
public class PathPoint 
{
	public float x;
	public float y;
	public float z;
	
	public PathPoint( float x, float y, float z ) 
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}
}
//public class PathVector : PathPoint
public class PathVector : PathPoint
{
	public float vx;
	public float vy;
	public float vz;

	public Quaternion rot;
	
	public PathVector( float x, float y, float z, float vx, float vy, float vz, Quaternion rot) : base( x, y, z)
	{
		this.vx = vx;
		this.vy = vy;
		this.vz = vz;

		this.rot = rot;
	}
}