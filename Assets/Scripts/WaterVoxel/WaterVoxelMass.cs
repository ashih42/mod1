using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVoxelMass : MonoBehaviour
{
	private static int count = 0;

	public int ID { get; private set; }

	private List<WaterVoxel> activeVoxels;
	private List<WaterVoxel> allVoxels;

	private void Awake()
	{
		this.ID = ++WaterVoxelMass.count;
		this.activeVoxels = new List<WaterVoxel>();
		this.allVoxels = new List<WaterVoxel>();
	}

	private void OnEnable()
	{
		this.activeVoxels.Clear();
		this.allVoxels.Clear();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.KeypadPeriod))
			this.Grow();
	}

	public void AbsorbMass(WaterVoxelMass otherMass)
	{
		foreach (WaterVoxel voxel in otherMass.allVoxels)
		{
			this.allVoxels.Add(voxel);
			voxel.Mass = this;
			voxel.transform.parent = this.transform;
		}
		foreach (WaterVoxel voxel in otherMass.activeVoxels)
		{
			this.activeVoxels.Add(voxel);
		}
		otherMass.activeVoxels.Clear();
		otherMass.allVoxels.Clear();
		ObjectPooler.Instance.Reclaim(otherMass.gameObject);
	}

	public void AddVoxel(WaterVoxel voxel)
	{
		this.activeVoxels.Add(voxel);
		this.allVoxels.Add(voxel);
		voxel.Mass = this;
		voxel.transform.SetParent(this.transform);
	}

	public void RemoveVoxel(WaterVoxel voxel)
	{
		this.allVoxels.Remove(voxel);
		this.activeVoxels.Remove(voxel);
		voxel.Mass = null;
	}

	public void Grow()
	{
		List<WaterVoxel> newVoxels = new List<WaterVoxel>();

		foreach (WaterVoxel voxel in this.activeVoxels)
		{
			int count = 0;

			if (this.GrowToPosition((int)voxel.transform.position.x, (int)voxel.transform.position.z - WaterVoxelManager.Instance.VoxelSize, newVoxels, voxel))
				count++;
			if (this.GrowToPosition((int)voxel.transform.position.x, (int)voxel.transform.position.z + WaterVoxelManager.Instance.VoxelSize, newVoxels, voxel))
				count++;
			if (this.GrowToPosition((int)voxel.transform.position.x - WaterVoxelManager.Instance.VoxelSize, (int)voxel.transform.position.z, newVoxels, voxel))
				count++;
			if (this.GrowToPosition((int)voxel.transform.position.x + WaterVoxelManager.Instance.VoxelSize, (int)voxel.transform.position.z, newVoxels, voxel))
				count++;
		}

		if (newVoxels.Count == 0)
		{
			// Set activeVoxels to one new voxel above lowest position
			Vector3 lowestPosition = this.FindLowestPosition(this.activeVoxels);
			lowestPosition.y += WaterVoxelManager.Instance.VoxelSize;

			WaterVoxel newWaterVoxel = WaterVoxelManager.Instance.SpawnWaterVoxel(lowestPosition);
			this.activeVoxels.Clear();
			this.AddVoxel(newWaterVoxel);
		}
		else
		{
			this.activeVoxels.Clear();
			foreach (WaterVoxel voxel in newVoxels)
				this.AddVoxel(voxel);
		}
	}

	private Vector3 FindLowestPosition(List<WaterVoxel> voxels)
	{
		Vector3 lowestPos = Vector3.positiveInfinity;

		foreach (WaterVoxel voxel in voxels)
			if (voxel.transform.position.y < lowestPos.y)
				lowestPos = voxel.transform.position;

		return lowestPos;
	}

	private bool GrowToPosition(int x, int z, List<WaterVoxel> newVoxels, WaterVoxel voxel)
	{
		if (0 <= x && x <= 256 && 0 <= z && z <= 256)
		{
			float height = float.NaN;

			// no water here
			if (WaterVoxelManager.Instance.WaterHeight[x, z] < 0)
			{
				Vector3 terrainPos = new Vector3(x, 0, z);

				terrainPos.y = GameManager.Instance.GetTerrainHeight(terrainPos);

				if (terrainPos.y + WaterVoxelManager.Instance.VoxelSize < voxel.transform.position.y)
					height = terrainPos.y;
				else if (terrainPos.y <= voxel.transform.position.y)
					height = voxel.transform.position.y;
			}
			// there is VERY lower water here
			else if (WaterVoxelManager.Instance.WaterHeight[x, z] + WaterVoxelManager.Instance.VoxelSize < voxel.transform.position.y)
			{
				height = WaterVoxelManager.Instance.WaterHeight[x, z] + WaterVoxelManager.Instance.VoxelSize;
			}
			// there is low water here
			else if (WaterVoxelManager.Instance.WaterHeight[x, z] < voxel.transform.position.y)
			{
				height = voxel.transform.position.y;
			}

			if (!float.IsNaN(height))
			{
				WaterVoxel waterVoxel = WaterVoxelManager.Instance.SpawnWaterVoxel(new Vector3(x, height, z));

				WaterVoxelManager.Instance.WaterHeight[x, z] = height;
				newVoxels.Add(waterVoxel);

				return true;
			}
		}
		return false;
	}

}
