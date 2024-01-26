using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanEmitter : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particleSystem;

	public void ShootBeans()
	{
		particleSystem.Play();
	}
}
