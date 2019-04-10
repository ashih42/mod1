using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private const float ROTATION_LOWER_LIMIT = -70f;
	private const float ROTATION_UPPER_LIMIT = 70f;

	[SerializeField] private float moveSpeed;
	[SerializeField] private float rotateSpeed;

	private float rotateV;

	private void Start()
	{
		this.rotateV = 0f;
	}

	private void Update()
	{
		//if (Input.GetKey(KeyCode.LeftControl))
			this.MoveCamera();

		if (Input.GetKey(KeyCode.LeftShift))
			this.RotateCamera();
	}

	private void MoveCamera()
	{
		if (Input.GetKey(KeyCode.W))
			this.transform.Translate(Vector3.forward * this.moveSpeed * Time.deltaTime);
		if (Input.GetKey(KeyCode.S))
			this.transform.Translate(Vector3.forward * -this.moveSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.D))
			this.transform.Translate(Vector3.right * this.moveSpeed * Time.deltaTime);
		if (Input.GetKey(KeyCode.A))
			this.transform.Translate(Vector3.right * -this.moveSpeed * Time.deltaTime);

		if (Input.GetKey(KeyCode.E))
			this.transform.Translate(Vector3.up * this.moveSpeed * Time.deltaTime);
		if (Input.GetKey(KeyCode.Q))
			this.transform.Translate(Vector3.up * -this.moveSpeed * Time.deltaTime);
	}

	private void RotateCamera()
	{
		// Rotate camera horizontally
		float rotateHIncrement = Input.GetAxis("Mouse X") * this.rotateSpeed;
		this.transform.Rotate(0, rotateHIncrement, 0, Space.World);

		// Rotate camera vertically
		float rotateVIncrement = Input.GetAxis("Mouse Y");
		if (rotateVIncrement > 0 && this.rotateV < ROTATION_UPPER_LIMIT)
		{
			this.rotateV += this.rotateSpeed;
			this.transform.Rotate(-this.rotateSpeed, 0, 0, Space.Self);
		}
		else if (rotateVIncrement < 0 && this.rotateV > ROTATION_LOWER_LIMIT)
		{
			this.rotateV -= this.rotateSpeed;
			this.transform.Rotate(this.rotateSpeed, 0, 0, Space.Self);
		}
	}
}
