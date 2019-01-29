using UnityEngine;
using System.Collections;

public class PathLine : IPath {
	//start point
	private float _x0;
	private float _y0;
	private float _z0;
	
	//end point
	private float _x1;
	private float _y1;
	private float _z1;

	// 
	private Quaternion _rot0;
	private Quaternion _rot1;
	
	
	public PathLine ( float x0, float y0, float z0, float x1, float y1, float z1, Quaternion rot0, Quaternion rot1 ){
		_x0 = x0;
		_y0 = y0;
		_z0 = z0;
		_x1 = x1;
		_y1 = y1;
		_z1 = z1;

		_rot0 = rot0;
		_rot1 = rot1;
	}
	
	public PathLine clone (){
		return new PathLine( _x0, _y0, _z0, _x1, _y1, _z1, _rot0, _rot1 );
	}
	
	public PathPoint f (  float t   ){
		return new PathPoint( _x0 + ( _x1 - _x0 ) * t, _y0 + ( _y1 - _y0 ) * t, _z0 + ( _z1 - _z0 ) * t );
	}
	public Vector3 fVec3(float t){
		return new Vector3( _x0 + ( _x1 - _x0 ) * t, _y0 + ( _y1 - _y0 ) * t, _z0 + ( _z1 - _z0 ) * t );
	}
	
	public PathPoint diff (  float t   ){
		return new PathPoint( ( _x1 - _x0 ), ( _y1 - _y0 ), ( _z1 - _z0 ) );
	}
	
	public PathVector vector (  float t   ){
		return new PathVector( _x0 + ( _x1 - _x0 ) * t, _y0 + ( _y1 - _y0 ) * t, _z0 + ( _z1 - _z0 ) * t, ( _x1 - _x0 ), ( _y1 - _y0 ), ( _z1 - _z0 ), _rot0 );
	}
	
	public PathPoint lengthToPoint (  float len   ){
		return f( lengthToValue( len ) );
	}
	
	public float lengthToValue (  float len   ){
		return len/length;
	}
	
	public float length{
		get{
			return Mathf.Sqrt( ( _x1 - _x0 ) * ( _x1 - _x0 ) + ( _y1 - _y0 ) * ( _y1 - _y0 ) + ( _z1 - _z0 ) * ( _z1 - _z0 ) );
		}
	}
	
	public float x0{
		get{ return _x0; }
		set{
			_x0 = value;
		}
	}
	public float y0{
		get{ return _y0; }
		set{
			_y0 = value;
		}
	}
	public float z0{
		get{ return _z0; }
		set{
			_z0 = value;
		}
	}
	
	public float x1{
		get{ return _x1; }
		set{
			_x1 = value;
		}
	}
	public float y1{
		get{ return _y1; }
		set{
			_y1 = value;
		}
	}
	public float z1{
		get{ return _z1; }
		set{
			_z1 = value;
		}
	}
}
