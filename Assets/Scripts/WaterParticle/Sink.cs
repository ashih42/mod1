using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "WaterParticle")
			ObjectPooler.Instance.Reclaim(other.gameObject);
	}
}
