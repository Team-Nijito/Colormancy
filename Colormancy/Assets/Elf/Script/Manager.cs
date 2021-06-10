using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

	public GameObject m_CameraObj;
	public Camera m_camera;

	public bool left = false;
	public bool right = false;
	public bool cameraIn = false;
	public bool cameraOut = false;
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonUp (0))
		{
			left = false;
			right = false;
			cameraIn =false;
			cameraOut = false;
		}
		if(left == true)
		{
			m_CameraObj.transform.Rotate(Vector3.down, Time.deltaTime * 5);
		}
		if(right == true)
		{
			m_CameraObj.transform.Rotate(Vector3.up, Time.deltaTime * 5);	
		}
		if(cameraIn == true)
		{
			if(m_camera.fieldOfView >= 20)
			{
				m_camera.fieldOfView--;
			}
		}
		if(cameraOut == true)
		{
			if(m_camera.fieldOfView <= 60)
			{
				m_camera.fieldOfView++;
			}
		}
	}

	void OnGUI() {

	//	GUI.skin.button.fontSize = 10;

		if (GUI.RepeatButton(new Rect(Screen.width/2 - 170, 450, 100, 50), "Zoom In"))
		{
			cameraIn = true;
		}
		if (GUI.Button(new Rect(Screen.width/2 - 50, 450, 100, 50), "Zoom Reset"))
		{
			m_camera.fieldOfView = 40;
		}
		if (GUI.RepeatButton(new Rect(Screen.width/2 + 70, 450, 100, 50), "Zoom Out"))
		{
			cameraOut = true;
		}

		if (GUI.RepeatButton(new Rect(Screen.width/2 - 170, 520, 100, 50), "Camera Left"))
		{
			left = true;
		}
		if (GUI.Button(new Rect(Screen.width/2 - 50, 520, 100, 50), "Camera Reset"))
		{
			m_CameraObj.transform.eulerAngles = new Vector3(0,0,0);
		}
		if (GUI.RepeatButton(new Rect(Screen.width/2 + 70, 520, 100, 50), "Camera Right"))
		{
			right = true;
		}
	}
}