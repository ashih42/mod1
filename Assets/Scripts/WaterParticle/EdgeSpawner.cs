using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeSpawner : Spawner
{
	protected override void Spawn(Vector3 position)
	{
		WaterParticleManager.Instance.SpawnWaterParticle(position);
	}
}
