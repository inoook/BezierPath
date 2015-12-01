using UnityEngine;
using System.Collections;

public class BezierInfo {

	private QBezier qBezier;
	
	public BezierInfo(Vector3[] points, Quaternion rot0, Quaternion rot1)
	{
		Vector3 p0 = points[0];
		Vector3 c  = points[1];
		Vector3 p1 = points[2];
		qBezier = new QBezier(	p0.x, p0.y, p0.z,
								c.x, c.y, c.z,
								p1.x, p1.y, p1.z, 
		                      rot0, rot1
		                      );
	}
	
	public void setPoints( Vector3[] points )
	{
		Vector3 p0 = points[0];
		Vector3 c  = points[1];
		Vector3 p1 = points[2];
		
		qBezier.point0 = p0;
		qBezier.control = c;
		qBezier.point1 = p1;
	}
	
	public Vector3 getPos(float t)
	{
		return qBezier.fVec3(t);
	}
	public float getBezierLength()
	{
		return qBezier.length;
	}
	public float lengthToValue(float len_ )
	{
		return qBezier.lengthToValue( len_ );
	}
	
	public PathVector vector( float t )
	{
		return qBezier.vector( t );
	}

}
