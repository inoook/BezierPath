using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShapePath {
	/*
	private static int __MOVE_TO = 1;
	private static int __LINE_TO = 2;
	private static int __CURVE_TO = 3;
	*/
	enum PathType
	{
		__MOVE_TO, __LINE_TO, __CURVE_TO
	}
	
	private ArrayList __command;
	private ArrayList __coordinates;
	private ArrayList __quats;
	private ArrayList __path;
	public ArrayList __path_length;

	
	private PathType[] b__command;
	private float[] b__coordinates;
	private Quaternion[] b__quats;
	private IPath[] b__path;
	public float[] b__path_length;
	
	
	public float __length;
	private int __cmd_index;
	
	public ShapePath (){
		__command = new ArrayList();
		__coordinates = new ArrayList();
		__quats = new ArrayList();
		__path = new ArrayList();
	}
	
	public void begin (){
		__command = new ArrayList();
		__coordinates = new ArrayList();
		__quats = new ArrayList();
		__path = new ArrayList();
		__path_length = new ArrayList();
		__cmd_index = 0;
		__length = -1;
		
	}
	
	public void end (){
		
		b__command = (PathType[])__command.ToArray( typeof(PathType) );
		b__coordinates = (float[])__coordinates.ToArray( typeof(float) );
		b__quats = (Quaternion[])__quats.ToArray( typeof(Quaternion) );
		
		int cmd_len = __cmd_index;
		PathType c;
		int ci = 0;
		int roti = 0;
		float x;
		float y;
		float z;
		float px = 0;
		float py = 0;
		float pz = 0;
		int path_i = 0;
		__length = 0;
		for ( int i = 0; i < cmd_len; i++ )
		{
			c = b__command[i];
			switch( c )
			{
				case PathType.__MOVE_TO:
					px = b__coordinates[ci];
					py = b__coordinates[ci + 1];
					pz = b__coordinates[ci + 2];
					ci += 3;
					break;
				
				case PathType.__LINE_TO:
					x = b__coordinates[ci];
					y = b__coordinates[ci + 1];
					z = b__coordinates[ci + 2];
					PathLine pline = new PathLine( px, py, pz, x, y, z, b__quats[roti], b__quats[roti+1] );
					//__path[path_i] = pline;
					__path.Add( pline );
					__length += pline.length;
					//__path_length[path_i] = __length;
					__path_length.Add( __length );
					path_i++;
					px = x;
					py = y;
					pz = z;
					ci += 3;
					roti += 1;
					break;
				
				case PathType.__CURVE_TO:
					x = b__coordinates[ci + 3];
					y = b__coordinates[ci + 4];
					z = b__coordinates[ci + 5];
					QBezier pbezier =  new QBezier( px, py, pz, b__coordinates[ci], b__coordinates[ci + 1], b__coordinates[ci + 2], x, y, z, b__quats[roti], b__quats[roti+1] );
					//__path[path_i] = pbezier;
					__path.Add( pbezier );
					__length += pbezier.length;
					//__path_length[path_i] = __length;
					__path_length.Add( __length );
					path_i++;
					px = x;
					py = y;
					pz = z;
					ci += 6;
					roti += 1;
					break;
			}
		}
		
		b__path = (IPath[])__path.ToArray( typeof(IPath) );
		b__path_length = (float[])__path_length.ToArray( typeof(float) );
	}
	//-------------------------------------------------------------------------------------------------------
	
	public PathVector getPointInfo (  float targetLength   ){
		/*
		if ( targetLength < 0 || targetLength > __length )
			return null;
		*/
		if ( targetLength <= 0.0f ){
			IPath p0 = b__path[0];
			return p0.vector( 0.0f );
		}
		
		if ( targetLength >= __length ){
			IPath p1 = b__path[b__path.Length-1];
			return p1.vector( 1.0f );
		}
		
		
		int n = b__path.Length;
		float pre_len = 0;
		float tmp_len;
		for ( int i = 0; i < n; i++ )
		{
			tmp_len = b__path_length[i];
			if ( targetLength <= tmp_len )
			{
				IPath p = b__path[i];
				float t = p.lengthToValue( targetLength - pre_len );
				return p.vector( t );
			}
			pre_len = tmp_len;
		}
		return null;
	}
	
	public Vector3 getPointInfoVec3 (  float targetLength   ){
		PathVector pVec = getPointInfo(targetLength);
		if(pVec == null){
			Debug.LogWarning("PathVector is null");
			return Vector3.zero;
		}else{
			return new Vector3(pVec.x, pVec.y, pVec.z);
		}
	}

	public PathVector getPathInfo( float percent ){
		return getPointInfo(this.length * percent);
	}
	
	public PathVector getPointInfoPercent ( float percent ){
		return getPointInfo(this.length * percent);
	}
	public Vector3 getPointInfoVec3Percent ( float percent ){
		return getPointInfoVec3(this.length * percent);
	}
	
	public Vector3 getPointInfoVec3__ (  float targetLength   ){
		/*
		if ( targetLength < 0 || targetLength > __length )
			return null;
		*/
		if ( targetLength <= 0.0f ){
			IPath p0 = b__path[0];
			return p0.fVec3( 0.0f );
		}
		
		if ( targetLength >= __length ){
			IPath p1 = b__path[b__path.Length-1];
			return p1.fVec3( 1.0f );
		}
		
		
		int n = b__path.Length;
		float pre_len = 0;
		float tmp_len;
		for ( int i = 0; i < n; i++ )
		{
			tmp_len = b__path_length[i];
			if ( targetLength <= tmp_len )
			{
				IPath p = b__path[i];
				float t = p.lengthToValue( targetLength - pre_len );
				return p.fVec3( t );
			}
			pre_len = tmp_len;
		}
		return Vector3.zero;
	}
	
	public float length{
		get {
			return __length;
		}
	}
	
	//-------------------------------------------------------------------------------------------------------
	
	public void moveTo ( float x, float y, float z, Quaternion rot){
		__command.Add(PathType.__MOVE_TO);
		__cmd_index++;
		__coordinates.AddRange( new float[]{x, y, z} );
		__quats.Add(rot);
	}
	
	public void lineTo ( float x, float y, float z, Quaternion rot ){
		__command.Add(PathType.__LINE_TO);
		__cmd_index++;
		__coordinates.AddRange( new float[]{x, y, z});
		__quats.Add(rot);
	}
	
	public void curveTo ( float cx, float cy, float cz, float x, float y, float z, Quaternion rot ){
		
		int preIndex = __coordinates.Count - 1;
		Vector3 prePos = new Vector3((float)__coordinates[preIndex-2], (float)__coordinates[preIndex-1], (float)__coordinates[preIndex]);
		Vector3 dir0 = new Vector3(cx, cy, cz) - prePos;
		Vector3 dir1 = new Vector3(x, y, z) - prePos;
		float dot = Vector3.Dot(dir0.normalized, dir1.normalized);
		
		if(Mathf.Abs(dot) > 0.999f){
			// ほぼ直線
			lineTo(x, y, z, rot);
		}else{
			__command.Add(PathType.__CURVE_TO);
			__cmd_index++;
			__coordinates.AddRange( new float[]{cx, cy, cz, x, y, z} );
			__quats.Add(rot);
		}
	}
	//
	
	public void moveTo ( Vector3 position, Quaternion rot ){
		moveTo(position.x, position.y, position.z, rot);
	}
	
	public void curveTo ( Vector3 cPosition, Vector3 position, Quaternion rot ){
		curveTo( cPosition.x, cPosition.y, cPosition.z, position.x, position.y, position.z, rot );
	}
	public void lineTo ( Vector3 position, Quaternion rot ){
		lineTo( position.x, position.y, position.z, rot );
	}
	
	
	// draw
	public void drawGizoms (){
		Gizmos.color = Color.magenta;
		
		int num = 200;
		float split = this.length / (num-1);
		
		Vector3 sPos0;
		Vector3 sPos1;
		
		sPos0 = this.getPointInfoVec3( 0 );
		for(int i = 0; i < num-1; i++){
			sPos1 = this.getPointInfoVec3( split * (i+1) );
			Gizmos.DrawLine(sPos0, sPos1);
			sPos0 = sPos1;
		}
	}
	
}
