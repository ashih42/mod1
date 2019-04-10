using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterParticleManager : MonoBehaviour
{
	#region Singleton

	public static WaterParticleManager Instance { get; private set; }

	private void Awake()
	{
		WaterParticleManager.Instance = this;
	}

	#endregion

	[SerializeField] private Slider particleSizeSlider;

	public WaterParticle SpawnWaterParticle(Vector3 position)
	{
		GameObject waterParticleGO = ObjectPooler.Instance.SpawnFromPool("WaterParticle");

		waterParticleGO.transform.SetParent(this.transform);
		waterParticleGO.transform.position = position;
		waterParticleGO.transform.localScale = Vector3.one * this.particleSizeSlider.value;
		return waterParticleGO.GetComponent<WaterParticle>();
	}
}
