using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Bezier;

public class BezierLayoutEditorWindow : EditorWindow {

	// Add menu named "My Window" to the Window menu
	[MenuItem ("Course/BezierLayout Create")]
	static void Init () {
		// Get existing open window or if none, make a new one:
		BezierLayoutEditorWindow window = (BezierLayoutEditorWindow)EditorWindow.GetWindow (typeof (BezierLayoutEditorWindow));
		window.Show();
	}

	BezierPathDrawer bezierPathDrawer;
	GameObject hoopPrefab;
	GameObject hoopCourse;

	void OnGUI () {
		GUILayout.Label ("Bezier Prot", EditorStyles.boldLabel);
		GUILayout.Label ("作成したいGameObjectを選択して、Createボタンを押す。");
		GUILayout.Label ("bezierPathDrawerにそってレイアウトする。");

		bezierPathDrawer = (BezierPathDrawer)EditorGUILayout.ObjectField("bezierPathDrawer", bezierPathDrawer, typeof(BezierPathDrawer), true);
		hoopPrefab = (GameObject)EditorGUILayout.ObjectField("prefab", hoopPrefab, typeof(GameObject), false);

		distance = EditorGUILayout.FloatField("layoutDistance", distance);

		GameObject createTarget = Selection.activeGameObject;
		if(createTarget == null){ return; }
		EditorGUILayout.SelectableLabel("CreateGObj: "+createTarget.name);

		hoopCourse = createTarget;
		if(createTarget != null){
			if( GUILayout.Button("Create") ){
				Create();
			}
//			if( GUILayout.Button("Clear") ){
//				Clear();
//			}
		}

		EditorGUILayout.SelectableLabel("pathLength: "+pathLength);
//		EditorGUILayout.se
//		myBool = EditorGUILayout.Toggle ("Toggle", myBool);
//		myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
//		EditorGUILayout.EndToggleGroup ();
	}

	private float split = 30;
	public float distance = 500;

	private float pathLength = 0;

	public void Create()
	{
		Clear();

		if(bezierPathDrawer == null){
			return;
		}

		// draw
		BezierPath path = bezierPathDrawer.GetBezierPath();
		pathLength = path.length;
		split = (pathLength / distance);
		float delta = 1.0f / split;


		for (int i = 0; i < split; i++) {

			float v = delta * i;

			BezierPointInfo pInfo = path.GetBezierPointInfo(v);

//			Hoop gObj = GameObject.Instantiate<Hoop>(hoopPrefab);
			GameObject gObj = ((GameObject)PrefabUtility.InstantiatePrefab (hoopPrefab));
			gObj.transform.SetParent(hoopCourse.transform);

			gObj.transform.position = pInfo.point;
			gObj.transform.rotation = pInfo.rotation;

			gObj.gameObject.name = "Prot_"+i;
		}
	}

	public void Clear()
	{
		Transform[] ts = hoopCourse.GetComponentsInChildren<Transform> ();
		for (int i = 0; i < ts.Length; i++) {
			if (ts [i] != hoopCourse.transform) {
				DestroyImmediate (ts [i].gameObject);
			}
		}
	}
}
