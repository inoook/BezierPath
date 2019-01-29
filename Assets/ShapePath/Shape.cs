using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ShapePath2;

public class Shape {

	public List<int> commands;
	public List<float> data;

	public Shape()
	{
		commands = new List<int>();
		data = new List<float>();
	}

	public void moveTo(Vector3 pos)
	{
		commands.Add((int)GraphicsPathCommand.MOVE_TO);

		data.Add(pos.x);
		data.Add(pos.y);
		data.Add(pos.z);
	}
	public void curveTo(Vector3 pos, Vector3 cPos)
	{
		commands.Add((int)GraphicsPathCommand.CURVE_TO);
		data.Add(pos.x);
		data.Add(pos.y);
		data.Add(pos.z);
		data.Add(cPos.x);
		data.Add(cPos.y);
		data.Add(cPos.z);
	}
	public void lineTo(Vector3 pos)
	{
		commands.Add((int)GraphicsPathCommand.LINE_TO);
		data.Add(pos.x);
		data.Add(pos.y);
		data.Add(pos.z);
	}
}
