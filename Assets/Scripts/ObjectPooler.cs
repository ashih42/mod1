using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
	#region Singleton

	public static ObjectPooler Instance { get; private set; }

	private void Awake()
	{
		ObjectPooler.Instance = this;
	}

	#endregion

	[System.Serializable]
	private class Pool
	{
		public GameObject prefab;
		public int poolSize;
		public Queue<GameObject> queue;
	}

	[SerializeField] private Pool[] pools;
	private Dictionary<string, Pool> poolDictionary;

	private void Start()
	{
		this.poolDictionary = new Dictionary<string, Pool>();

		foreach(Pool pool in this.pools)
		{
			pool.queue = new Queue<GameObject>();
			this.PopulatePool(pool);
			this.poolDictionary.Add(pool.prefab.tag, pool);
		}
	}

	private void PopulatePool(Pool pool)
	{
		for (int i = 0; i < pool.poolSize; i++)
		{
			GameObject item = Instantiate(pool.prefab);
			item.SetActive(false);
			item.transform.SetParent(this.transform);
			pool.queue.Enqueue(item);
		}
	}

	public GameObject SpawnFromPool(string tag)
	{
		Pool pool = this.poolDictionary[tag];

		if (pool.queue.Count == 0)
			this.PopulatePool(pool);

		GameObject item = pool.queue.Dequeue();

		// Sometimes this fails even though the queue had just been populated ¯\_(ツ)_/¯
		if (item == null)
		{
			Debug.Log("SpawnFromPool() item is NULL, tag = " + tag + ", queue.Count = " + pool.queue.Count);
			item = Instantiate(this.poolDictionary[tag].prefab);
		}

		item.SetActive(true);
		item.transform.SetParent(null);
		return item;
	}

	public void Reclaim(GameObject go)
	{
		go.SetActive(false);
		go.transform.SetParent(this.transform);
		this.poolDictionary[go.tag].queue.Enqueue(go);
	}
}
