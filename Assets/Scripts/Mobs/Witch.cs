using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Witch : MonoBehaviour
{
	private NavMeshAgent agent;
	private Animator animator;

	[SerializeField] private float wanderTime;
	[SerializeField] private float wanderRadius;
	[SerializeField] private Vector3 destination;
	[SerializeField] private float timer;

	private bool isRunning;
	private bool isDead;

	[SerializeField] private float distanceToGo;
	[SerializeField] private float remainingDistance;
	[SerializeField] private float stoppingDistance;

	private void Awake()
	{
		this.agent = this.GetComponent<NavMeshAgent>();
		this.animator = this.GetComponent<Animator>();
	}

	private void OnEnable()
	{
		this.agent.enabled = false;
		this.timer = this.wanderTime;
		this.destination = this.transform.position;
		this.isRunning = false;
		this.isDead = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "Terrain" || other.name == "TerrainMesh")
			this.agent.enabled = true;

		if (other.tag == "WaterParticle" || other.tag == "WaterVoxel" || other.tag == "RainVoxel")
			this.Die();
	}

	private void Die()
	{
		if (!this.isDead)
		{
			this.isDead = true;
			this.animator.SetTrigger("dieTrigger");
			AudioManager.Instance.PlaySFX("WitchDeath");
			Invoke("Disappear", 3f);
		}
	}

	private void Disappear()
	{
		if (this.gameObject.activeInHierarchy)
			ObjectPooler.Instance.Reclaim(this.gameObject);
	}

	private void Update()
	{
		if (this.isDead || !this.agent.enabled)
			return;

		this.timer += Time.deltaTime;

		// Cache and use my own values because I don't trust NavMeshAgent functions
		this.distanceToGo = Vector3.Distance(this.transform.position, this.destination);
		this.remainingDistance = this.agent.remainingDistance;
		this.stoppingDistance = this.agent.stoppingDistance;

		// Return to idle after reaching destination
		if (this.isRunning && this.distanceToGo <= this.agent.stoppingDistance)
		{
			this.animator.SetTrigger("idleTrigger");
			this.isRunning = false;
		}

		// Start running toward new destination
		if (this.timer > this.wanderTime)
		{
			this.destination = GetRandomNavPosition(this.transform.position, this.wanderRadius, -1);
			this.agent.SetDestination(this.destination);
			this.animator.SetTrigger("runTrigger");
			this.isRunning = true;
			this.timer = 0;
		}
	}

	private Vector3 GetRandomNavPosition(Vector3 origin, float distance, int layermask)
	{
		Vector3 randomDirection = origin + Random.insideUnitSphere * distance;
		NavMeshHit navHit;

		NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
		return navHit.position;
	}

}
