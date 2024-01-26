using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanEater : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	public void EatBeans(bool value)
	{
		animator.SetBool("Eating", value);
	}
}
