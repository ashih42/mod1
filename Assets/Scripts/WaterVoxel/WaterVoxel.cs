using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVoxel : MonoBehaviour
{
	private MeshRenderer meshRenderer;

	public WaterVoxelMass Mass { get; set; }

	private void Awake()
	{
		this.meshRenderer = this.GetComponent<MeshRenderer>();
	}

	private void OnEnable()
	{
		this.Mass = null;
		this.meshRenderer.material = GameManager.Instance.WaterMaterial;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "WaterVoxel")
		{
			WaterVoxelMass otherMass = other.GetComponent<WaterVoxel>().Mass;

			if (this.Mass.ID > otherMass.ID)
				this.Mass.AbsorbMass(otherMass);
		}
	}

	// Called by Moses only
	public void DestroySelf()
	{
		int x = (int)this.transform.position.x;
		int z = (int)this.transform.position.z;

		WaterVoxelManager.Instance.WaterHeight[x, z] = -1f;		// Damn it Moses!

		this.Mass.RemoveVoxel(this);
		ObjectPooler.Instance.Reclaim(this.gameObject);
	}

}
