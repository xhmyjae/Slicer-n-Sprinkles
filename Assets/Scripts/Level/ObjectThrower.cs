﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectThrower : MonoBehaviour
{
	public List<GameObject> objectsToThrow;
	public GameObject bombPrefab;
	public float randomThrowPositionXStart = -7;
	public float randomThrowPositionXEnd = 7;
	public float throwForceMin = 10.5f;
	public float throwForceMax = 15f;
	private readonly Difficulty _diff = Difficulty.selectedDifficulty;
	private int _currentObjectIndex;

	private void Start()
	{
		StartCoroutine(ThrowRandomObjects());
	}

	private IEnumerator ThrowRandomObjects()
	{
		while (true)
		{
			var randomQuantity = MathUtils.GenerateRandomNumber(1, _diff.throwMaxGroupQuantity, 0.15f);
			for (var i = 0; i < randomQuantity; i++) ThrowRandomObject();

			var randomDelay = MathUtils.GenerateRandomNumber(0.8f, _diff.throwMaxDelay, 0.8f);
			yield return new WaitForSeconds(randomDelay);
		}
	}

	private void ThrowRandomObject()
	{
		var centerPosition = transform.position;
		var prefab = Random.value < _diff.bombProbability ? bombPrefab : objectsToThrow[_currentObjectIndex];

		var throwPosition = new Vector3(Random.Range(randomThrowPositionXStart, randomThrowPositionXEnd), centerPosition.y, centerPosition.z);

		// objects thrown angle is affected by the distance from the center inversely
		var throwAngle = Random.Range(-_diff.randomAngleMax, _diff.randomAngleMax) *
						 (1 - Mathf.Abs(throwPosition.x - centerPosition.x) / (randomThrowPositionXEnd - randomThrowPositionXStart));

		var throwForce = Random.Range(throwForceMin, throwForceMax);
		var throwDirection = Quaternion.Euler(0, 0, throwAngle);

		var obj = Instantiate(prefab, throwPosition, throwDirection);
		obj.transform.localScale = new(_diff.launchedObjectsSize, _diff.launchedObjectsSize, _diff.launchedObjectsSize);

		var rb = obj.GetComponent<Rigidbody>();
		rb.useGravity = true;
		rb.AddForce(throwForce * rb.transform.up, ForceMode.Impulse);

		var randomTorque = new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));
		rb.AddTorque(randomTorque, ForceMode.Impulse);

		_currentObjectIndex = (_currentObjectIndex + 1) % objectsToThrow.Count;
	}
}
