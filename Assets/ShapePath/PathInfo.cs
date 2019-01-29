using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShapePath2{

	public class PathInfo {

		public List<IPath> paths;
		private float _length;
		private List<float> _lengths;
		
		public PathInfo(List<int> commands =null, List<float>data=null ) 
		{
			if ( commands != null && data != null )
				build( commands, data );
			else
				_length = -1;
		}

		// -----
		public VPoint3D lengthToVector(float targetLength )
		{
			int pi = lengthToIndex( targetLength );
//			float len;
//			if ( pi > 0 ) {
//				targetLength -= _lengths[(int)(pi - 1)];
//			}else if ( pi < 0 ) {
//				return null;
//			}
			if ( pi > 0 && pi < paths.Count) {
				targetLength -= _lengths[(int)(pi - 1)];
			}else if ( pi < 0 ) {
				//return null;
//				Debug.LogWarning("MIN");
				IPath pS = paths[0];
				return pS.vector( 0 );
			}else if ( pi == paths.Count){
//				Debug.LogWarning("MAX");
				IPath pE = paths[paths.Count-1];
				return pE.vector( 1 );
			}

			IPath p = paths[pi];
			float t = p.lengthToValue( targetLength );
			return p.vector( t );
		}

		public Point3D lengthToPoint( float targetLength )
		{
			int pi = lengthToIndex( targetLength );
//			if ( pi > 0 ) {
//				targetLength -= _lengths[(int)(pi - 1)];
//			}else if ( pi < 0 ) {
//				return null;
//			}

			if ( pi > 0 && pi < paths.Count) {
				targetLength -= _lengths[(int)(pi - 1)];
			}else if ( pi < 0 ) {
				//return null;
				Debug.LogWarning("MIN");
				IPath pS = paths[0];
				float tS = pS.lengthToValue( 0 );
				return pS.f( tS );
			}else if ( pi == paths.Count){
				Debug.LogWarning("MAX");
				IPath pE = paths[paths.Count-1];
				targetLength -= _lengths[(int)(paths.Count - 1)];
				float tE = pE.lengthToValue(targetLength );
				return pE.f( tE );
			}

			IPath p = paths[pi];
			float t = p.lengthToValue( targetLength );
			return p.f( t );
		}

		public int lengthToIndex( float targetLength )
		{
//			if ( targetLength < 0 || targetLength > _length ){
//				return -1;
//			}

			if ( targetLength < 0){
				return -1;
			}else if( targetLength >= _length ){
//				Debug.LogWarning("over");
				return paths.Count;
			}
			
			int n = paths.Count;
			for ( int i = 0; i < n; i++ ){
				if ( targetLength <= _lengths[i] ) {
					return i;
				}
			}
			return -1;
		}

		//----
		// merge path
		//----
		public static GraphicsPath mergePath( PathInfo p0, PathInfo p1 )
		{
			//交点を求めてパスを分割
			
			//パスを結合
			
			return null;
		}
		//----
		// merge graphics path
		//----
		/**
		 * merge path commands
		 */
		public static List<GraphicsPath> mergePathCommands( PathInfo p0, PathInfo p1, float err=0.1f )
		{
			if ( p0.paths.Count == 0 || p1.paths.Count == 0 ) {
				//throw new Error("[mergePathCommands] PathInfo has no path.")
				Debug.LogError("[mergePathCommands] PathInfo has no path.");
			}
			//segment 1st
			List<List<IPath>> paths0 = new List<List<IPath>>();
			List<List<IPath>> paths1 = new List<List<IPath>>();
			_segmentPathToCompare( p0, p1, err, paths0, paths1 );
			
			//segment 2nd make graphics paths
			List<int> c0 = new List<int>();
			List<float> d0 = new List<float>();
			List<float> d1 = new List<float>();
			_mergeGraphicsPathCommands( c0, d0, d1, paths0, paths1 );
			
			//make result
			List<GraphicsPath> res;
			//res = new Vector.<GraphicsPath>(2, true);
			res = new List<GraphicsPath>();
			res[0] = new GraphicsPath(c0, d0);
			res[1] = new GraphicsPath(c0, d1);
			return res;
		}

		private static void _segmentPathToCompare(PathInfo p0, PathInfo p1, float err, List<List<IPath>> paths0, List<List<IPath>> paths1 )
		{
			int n0      = p0.paths.Count;
			int n1      = p1.paths.Count;
			float len0 = p0.length;
			float len1 = p1.length;
			int b0 = 0;
			int b1 = 0;
			int i0 = 0;
			int i1 = 0;
			bool last0 = false;
			bool last1 = false;
			err = err * 2.0f / (n0 + n1);
			float s0 = p0._lengths[0] / len0;
			float s1 = p1._lengths[0] / len1;
			bool flg = true;
			while ( flg ) {
				bool gt = ( s0 > s1 );
				float d  = ( gt ) ? s0 - s1 : s1 - s0;
				if ( d < err ) {
					i0++;
					i1++;
					s0 = p0._lengths[i0] / len0;
					s1 = p1._lengths[i1] / len1;
					//paths0.Add( p0.path.slice( b0, i0 ) );
					paths0.Add( p0.paths.GetRange( b0, b0 + i0 ) );
					paths1.Add( p1.paths.GetRange( b1, b1 + i1 ) );
					last0 = ( i0 >= n0 - 1 );
					last1 = ( i1 >= n1 - 1 );
					b0 = i0;
					b1 = i1;
				}else if ( gt ) {
					i1++;
					s1 = p1._lengths[i1] / len1;
					last1 = ( i1 >= n1 - 1 );
				}else {
					i0++;
					s0 = p0._lengths[i0] / len0;
					last0 = ( i0 >= n0 - 1 );
				}
				if ( last0 || last1 ) {
					paths0.Add( p0.paths.GetRange( b0, b0 + n0 ) );
					paths1.Add( p1.paths.GetRange( b1, b1 + n1 ) );
					flg = false;
				}
			}
		}


		private static void _mergeGraphicsPathCommands( List<int> c0, List<float> d0, List<float> d1, List<List<IPath>> paths0, List<List<IPath>> paths1 )
		{
			IPath pt0 = paths0[0][0];
			IPath pt1 = paths1[0][0];

			c0.Add( (int)GraphicsPathCommand.MOVE_TO );
			d0.AddRange(new List<float>(){ pt0.x0, pt0.y0 });
			d1.AddRange(new List<float>(){ pt1.x0, pt1.y0 });
			
			int sn = paths0.Count;
			for ( int i = 0; i < sn ; i++ ) {
				List<IPath> ps0 = paths0[i];
				List<IPath> ps1 = paths1[i];
				int n0 = ps0.Count;
				int n1 = ps1.Count;
				if ( n0 == 1 && n1 == 1 ) {
					_addmergePathCommand( ps0[0], ps1[0], c0, d0, d1 );
					continue;
				}
				int k;
				float len0 = 0;
				float len1 = 0;
				for ( k = 0; k < n0; k++ )
					len0 += ps0[k].length;
				for ( k = 0; k < n1; k++ )
					len1 += ps1[k].length;
				pt0  = ps0[0];
				pt1  = ps1[0];
				float s0 = pt0.length / len0;
				float s1 = pt1.length / len1;
				float t0 = pt0.length;
				float t1 = pt1.length;
				
				int i0 = 0;
				int i1 = 0;
				float splen;
				IPath[] segments;
				IPath tp;
				bool flg = true;
				//trace( "-----------------\n", n0, n1, len0, len1 );				
				while ( flg ) {
					//trace("---", s0, s1 );
					if ( s0 > s1 ) {
						splen = pt0.length - (s0 - s1) * len0;
						//if( splen<0 || splen>pt0.length  ) trace( "len0", splen, pt0.length, splen/pt0.length, t0, t1);
						segments = pt0.split( pt0.lengthToValue(splen) );
						tp = segments[0];
						_addmergePathCommand( tp, pt1, c0, d0, d1 );
						pt0 = segments[1];
						i1++;
						//trace( "S1", i0, ps0.length , i1, ps1.length );
						pt1 = ps1[i1];
						s1 += pt1.length / len1;
						t1 += pt1.length;
						//trace("t1",s1,i1, pt1.length, len1)
					}else {
						splen = pt1.length - (s1 - s0) * len1;
						//if ( splen < 0 || splen>pt1.length ) trace( "len1", splen, pt1.length, splen/pt1.length, t0, t1);
						segments = pt1.split( pt1.lengthToValue(splen) );
						tp = segments[0];
						_addmergePathCommand( pt0, tp, c0, d0, d1 );
						pt1 = segments[1];
						i0++;
						//trace( "S0", i0, ps0.length , i1, ps1.length );
						pt0 = ps0[i0];
						s0 += pt0.length / len0;
						t0 += pt0.length;
						//trace("t0",s0, i0, pt0.length, len0)
					}
					if ( i0 >= n0 - 1 && i1 >= n1 - 1 ) {
						_addmergePathCommand( pt0, pt1, c0, d0, d1 );
						flg = false;
					}
				}
			}
		}

		private static void _addmergePathCommand( IPath pt0, IPath pt1, List<int> commands, List<float> d0, List<float> d1 )
		{
			if ( pt0.isLine ) {
				if ( pt1.isLine ) {
					commands.Add( (int)GraphicsPathCommand.LINE_TO );
					d0.AddRange(new List<float>(){ pt0.x1, pt0.y1, pt0.z1 } );
					d1.AddRange(new List<float>(){ pt1.x1, pt1.y1, pt1.z1 } );
				}else {
					commands.Add( (int)GraphicsPathCommand.CURVE_TO );
					d0.AddRange(new List<float>(){  (pt0.x0+pt0.x1)*0.5f, (pt0.y0+pt0.y1)*0.5f, (pt0.z0+pt0.z1)*0.5f, pt0.x1, pt0.y1, pt0.z1 } );
					d1.AddRange(new List<float>(){  ((QBezierPath)pt1).cx, ((QBezierPath)pt1).cy, ((QBezierPath)pt1).cz, pt1.x1, pt1.y1, pt1.z1 } );
				}
			}else {
				commands.Add( (int)GraphicsPathCommand.CURVE_TO );
				d0.AddRange(new List<float>(){ ((QBezierPath)pt0).cx, ((QBezierPath)pt0).cy, ((QBezierPath)pt0).cz, pt0.x1, pt0.y1, pt0.z1 });
				if ( pt1.isLine ) {
					d1.AddRange(new List<float>(){ (pt1.x0+pt1.x1)*0.5f, (pt1.y0+pt1.y1)*0.5f, (pt1.z0+pt1.y1)*0.5f, pt1.x1, pt1.y1, pt1.z1 });
				}else {
					d1.AddRange(new List<float>(){ ((QBezierPath)pt1).cx, ((QBezierPath)pt1).cy, ((QBezierPath)pt1).cz, pt1.x1, pt1.y1, pt1.z1 });
				}
			}
		}

		//----
		// length
		//----

		public float length {
			get{
				if ( _length < 0 )
					update();
				return _length;
			}
		}
		
		public void update() {
			int len = paths.Count;
			_length = 0;
			_lengths = new List<float>(len);

			for ( int i = 0; i < len; i++ ) {
				float d = paths[i].length;
				_length += d;
				//_lengths[i] = _length;
				_lengths.Add(_length);
			}
		}

		//----
		// build
		//----
		/**
		 * GraphicsPath to Path Object
		 * @param	commands
		 * @param	data
		 */
		public void build( List<int> commands, List<float> data ) 
		{
			paths = new List<IPath>();
			int di = 0;
			int len = commands.Count;
			float px = 0;
			float py = 0;
			float pz = 0;
			float x, y, z;
			float cx, cy, cz;
			for ( int i = 0; i < len; i++ ) {
				int c = commands[i];
				if ( c == (int)GraphicsPathCommand.LINE_TO ){
					x = data[di]; di++;
					y = data[di]; di++;
					z = data[di]; di++;
					if ( px != x || py != y || pz != z ) paths.Add( new LinePath( px, py,pz, x, y, z ) );
					px = x;
					py = y;
					pz = z;
				}else if ( c == (int)GraphicsPathCommand.CURVE_TO ){
					cx = data[di]; di++;
					cy = data[di]; di++;
					cz = data[di]; di++;
					x = data[di]; di++;
					y = data[di]; di++;
					z = data[di]; di++;
					if ( px != x || px != cx || py != y || py != cy  || pz != z || pz != cz ) {
						paths.Add( new QBezierPath( px, py, pz, cx, cy, cz, x, y, z ) );
					}
					px = x;
					py = y;
					pz = z;
				}else if ( c == (int)GraphicsPathCommand.WIDE_LINE_TO ){
					di += 3;
					x = data[di]; di++;
					y = data[di]; di++;
					z = data[di]; di++;
					if ( px != x || py != y || pz != z) paths.Add( new LinePath( px, py, pz, x, y, z ) );
					px = x;
					py = y;
					pz = z;
				}else if ( c == (int)GraphicsPathCommand.MOVE_TO ){
					px = data[di]; di++;
					py = data[di]; di++;
					pz = data[di]; di++;
				}else if ( c == (int)GraphicsPathCommand.WIDE_MOVE_TO ){
					di += 3;
					px = data[di]; di++;
					py = data[di]; di++;
					pz = data[di]; di++;
				}
			}
			update();
		}

	}
}
