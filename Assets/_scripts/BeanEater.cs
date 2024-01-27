using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanEater : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Rigidbody2D rb;

	private float fuel = 0f;

	private float fuelPerServing = 10f;

	private float fuelPotency = 100f;

	public void Init()
	{
		fuel = 0f;
		// Load values
	}

	public void EatBeans(bool value)
	{
		fuel += fuelPerServing;
		animator.SetBool("Eating", value);
	}

	public void Launch(float power)
	{

	}
}
