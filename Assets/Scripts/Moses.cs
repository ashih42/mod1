using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moses : MonoBehaviour
{
	[SerializeField] private BoxCollider boxCollider;
	[SerializeField] private BoxCollider triggerBoxCollider;

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "RainVoxel")
			ObjectPooler.Instance.Reclaim(other.gameObject);

		if (other.tag == "WaterVoxel")
			other.GetComponent<WaterVoxel>().DestroySelf();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.M))
			this.Expand();
		if (Input.GetKeyUp(KeyCode.M))
			this.Reset();
	}

	private void Expand()
	{
		Vector3 scale = this.transform.localScale;

		scale.z += 1f;
		scale.z = Mathf.Clamp(scale.z, 0f, 100f);
		this.transform.localScale = scale;

		this.boxCollider.enabled = true;
		this.triggerBoxCollider.enabled = true;
	}

	private void Reset()
	{
		Vector3 scale = this.transform.localScale;

		scale.z = 0f;
		this.transform.localScale = scale;

		this.boxCollider.enabled = false;
		this.triggerBoxCollider.enabled = false;
	}

}
