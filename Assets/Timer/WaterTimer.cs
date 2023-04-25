using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTimer : MonoBehaviour
{

	public float waterSpeed;
	public float maxTime;
	public float time;

	// Singleton design pattern, only 1 WaterTimer can exist at a time.
	public static WaterTimer Instance { get; private set; }
	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(this.gameObject);
			Debug.Log("WaterTimer already exists");
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		time = maxTime;
	}

	private void Update()
	{
		time -= Time.deltaTime * waterSpeed;
	}
}