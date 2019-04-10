using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
	#region Singleton

	public static GameManager Instance { get; private set; }

	private void Awake()
	{
		GameManager.Instance = this;
	}

	#endregion

	[SerializeField] private InputField terrainFileInputField;
	[SerializeField] private TextMeshProUGUI consoleText;
	[SerializeField] private Toggle dogeToggle;

	[SerializeField] private Material waterMaterial;
	[SerializeField] private Material dogeMaterial;

	[SerializeField] private TerrainManager terrainManager;
	[SerializeField] private TerrainMesh terrainMesh;

	public bool CanSpawnVoxels
	{
		get { return this.terrainManager.isActiveAndEnabled; }
	}

	public Material WaterMaterial
	{
		get { return this.dogeToggle.isOn ? this.dogeMaterial : this.waterMaterial; }
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
			this.Reset();
	}

	public void FlipDogeToggle()
	{
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("WaterVoxel"))
			go.GetComponent<MeshRenderer>().material = this.WaterMaterial;

		foreach (GameObject go in GameObject.FindGameObjectsWithTag("WaterParticle"))
			go.GetComponent<MeshRenderer>().material = this.WaterMaterial;
	}

	public float GetTerrainHeight(Vector3 position)
	{
		return Terrain.activeTerrain.SampleHeight(position);
	}

	public void LoadTerrainFile()
	{
		try
		{
			this.consoleText.text = "Loading terrain from <color=blue>" + terrainFileInputField.text + "</color>...\n";
			this.terrainManager.GenerateTerrainFromFile(terrainFileInputField.text);
			this.consoleText.text += "<color=green>" + "Success!" + "</color>";

			this.terrainMesh.gameObject.SetActive(false);
			this.terrainManager.gameObject.SetActive(true);
			this.Reset();
		}
		catch (ParserException exception)
		{
			this.consoleText.text += exception.Message;
		}
		catch (FileNotFoundException)
		{
			this.consoleText.text += "Could not open file: <color=red>" + terrainFileInputField.text + "</color>";
		}
		catch (UnauthorizedAccessException)
		{
			this.consoleText.text += "<color=red>" + "u w0t m8?" + "</color>";
		}
	}

	public void GenerateTerrainMesh()
	{
		this.terrainManager.gameObject.SetActive(false);
		this.terrainMesh.gameObject.SetActive(true);
		this.Reset();

		this.consoleText.text = "Generating random terrain...\n";
		this.terrainMesh.GenerateRandomTerrain();
		this.consoleText.text += "<color=green>" + "Success!" + "</color>";
	}

	public void Reset()
	{
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("RainVoxel"))
			ObjectPooler.Instance.Reclaim(go);

		foreach (GameObject go in GameObject.FindGameObjectsWithTag("WaterVoxel"))
			ObjectPooler.Instance.Reclaim(go);
		
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("WaterVoxelMass"))
			ObjectPooler.Instance.Reclaim(go);

		foreach (GameObject go in GameObject.FindGameObjectsWithTag("WaterParticle"))
			ObjectPooler.Instance.Reclaim(go);
			
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Witch"))
			ObjectPooler.Instance.Reclaim(go);

		WaterVoxelManager.Instance.ResetWaterHeight();
	}

}
