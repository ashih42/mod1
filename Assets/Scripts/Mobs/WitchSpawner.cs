using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchSpawner : Spawner
{
	protected override void Spawn(Vector3 position)
	{
		Ray ray = new Ray(position, Vector3.down);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			if (hit.collider != null && (hit.collider.name == "Terrain" || hit.collider.name == "TerrainMesh"))
			{
				GameObject witchGO = ObjectPooler.Instance.SpawnFromPool("Witch");

				witchGO.transform.position = hit.point + new Vector3(0, 0, 0);
				witchGO.transform.SetParent(this.transform);
			}
		}
	}
}
