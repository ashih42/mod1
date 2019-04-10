using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterParticle : MonoBehaviour
{
	private MeshRenderer meshRenderer;

	public Rigidbody RB { get; private set; }

	private void Awake()
	{
		this.meshRenderer = this.GetComponent<MeshRenderer>();
		this.RB = this.GetComponent<Rigidbody>();
	}

	private void OnEnable()
	{
		this.meshRenderer.material = GameManager.Instance.WaterMaterial;
		this.RB.AddForce(Random.onUnitSphere);
	}

}
