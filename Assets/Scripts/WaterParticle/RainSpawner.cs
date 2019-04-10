using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSpawner : Spawner
{
	protected override void Spawn(Vector3 position)
	{
		WaterParticle waterParticle = WaterParticleManager.Instance.SpawnWaterParticle(position);

		waterParticle.RB.drag = 10f;
	}
}
