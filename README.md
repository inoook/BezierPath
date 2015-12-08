# BezierPath
BezierPath for unity

![Sample](screen.png)

~~~cs
BezierPath path = new BezierPath (nodes, rots);

// v: 0 - 1
BezierPointInfo pInfo = path.GetBezierPointInfo(v);

/// <summary>
/// Bezier point info.
/// </summary>
public class BezierPointInfo
{
	public Vector3 point;
	public Quaternion rotation;
	public Vector3 tangent;
}
~~~
