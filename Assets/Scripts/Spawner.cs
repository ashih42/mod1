using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
	[SerializeField] private KeyCode downKeyCode;
	[SerializeField] private KeyCode heldKeyCode;

	private Vector3 minPosition;
	private Vector3 maxPosition;

	protected void Start()
	{
		BoxCollider boxCollider = this.GetComponent<BoxCollider>();
		this.minPosition = boxCollider.bounds.min;
		this.maxPosition = boxCollider.bounds.max;
	}

	private void Update()
	{
		if (Input.GetKeyDown(this.downKeyCode) || Input.GetKey(this.heldKeyCode))
		{
			Vector3 position = new Vector3(
				Random.Range(this.minPosition.x, this.maxPosition.x),
				Random.Range(this.minPosition.y, this.maxPosition.y),
				Random.Range(this.minPosition.z, this.maxPosition.z));

			this.Spawn(position);
		}
	}

	protected abstract void Spawn(Vector3 position);
}
