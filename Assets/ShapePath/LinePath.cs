using UnityEngine;
using System.Collections;

namespace ShapePath2{

	public class LinePath : IPath {

		//start point
		private float _x0;
		private float _y0;
		private float _z0;
		
		//end point
		private float _x1;
		private float _y1;
		private float _z1;
		
		//length
		private float _length;

		public LinePath ( float x0, float y0, float z0, float x1, float y1, float z1 ){
			_x0 = x0;
			_y0 = y0;
			_z0 = z0;
			_x1 = x1;
			_y1 = y1;
			_z1 = z1;
		}

		public LinePath clone (){
			return new LinePath( _x0, _y0, _z0, _x1, _y1, _z1 );
		}

		public bool isLine
		{
			get{ return true; }
		}

		// -------
		public Point3D f (  float t   ){
			return new Point3D( _x0 + ( _x1 - _x0 ) * t, _y0 + ( _y1 - _y0 ) * t, _z0 + ( _z1 - _z0 ) * t );
		}
		public Point3D diff (  float t   ){
			return new Point3D( ( _x1 - _x0 ), ( _y1 - _y0 ), ( _z1 - _z0 ) );
		}
		
		public VPoint3D vector (  float t   ){
			return new VPoint3D( _x0 + ( _x1 - _x0 ) * t, _y0 + ( _y1 - _y0 ) * t, _z0 + ( _z1 - _z0 ) * t, ( _x1 - _x0 ), ( _y1 - _y0 ), ( _z1 - _z0 ) );
		}

		// -------
		public float length{
			get{
				if ( _length < 0 )
					_length = Mathf.Sqrt( ( _x1 - _x0 ) * ( _x1 - _x0 ) + ( _y1 - _y0 ) * ( _y1 - _y0 ) + ( _z1 - _z0 ) * ( _z1 - _z0 ) );

				return _length; 
			}
		}

		public Point3D lengthToPoint (  float len   ){
			return f( lengthToValue( len ) );
		}
		
		public float lengthToValue (  float len   ){
			return len/length;
		}

		// -------

		public IPath[] split(float t )
		{
			if( t< 0 || t >1 ){
				//throw new ArgumentError("parameter t:0<t<1");
				return null;
			}
			
			if ( t == 0 ){
				return new IPath[]{ null, clone() };
			}else if ( t == 1 ){
				return new IPath[]{ clone(), null };
			}else{
				float x2 = _x0 + ( _x1 - _x0 ) * t;
				float y2 = _y0 + ( _y1 - _y0 ) * t;
				float z2 = _z0 + ( _z1 - _z0 ) * t;
				return new IPath[]{ new LinePath( _x0, _y0, _z0, x2, y2, z2 ), new LinePath( x2, y2, z2, _x1, _y1, _z1 ) };
			}
		}

		// -------
		public float x0{
			get{ return _x0; }
			set{
				_x0 = value;
				_length = -1;
			}
		}
		public float y0{
			get{ return _y0; }
			set{
				_y0 = value;
				_length = -1;
			}
		}
		public float z0{
			get{ return _z0; }
			set{
				_z0 = value;
				_length = -1;
			}
		}

		public float x1{
			get{ return _x1; }
			set{
				_x1 = value;
				_length = -1;
			}
		}
		public float y1{
			get{ return _y1; }
			set{
				_y1 = value;
				_length = -1;
			}
		}
		public float z1{
			get{ return _z1; }
			set{
				_z1 = value;
				_length = -1;
			}
		}
	}
}
