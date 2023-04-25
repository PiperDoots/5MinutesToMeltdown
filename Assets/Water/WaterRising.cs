using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRising : MonoBehaviour
{
	private Transform objectToScale;
	[SerializeField] private float maxYScale;
	private float initialYScale;

	void Start()
	{
		objectToScale = transform;

		initialYScale = objectToScale.localScale.y;
	}

	void Update()
	{
		float timeLeft = WaterTimer.Instance.time;

		if (timeLeft <= 0f)
		{
			objectToScale.localScale = new Vector3(
				objectToScale.localScale.x,
				maxYScale,
				objectToScale.localScale.z);
		}
		else
		{
			float scaleAmount = Mathf.Clamp01(1f - timeLeft / WaterTimer.Instance.maxTime);
			float newYScale = initialYScale + (maxYScale - initialYScale) * scaleAmount;
			objectToScale.localScale = new Vector3(
				objectToScale.localScale.x,
				newYScale,
				objectToScale.localScale.z);
		}
	}
}
