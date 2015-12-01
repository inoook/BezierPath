using UnityEngine;
using System.Collections;

namespace THREE
{
	public class Utils  {

		public static Vector2 clone (Vector2 vec)
		{
			return new Vector2 (vec.x, vec.y);
		}

		public static Vector3 clone (Vector3 vec)
		{
			return new Vector3 (vec.x, vec.y, vec.z);
		}

		public static void DebugLog(object message)
		{
			//Debug.Log(message);
		}
	}
}
