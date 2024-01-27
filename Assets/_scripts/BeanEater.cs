using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanEater : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Rigidbody2D rb;

	private float fuelPerServing = 10f;

	private float fuelPotency = 100f;

	private float fuel = 0f;
	private float Fuel { get => fuel; set { fuel = value; UpdateFuelDisplay(); } }

	public void Init()
	{
		Fuel = 0f;
		// Load values
	}

	public void EatBeans(bool value)
	{
		Fuel += fuelPerServing;
		animator.SetBool("Eating", value);
	}

	public void Launch(float power)
	{
		if (rocketLaunched) return;

		transform.parent = null;
		rb.bodyType = RigidbodyType2D.Dynamic;

		StartCoroutine(Rocket(power));
	}

	bool rocketLaunched = false;

	private IEnumerator Rocket(float power)
	{
		rocketLaunched = true;
		while (Fuel > 0f)
		{
			Fuel -= Time.deltaTime * 100f;

			rb.AddRelativeForce(Vector2.up * fuelPotency * power * 10f * Time.deltaTime, ForceMode2D.Force);

			yield return null;
		}

		Fuel = 0f;
	}

	public void UpdateFuelDisplay()
	{
		GameController.Instance.UpdateStatDisplay($"Fuel: {(int)Fuel}");
	}
}
