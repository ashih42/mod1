using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : Spawner
{
	protected override void Spawn(Vector3 position)
	{
		for (int i = 0; i < 10; i++)
		{
			WaterParticleManager.Instance.SpawnWaterParticle(position);
		}
	}
}
