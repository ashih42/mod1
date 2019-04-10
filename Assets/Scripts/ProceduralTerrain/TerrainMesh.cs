using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainMesh : MonoBehaviour
{
	[SerializeField] private MapGenerator mapGenerator;

	private MeshFilter meshFilter;
	private MeshCollider meshCollider;
	private NavMeshSurface navMeshSurface;
	
	private void Awake()
	{
		this.meshFilter = this.GetComponent<MeshFilter>();
		this.meshCollider = this.GetComponent<MeshCollider>();
		this.navMeshSurface = this.GetComponent<NavMeshSurface>();
	}

	public void GenerateRandomTerrain()
	{
		this.mapGenerator.seed = UnityEngine.Random.Range(0, int.MaxValue);
		this.mapGenerator.offset = UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(0f, 42000f);
		this.mapGenerator.GenerateMap();

		this.meshCollider.sharedMesh = this.meshFilter.sharedMesh;
		this.navMeshSurface.BuildNavMesh();
	}

}
