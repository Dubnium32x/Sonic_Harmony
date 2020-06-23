/*
Unity editor script to precisely move, rotate and scale GameObjects on 2D scenes, using the arrow keys.

Notes:
- To use it just include it on an "Assets/Editor" folder on your project.
- The action depends on the selected tool and the size of the movement depends on the Scene view zoom.
- The more "zoomed in" is the scene view the smaller is the movement step. 
- It will work when the current Scene tab is in 2D mode and there is at least one gameObject selected,
otherwise the scene camera will move as usual :)

David Darias. Lic: MIT. Dec 2017.
*/

using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class Arrows2DMovement {
	private const float viewportPercentRotation = 0.1f;
	private const float viewportPercentMovement = 0.001f;
	private const float viewportPercentScale = 0.001f;
	private const int rotationSnapAngle = 15;

	static Arrows2DMovement(){
		//avoid registering twice to the SceneGUI delegate
		SceneView.onSceneGUIDelegate -= OnSceneView;
		SceneView.onSceneGUIDelegate += OnSceneView;
	}

	static void OnSceneView(SceneView sceneView){
		var currentEvent = Event.current;
        //if the event is not a keyDown on an orthographic camera then return to avoid confusing behavior
        if (!currentEvent.isKey || currentEvent.type != EventType.KeyDown ||
            (currentEvent.modifiers != EventModifiers.None && currentEvent.modifiers != EventModifiers.FunctionKey) ||
            !sceneView.camera.orthographic) return;
        //choose the right direction to move
        switch (currentEvent.keyCode)
        {
	        case KeyCode.RightArrow:
		        moveSelectedObjects(Vector3.right, sceneView);
		        break;
	        case KeyCode.LeftArrow:
		        moveSelectedObjects(Vector3.left, sceneView);
		        break;
	        case KeyCode.UpArrow:
		        moveSelectedObjects(Vector3.up, sceneView);
		        break;
	        case KeyCode.DownArrow:
		        moveSelectedObjects(Vector3.down, sceneView);
		        break;
        }
	}

	private static void moveSelectedObjects(Vector3 direction, SceneView sceneView){
		//the step size is a percent of the scene viewport
		var cameraSize = getCameraSize(sceneView.camera);
		var step = Vector3.Scale(direction, cameraSize);
		//choose the transformation based on the selected tool
		Action<Transform,Vector3> transform;
		switch (Tools.current)
		{
			case Tool.Rotate:
				transform = rotateObject;
				break;
			case Tool.Scale:
				transform = scaleObject;
				break;
			default:
				transform = moveObject;
				break;
		}
		//get the current scene selection and move them
		var selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable | SelectionMode.ExcludePrefab) as GameObject[];
		//apply the transformation to every selected gameObject
        foreach (var mGameObject in selection)
	        transform( mGameObject.transform, step);

        //only consume the event if there was at least one gameObject selected, otherwise the camera will move as usual :)
		if(selection.Length>0) Event.current.Use();
	}

	private static Vector2 getCameraSize(Camera sceneCamera){
		var topRightCorner = sceneCamera.ViewportToWorldPoint(new Vector2(1,1));
		var bottomLeftCorner = sceneCamera.ViewportToWorldPoint(new Vector2(0, 0));
		return  (topRightCorner - bottomLeftCorner);
	}

	private static void rotateObject(Transform t, Vector3 rotation)
    {
        //allow undo of the rotation
        Undo.RecordObject(t, "Rotation Step");
		if(rotation.y!=0){
			//move the rotation to the nearest multiple of the snap angle.
			var currentRotation = t.rotation.eulerAngles;
			if(rotation.y>0){
				currentRotation.z = (Mathf.RoundToInt(currentRotation.z) / rotationSnapAngle) * rotationSnapAngle;
				currentRotation.z += rotationSnapAngle;
			}
			else{
				var current = Mathf.RoundToInt(currentRotation.z);
				if(current%rotationSnapAngle == 0) currentRotation.z -= rotationSnapAngle;
				else currentRotation.z = (current / rotationSnapAngle) * rotationSnapAngle;
			} 
			//set the new angle
			t.rotation = Quaternion.Euler(currentRotation);
		}
		else{
			t.Rotate(new Vector3(0,0, -rotation.x) * viewportPercentRotation);
		}
    }

	private static void scaleObject(Transform t, Vector3 scale)
    {
        //allow undo of the scale
        Undo.RecordObject(t, "Scale Step");
        t.localScale += scale * viewportPercentScale;
    }

	private static void moveObject(Transform t, Vector3 movement){
		//allow undo of the movements
		Undo.RecordObject(t, "Move Step");
		t.position += movement * viewportPercentMovement; 
	}
}
