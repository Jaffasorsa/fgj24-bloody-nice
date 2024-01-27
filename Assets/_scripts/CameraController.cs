using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform target;

	private void LateUpdate()
	{
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (target == null) return;

		transform.position = target.position + Vector3.back * 10;
	}
}
