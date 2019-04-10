using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainVoxel : MonoBehaviour
{
	private bool isGrounded;
	private bool isAtBottom;
	private bool hasJoinedVoxelMass;

	private float terrainHeight;

	private void OnEnable()
	{
		this.isGrounded = false;
		this.isAtBottom = false;
		this.hasJoinedVoxelMass = false;
	}

	private void FixedUpdate()
	{
		if (!this.isGrounded)
			this.Fall();
		else if (!this.isAtBottom)
			this.RollDown();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "WaterVoxel" && !this.hasJoinedVoxelMass)
		{
			this.hasJoinedVoxelMass = true;
			other.GetComponent<WaterVoxel>().Mass.Grow();
			AudioManager.Instance.PlaySFX("Splash");
			ObjectPooler.Instance.Reclaim(this.gameObject);
		}
	}

	public void CheckTerrainHeight()
	{
		this.terrainHeight = GameManager.Instance.GetTerrainHeight(this.transform.position);
	}

	private void Fall()
	{
		this.transform.Translate(Vector3.down * WaterVoxelManager.Instance.VoxelSize);

		if (this.transform.position.y <= this.terrainHeight)
		{
			this.transform.position = new Vector3(this.transform.position.x, this.terrainHeight, this.transform.position.z);
			this.isGrounded = true;
			AudioManager.Instance.PlaySFX("Raindrop");
		}
	}

	private void RollDown()
	{
		Vector3 lowestPos = this.GetLowestPosition();

		if (lowestPos.y == this.transform.position.y)
		{
			this.isAtBottom = true;
			Invoke("BecomeWaterVoxelMass", 0.1f);
		}
		else
			this.transform.position = lowestPos;
	}

	private void BecomeWaterVoxelMass()
	{
		WaterVoxelMass mass = WaterVoxelManager.Instance.SpawnWaterVoxelMass();
		WaterVoxel waterVoxel = WaterVoxelManager.Instance.SpawnWaterVoxel(this.transform.position);

		mass.AddVoxel(waterVoxel);
		ObjectPooler.Instance.Reclaim(this.gameObject);
	}

	private Vector3 GetLowestPosition()
	{
		Vector3 lowestPos = this.transform.position;

		int x = (int)this.transform.position.x;
		int z = (int)this.transform.position.z;

		float tempHeight;

		if ((tempHeight = this.GetHeight(x + WaterVoxelManager.Instance.VoxelSize, z)) != float.NaN && tempHeight < lowestPos.y)
		{
			lowestPos = new Vector3(x + WaterVoxelManager.Instance.VoxelSize, tempHeight, z);
		}
		if ((tempHeight = this.GetHeight(x - WaterVoxelManager.Instance.VoxelSize, z)) != float.NaN && tempHeight < lowestPos.y)
		{
			lowestPos = new Vector3(x - WaterVoxelManager.Instance.VoxelSize, tempHeight, z);
		}
		if ((tempHeight = this.GetHeight(x, z + WaterVoxelManager.Instance.VoxelSize)) != float.NaN && tempHeight < lowestPos.y)
		{
			lowestPos = new Vector3(x, tempHeight, z + WaterVoxelManager.Instance.VoxelSize);
		}
		if ((tempHeight = this.GetHeight(x, z - WaterVoxelManager.Instance.VoxelSize)) != float.NaN && tempHeight < lowestPos.y)
		{
			lowestPos = new Vector3(x, tempHeight, z - WaterVoxelManager.Instance.VoxelSize);
		}
		return lowestPos;
	}

	private float GetHeight(int x, int z)
	{
		if (0 <= x && x <= 256 && 0 <= z && z <= 256)
			//return Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
			return GameManager.Instance.GetTerrainHeight(new Vector3(x, 0, z));
		else
			return float.NaN;
	}
}
