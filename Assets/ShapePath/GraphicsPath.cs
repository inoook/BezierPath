using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ShapePath2{
	public class GraphicsPath {

		public GraphicsPath(List<int> commands  = null, List<float> data = null, string winding = "evenOdd")
		{

		}
	}

	public enum GraphicsPathCommand
	{
		MOVE_TO, LINE_TO, CURVE_TO, WIDE_LINE_TO, WIDE_MOVE_TO
	}

}
