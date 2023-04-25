using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{

	// Controls how quickly the camera responds to mouse input
	[SerializeField] private float sensitivity;

	// Used to rotate the player object based on camera movement
	[SerializeField] private Transform orientation;


	private float xRot;
	private float yRot;

	// Start is called before the first frame update
	void Start()
	{
		// Lock the cursor to the center of the screen and hide it
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	// OnDisable is called when the script is disabled or the game ends
	void OnDisable()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	// Update is called once per frame
	void Update()
	{
		// Get the horizontal and vertical mouse input and apply sensitivity
		float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
		float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

		// Add the mouse input to the current rotation values
		yRot += mouseX;
		xRot -= mouseY;

		// Clamp the x rotation to prevent the camera from flipping upside down
		xRot = Mathf.Clamp(xRot, -90f, 90f);

		// Apply the new rotation values to the camera and the orientation transform
		transform.rotation = Quaternion.Euler(xRot, yRot, 0);
		orientation.rotation = Quaternion.Euler(0, yRot, 0);
	}
}
