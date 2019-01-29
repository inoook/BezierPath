//
// Licensed under the MIT License
//
// Copyright (C) 2008-2009  TAKANAWA Tomoaki (http://nutsu.com) and
//					   		Spark project (www.libspark.org)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//


using UnityEngine;
using System.Collections;

public class QBezier : IPath
{

	//start point
	private float _x0;
	private float _y0;
	private float _z0;
	
	//end point
	private float _x1;
	private float _y1;
	private float _z1;

	// rot
	private Quaternion _rot0;
	private Quaternion _rot1;
	
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
	
	//
	//private float __err_d = 0.005f;
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
	public QBezier(float x0, float y0, float z0, 
					float cx, float cy, float cz, 
					float x1, float y1, float z1,
					Quaternion rot0, Quaternion rot1
	               )
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

		_rot0 = rot0;
		_rot1 = rot1;
	}
	
	/**
	 * Clone
	 */
	public QBezier clone()
	{
		QBezier c = new QBezier(_x0, _y0, _z0, _cx, _cy, _cz, _x1, _y1, _z1, _rot0, _rot1);
		
		if (float.IsNaN(_length) == false) {
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
	
	//--------------------------------------------------------------------------------------------------- F
	
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
	
	/**
	* Quadratic Bezier Function
	* @param	t( 0～1.0 )
	* @return	座標
	*/
	public PathPoint f(float t)
	{
		float tp = 1.0f - t;
		return new PathPoint(
					_x0 * tp * tp + 2.0f * _cx * t * tp + _x1 * t * t, 
					_y0 * tp * tp + 2.0f * _cy * t * tp + _y1 * t * t,
					_z0 * tp * tp + 2.0f * _cz * t * tp + _z1 * t * t);
	}
	
	public Vector3 fVec3(float t)
	{
		float tp = 1.0f - t;
		return new Vector3(
					_x0 * tp * tp + 2.0f * _cx * t * tp + _x1 * t * t, 
					_y0 * tp * tp + 2.0f * _cy * t * tp + _y1 * t * t,
					_z0 * tp * tp + 2.0f * _cz * t * tp + _z1 * t * t);
	}
	
	/**
	* Diff of Bezier Function
	* @param	t( 0～1.0 )
	* @return	ベクトル
	*/
	public PathPoint diff(float t)
	{
		return new PathPoint(2 * (t * (_x0 + _x1 - 2 * _cx) - _x0 + _cx), 2 * (t * (_y0 + _y1 - 2 * _cy) - _y0 + _cy), 2 * (t * (_z0 + _z1 - 2 * _cz) - _z0 + _cz));
	}
	
	public PathVector vector(float t)
	{
		float tp = 1.0f - t;
		return new PathVector(_x0 * tp * tp + 2.0f * _cx * t * tp + _x1 * t * t, 
								_y0 * tp * tp + 2.0f * _cy * t * tp + _y1 * t * t,
								_z0 * tp * tp + 2.0f * _cz * t * tp + _z1 * t * t,
								2.0f * (t * (_x0 + _x1 - 2.0f * _cx) - _x0 + _cx), 
								2.0f * (t * (_y0 + _y1 - 2.0f * _cy) - _y0 + _cy),
								2.0f * (t * (_z0 + _z1 - 2.0f * _cz) - _z0 + _cz),
		                      Quaternion.Lerp(_rot0, _rot1, t)
		                      );
	}
	
	//--------------------------------------------------------------------------------------------------- LENGTH_T
	
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
	public PathPoint lengthToPoint(float len)
	{
		return f(lengthToValue(len));
	}
	
	/**
	* integrate of bezier curve
	* @param	t( 0～1.0 )
	* @return	積分値
	*/
	public float integrate(float t)
	{
		return (integrateF(t) - INTG_0);
	}
	
	/**
	* get value from length
	* @param	len		target lenth (0,length)
	* @return 	t value
	*/
	public float lengthToValue(float len_)
	{
//		if (len_ < 0 || len_ > length) {
//			return float.NaN;
		if (len_ < 0) {
			return 0.0f;
		} else if (len_ > length) {
			return 1.0f;
		} else {
			return __seek(len_, __err_d);
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
	private float __seek(float len_, float d, float t0 = 0.5f, float td = 0.25f)
	{
		float lent0 = integrate(t0);
		if (Mathf.Abs(len_ - lent0) < d)
			return t0;
		else
			return __seek(len_, d, (lent0 < len_) ? t0 + td : t0 - td, td / 2.0f);
	}
	/*
	private float __seek( float len_, float d )
	{
		return __seek(len_, d, 0.5f, 0.25f);
		//return __seek(len_, d, 1.0f, 0.5f);
	}
	*/
	
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
			_length = 0;
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
			_length = integrate(1.0f);
		}
	}
	
	/**
	* 積分関数
	* @param	t( 0～1.0f )
	* @return	積分結果
	*/
	private float integrateF(float t)
	{
		float BT = B + t;
		float BTS = Mathf.Sqrt(BT * BT + C);
		return Mathf.Sqrt(XY) * (BTS * BT + C * Mathf.Log((BT + BTS) / CS + CS2));
	}
		
	//--------------------------------------------------------------------------------------------------- COORDINATES
	
	/**
	* offset coordinates
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
		
	public Vector3 point0 {
		get { 
			return new Vector3(_x0, _y0, _z0);
		}
		set {
			_x0 = value.x; 
			_y0 = value.y;
			_z0 = value.z; 
			_length = float.NaN;
		}
	}
	
	public Vector3 point1 {
		get { return new Vector3(_x1, _y1, _z1); }
		set {
			_x1 = value.x; 
			_y1 = value.y;
			_z1 = value.z; 
			_length = float.NaN;
		}
	}
	
	public Vector3 control {
		get { return new Vector3(_cx, _cy, _cz); }
		set {
			_cx = value.x; 
			_cx = value.y;
			_cz = value.z; 
			_length = -1;
		}
	}		
	// 
	
	public Vector3[] points {
		get {
			Vector3[] p = new Vector3[3];
			p[0] = point0;
			p[1] = control;
			p[2] = point1;
			
			return p;
		}
	}
	
	//------
	
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
