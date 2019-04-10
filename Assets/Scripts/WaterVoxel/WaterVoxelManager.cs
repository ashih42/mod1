using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterVoxelManager : MonoBehaviour
{
	#region Singleton

	public static WaterVoxelManager Instance { get; private set; }

	private void Awake()
	{
		WaterVoxelManager.Instance = this;
	}

	#endregion

	private float[,] waterHeight = new float[256 + 1,256 + 1];

	public float[,] WaterHeight
	{
		get { return this.waterHeight; }
	}

	[SerializeField] private Slider voxelSizeSlider;

	public int VoxelSize
	{
		get { return (int)System.Math.Pow(2, this.voxelSizeSlider.value); }
	}

	private void Start()
	{
		this.ResetWaterHeight();
	}

	private void Update()
	{
		if (GameManager.Instance.CanSpawnVoxels)
		{
			if (Input.GetKeyDown(KeyCode.Keypad1))
				this.InitEdges();

			if (Input.GetKeyDown(KeyCode.Keypad2))
				this.InitWave();

			if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKey(KeyCode.Keypad6))
				this.SpawnRainVoxel();
		}
	}

	public void ResetWaterHeight()
	{
		for (int i = 0; i <= 256; i++)
			for (int j = 0; j <= 256; j++)
				this.waterHeight[i, j] = -1f;
	}

	private void InitWave()
	{
		GameManager.Instance.Reset();

		WaterVoxelMass mass = this.SpawnWaterVoxelMass();

		for (int i = 0; i <= 256; i += this.VoxelSize)
			mass.AddVoxel(this.SpawnWaterVoxel(new Vector3(i, 0, 0)));
	}

	private void InitEdges()
	{
		GameManager.Instance.Reset();

		WaterVoxelMass mass = this.SpawnWaterVoxelMass();

		for (int i = 0; i <= 256; i += this.VoxelSize)
		{
			mass.AddVoxel(this.SpawnWaterVoxel(new Vector3(i, 0, 0)));
			mass.AddVoxel(this.SpawnWaterVoxel(new Vector3(i, 0, 256)));
			mass.AddVoxel(this.SpawnWaterVoxel(new Vector3(0, 0, i)));
			mass.AddVoxel(this.SpawnWaterVoxel(new Vector3(256, 0, i)));
		}
	}

	public WaterVoxel SpawnWaterVoxel(Vector3 position)
	{
		int x = (int)position.x;
		int z = (int)position.z;

		this.waterHeight[x, z] = position.y;

		GameObject waterVoxelGO = ObjectPooler.Instance.SpawnFromPool("WaterVoxel");

		waterVoxelGO.transform.position = position;
		waterVoxelGO.transform.localScale = Vector3.one * this.VoxelSize;
		return waterVoxelGO.GetComponent<WaterVoxel>();
	}

	public RainVoxel SpawnRainVoxel()
	{
		GameObject rainVoxelGO = ObjectPooler.Instance.SpawnFromPool("RainVoxel");

		int x = Random.Range(0, 256) * this.VoxelSize % 256;
		int z = Random.Range(0, 256) * this.VoxelSize % 256;
		int y = Random.Range(150, 200);

		rainVoxelGO.transform.position = new Vector3(x, y, z);
		rainVoxelGO.transform.localScale = Vector3.one * this.VoxelSize;
		rainVoxelGO.transform.SetParent(this.transform);

		RainVoxel rainVoxel = rainVoxelGO.GetComponent<RainVoxel>();
		rainVoxel.CheckTerrainHeight();
		return rainVoxelGO.GetComponent<RainVoxel>();
	}

	public WaterVoxelMass SpawnWaterVoxelMass()
	{
		GameObject massGO = ObjectPooler.Instance.SpawnFromPool("WaterVoxelMass");

		return massGO.GetComponent<WaterVoxelMass>();
	}

}
