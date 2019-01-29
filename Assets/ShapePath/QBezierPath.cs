using UnityEngine;
using System.Collections;

namespace ShapePath2{

	public class QBezierPath : IPath {

		//start point
		private float _x0;
		private float _y0;
		private float _z0;
		
		//end point
		private float _x1;
		private float _y1;
		private float _z1;
		
		//control point
		private float _cx;
		private float _cy;
		private float _cz;

		//length of bezier line
		private float _length;
		
		//fixed numbers for integration
		private float XY;
		private float B;
		private float C;
		private float CS;
		private float CS2;
		private float INTG_0;

		private float __err_d = 0.01f;

		/**
		 * Quadratic Bezier Constructor
		 * @param	start x
		 * @param	start y
		 * @param	end x
		 * @param	end y
		 * @param	control x
		 * @param	control y
		 */
		public QBezierPath(float x0, float y0, float z0, 
		               float cx, float cy, float cz, 
		               float x1, float y1, float z1)
		{
			_x0 = x0;
			_y0 = y0;
			_z0 = z0;
			
			_cx = cx;
			_cy = cy;
			_cz = cz;
			
			_x1 = x1;
			_y1 = y1;
			_z1 = z1;
			_length = -1;
		}

		/**
		 * Clone
		 */
		public QBezierPath clone()
		{
			QBezierPath c = new QBezierPath(_x0, _y0, _z0, _cx, _cy, _cz, _x1, _y1, _z1);
			
			if ( _length > 0 ){
				c._length = _length;

				c.XY = XY;
				c.B = B;
				c.C = C;
				c.CS = CS;
				c.CS2 = CS2;
				c.INTG_0 = INTG_0;
			}
			return c;
		}

		public bool isLine
		{
			get{
				return false;
			}
		}
		//------
		// Static
		//------
		/**
		 * Quadratic Bezier Function
		 */
		public static float point(float a, float b, float c, float t)
		{
			float tp = 1.0f - t;
			return a * tp * tp + 2 * b * t * tp + c * t * t;
		}
		/**
		 * Quadratic Bezier Tangent
		 */
		public static float tangent(float a, float b, float c, float t)
		{
			return 2 * (t * (a + c - 2 * b) - a + b);
		}
		//------
		// Function
		//------
		/**
		* Quadratic Bezier Function
		* @param	t( 0～1.0 )
		* @return	座標
		*/
		public Point3D f(float t)
		{
			float tp = 1.0f - t;
			return new Point3D(
				_x0 * tp * tp + 2.0f * _cx * t * tp + _x1 * t * t, 
				_y0 * tp * tp + 2.0f * _cy * t * tp + _y1 * t * t,
				_z0 * tp * tp + 2.0f * _cz * t * tp + _z1 * t * t);
		}

		/**
		* Diff of Bezier Function
		* @param	t( 0～1.0 )
		* @return	ベクトル
		*/
		public Point3D diff(float t)
		{
			return new Point3D(2 * (t * (_x0 + _x1 - 2 * _cx) - _x0 + _cx), 2 * (t * (_y0 + _y1 - 2 * _cy) - _y0 + _cy), 2 * (t * (_z0 + _z1 - 2 * _cz) - _z0 + _cz));
		}

		/**
		 * Point and Diff
		 */
		public VPoint3D vector(float t)
		{
			float tp = 1.0f - t;
			return new VPoint3D(_x0 * tp * tp + 2.0f * _cx * t * tp + _x1 * t * t, 
			                      _y0 * tp * tp + 2.0f * _cy * t * tp + _y1 * t * t,
			                      _z0 * tp * tp + 2.0f * _cz * t * tp + _z1 * t * t,
			                      2.0f * (t * (_x0 + _x1 - 2.0f * _cx) - _x0 + _cx), 
			                      2.0f * (t * (_y0 + _y1 - 2.0f * _cy) - _y0 + _cy),
			                      2.0f * (t * (_z0 + _z1 - 2.0f * _cz) - _z0 + _cz));
		}

		//------
		// Length
		//------
		/**
		* Length of bezier curve
		* @return	長さ
		*/
		public float length {
			get {
				if (_length < 0)
					__integrateInit();
				return _length;
			}
		}

		/**
		 * get coordinates by length
		 * @param	len
		 */
		public Point3D lengthToPoint(float len)
		{
			return f(lengthToValue(len));
		}

		/**
		* get value from length
		* @param	len		target lenth (0,length)
		* @return 	t value
		*/
		public float lengthToValue(float len)
		{
			//		if (len_ < 0 || len_ > length) {
			//			return float.NaN;
			if (len < 0) {
				return 0.0f;
			} else if (len > length) {
				return length;
			} else {
				return __seek(len, __err_d);
			}

//			if ( len < 0 || len > length ) {
//				Debug.LogWarning("QBezierErr: " + len );
//				return __seek( len, __err_d );//Number.NaN;
//			}else{
//				return __seek( len, __err_d );
//			}
		}

		/**
		* integrate of bezier curve
		* @param	t( 0～1.0 )
		*/
		public float valutToLength(float t)
		{
			return (integrateF(t) - INTG_0);
		}

		//------
		// Cut, Clip
		//------
		/**
		 * Split Bezier at t.
		 * @param	t	( 0 to 1 )
		 * @return	QBezierPath[]
		 */
		public IPath[] split( float t )
		{
			if( t<0 || t>1 ){
				//throw new ArgumentError("parameter t:0<t<1");
				Debug.LogError("parameter t:0<t<1");
			}
			
			if ( t == 0 ){
				return new IPath[]{ null, clone() };
			}else if ( t == 1 ){
				return new IPath[]{ clone(), null };
			}else{
				float tp = 1-t;
				float ax = _x0*tp + _cx*t;
				float ay = _y0*tp + _cy*t;
				float az = _z0*tp + _cz*t;

				float bx = _cx*tp + _x1*t;
				float by = _cy*tp + _y1*t;
				float bz = _cz*tp + _z1*t;

				float px = ax * tp + bx * t;
				float py = ay * tp + by * t;
				float pz = az * tp + bz * t;
				return new IPath[]{ new QBezierPath( _x0, _y0, _z0, ax, ay, az, px, py, pz ), new QBezierPath( px, py, pz, bx, by, bz, _x1, _y1, _z1 )};
			}
		}

		/**
		 * Clip Bezier between t0 to t1.
		 * @param	t0
		 * @param	t1
		 * @return	QBezierPath
		 */
		public QBezierPath clip(float t0, float t1 )
		{
			//coordinates and vector of edges
			float tp0 = 1-t0;
			float tp1 = 1-t1;
			float tx0 = _x0*tp0*tp0 + 2*_cx*t0*tp0 + _x1*t0*t0; 
			float ty0 = _y0*tp0*tp0 + 2*_cy*t0*tp0 + _y1*t0*t0;
			float tz0 = _z0*tp0*tp0 + 2*_cz*t0*tp0 + _z1*t0*t0;
			float tx1 = _x0*tp1*tp1 + 2*_cx*t1*tp1 + _x1*t1*t1;
			float ty1 = _y0*tp1*tp1 + 2*_cy*t1*tp1 + _y1*t1*t1;
			float tz1 = _z0*tp1*tp1 + 2*_cz*t1*tp1 + _z1*t1*t1;
			float vx0 = 2*( t0*( _x0 + _x1 - 2*_cx ) - _x0 + _cx ); 
			float vy0 = 2*( t0*( _y0 + _y1 - 2*_cy ) - _y0 + _cy );
			float vz0 = 2*( t0*( _z0 + _z1 - 2*_cz ) - _z0 + _cz );
			float vx1 = 2*( t1*( _x0 + _x1 - 2*_cx ) - _x0 + _cx ); 
			float vy1 = 2*( t1*( _y0 + _y1 - 2*_cy ) - _y0 + _cy );
			float vz1 = 2*( t1*( _z0 + _z1 - 2*_cz ) - _z0 + _cz );
			//delta of edges
			float dx = tx1 - tx0;
			float dy = ty1 - ty0;
			float dz = tz1 - tz0;
			
			// Point(dx,dy) = a*(pv0 + pv1)
			float a = ( dx != 0 ) ? dx/(vx0+vx1) : dy/(vy0+vy1);
			a = ( dz != 0 ) ? dz/(vz0+vz1) : a;
			
			return new QBezierPath( tx0, ty0, tz0, tx0+a*vx0, ty0+a*vy0, tz0+a*vz0, tx1, ty1, tz1 );
		}

		/**
		 * Clip Bezier by rectangle
		 * @param	Cut Rectangle
		 * @return	QBezierPath[]
		 */
		/*
		public function clipByRect( cutRect:Rectangle ):Array
		{
			var ts:Array = clipTValueByRect(cutRect);
			//result
			var res:Array = [];
			for( var i:int=0; i<ts.length ;i+=2 )
				res.push( clip(ts[i],ts[i+1]) );
			
			return res;
		}
		*/
		/**
		 * Clip tVlaue by rectangle
		 * @param	Cut Rectangle
		 * @return	t[]
		 * @private
		 */
		/*
		internal function clipTValueByRect( cutRect:Rectangle ):Array
		{
			var rect:Rectangle = getRect();
			if ( cutRect.intersects( rect ) )
			{
				//内包かどうか
				if ( cutRect.containsRect( rect ) )
				{
					return [0,1];
				}
				
				var ts:Array = QBezierUtil.intersectionTByRect(this,cutRect);
				if( cutRect.contains( _x0, _y0 ) && ts[0]>1e-10 ) ts.unshift(0);
				if( cutRect.contains( _x1, _y1 ) && ts[ts.length-1]<(1-1e-10) ) ts.push(1);
				
				//端点のrect.containsが微妙に外れる場合の誤差処理
				if ( ts.length == 1 )
				{
					if( cutRect.containsPoint( f(ts[0]-1e-10) ) ){
						ts.unshift( 0 );
					}else{
						ts.push( 1 );
					}
				}
				
				//端点が交点の場合
				if ( ts.length == 3 )
				{
					if( cutRect.containsPoint( f((ts[1]+ts[0])/2) ) ){
						ts.pop();
					}else{
						ts.shift();
					}
				}
				
				//result
				return ts;	
			}
			else
			{
				return [];
			}
		}
		*/
		//------------------------------------------------------------------------------INTERSECTION
		
		/**
		 * Rectangle of bezier curve
		 */
		/*
		public Rectangle getRect()
		{
			float minX;
			float minY;
			float maxX;
			float maxY;
			
			if ( _x0 > _x1 ){
				maxX = _x0; minX = _x1;
			}else {
				maxX = _x1; minX = _x0;
			}
			if ( _y0 > _y1 ){
				maxY = _y0; minY = _y1;
			}else {
				maxY = _y1; minY = _y0;
			}
			
			float xt = (_x0 - _cx)/( _x0 + _x1 - 2*_cx );
			float yt = (_y0 - _cy)/( _y0 + _y1 - 2*_cy );
			if( xt>0 && xt<1 ){
				xt = _x0*(1-xt)*(1-xt) + 2*_cx*xt*(1-xt) + _x1*xt*xt;
				if ( xt > maxX ) maxX = xt;
				if ( xt < minX ) minX = xt;
			}
			if( yt>0 && yt<1 ){
				yt = _y0*(1-yt)*(1-yt) + 2*_cy*yt*(1-yt) + _y1*yt*yt;
				if ( yt > maxY ) maxY = yt;
				if ( yt < minY ) minY = yt;
			}
			return new Rectangle( minX, minY, maxX - minX, maxY - minY );
		}
		*/
		/**
		 * fat line for intersection
		 */
		/*
		public function getFatLine():Array
		{
			var vx:Number = _x1 - _x0;
			var vy:Number = _y1 - _y0;
			var vcx:Number = point( _x0, _cx, _x1, 0.5 ) - _x0;
			var vcy:Number = point( _y0, _cy, _y1, 0.5 ) - _y0;
			var vlen:Number = Math.sqrt( vx * vx + vy * vy );
			var d:Number = (vx * vcy - vy * vcx) / vlen;
			vx /= vlen;
			vy /= vlen;
			var vdx:Number = -vy * d;
			var vdy:Number = vx * d;
			return [ _x0, _y0, _x1, _y1, _x1 + vdx, _y1 + vdy, _x0 + vdx, _y0 + vdy ];
		}
		*/

		//--------------------------------------------------------------------------------------------------- INTEGRATE
		
		/**
	* 積分定数初期化
	*/
		private void __integrateInit()
		{
			float kx = _x0 + _x1 - 2.0f * _cx;
			float ky = _y0 + _y1 - 2.0f * _cy;
			float kz = _z0 + _z1 - 2.0f * _cz;

			float ax = - _x0 + _cx;
			float ay = - _y0 + _cy;
			float az = - _z0 + _cz;
			
			if (kx == 0 && ky == 0 && kz == 0) {
				XY = 0;
				B = 0;
				C = 0;
				CS = CS2 = 1.0f;
				INTG_0 = 0;
				_length = Mathf.Sqrt( (_x1 - _x0) * (_x1 - _x0) + (_y1 - _y0) * (_y1 - _y0) + (_z1 - _z0) * (_z1 - _z0));
			} else {
				//積分計算の為の定数
				XY = kx * kx + ky * ky + kz * kz;
				B = (ax * kx + ay * ky + az * kz) / XY;
				C = (ax * ax + ay * ay + az * az) / XY - B * B;
				if (C > 1e-10) {
					CS = Mathf.Sqrt(C);
					CS2 = 0.0f;
				} else {
					C = 0.0f;
					CS = CS2 = 1.0f;
				}
				INTG_0 = integrateF(0.0f);
				
				//長さ
				_length = valutToLength(1.0f);
			}
		}

		/**
		* 積分関数
		* @param	t( 0～1.0f )
		* @return	積分結果
		*/
		private float integrateF(float t)
		{
			if( XY > 0 ){
				float BT = B + t;
				float BTS = Mathf.Sqrt(BT * BT + C);
				return Mathf.Sqrt(XY) * (BTS * BT + C * Mathf.Log((BT + BTS) / CS + CS2));
			}else{
				return _length * t;
			}
		}

		/**
		* seek value
		* @param	len		target length
		* @param	d		許容誤差
		* @param	t0		check t value
		* @param	td		next check t value
		* @return 	t value
		*/
		private float __seek(float len, float d = 0.1f, float t0 = 0.5f, float td = 0.25f)
		{
			float len_t0 = valutToLength(t0);
			bool dt = (len_t0 < len);
			if( (dt && ( len - len_t0 )<d ) || ( !dt && ( len_t0 - len )<d ) )
				return t0;
			else
				return __seek( len, d, dt ? t0+td : t0-td, td*0.5f );
		}

		//--------------------------------------------------------------------------------------------------- 
		// COORDINATES
		//---------------------------------------------------------------------------------------------------
		/* offset coordinates
		* @param	x offset
		* @param	y offset
		*/
		public void offset(float x, float y, float z)
		{
			_x0 += x;
			_y0 += y;
			_z0 += z;
			
			_x1 += x;
			_y1 += y;
			_z1 += z;
			
			_cx += x;
			_cx += y;
			_cz += z;
		}

		public float x0 {
			get { return _x0; }
			set {
				_x0 = value; 
				_length = -1;
			}
		}
		public float y0 {
			get { return _y0; }
			set {
				_y0 = value; 
				_length = -1;
			}
		}
		public float z0 {
			get { return _z0; }
			set {
				_z0 = value; 
				_length = -1;
			}
		}
		
		public float x1 {
			get { return _x1; }
			set {
				_x1 = value; 
				_length = -1;
			}
		}
		public float y1 {
			get { return _y1; }
			set {
				_y1 = value; 
				_length = -1;
			}
		}
		public float  z1 {
			get { return _z1; }
			set {
				_z1 = value; 
				_length = -1;
			}
		}
		
		public float cx {
			get { return _cx; }
			set {
				_cx = value; 
				_length = -1;
			}
		}
		public float cy {
			get { return _cy; }
			set {
				_cy = value; 
				_length = -1;
			}
		}
		public float cz {
			get { return _cz; }
			set {
				_cz = value; 
				_length = -1;
			}
		}
	}
}