using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanEmitter : MonoBehaviour
{
	[SerializeField]
	private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	public void ShootBeans()
	{
		foreach (ParticleSystem particleSystem in particleSystems)
		{
			if (particleSystem.gameObject.activeSelf)
			{
				particleSystem.Play();
			}
		}
	}
}
