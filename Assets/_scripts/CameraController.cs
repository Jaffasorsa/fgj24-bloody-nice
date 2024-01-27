using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField]
	private Transform target;

	private void LateUpdate()
	{
		UpdatePosition();
	}

	private void UpdatePosition()
	{
		transform.position = target.position + Vector3.back * 10;
	}
}
