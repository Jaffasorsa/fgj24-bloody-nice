using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeanEater : MonoBehaviour
{
	[System.Serializable]
	private class Upgrade
	{
		public string name;

		public GameObject item;

		public UnityEvent action;
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private Rigidbody2D rb;

	[SerializeField]
	private List<Upgrade> upgrades = new List<Upgrade> { };

	private float fuelPerServing = 10f;

	private float fuelPotency = 70f;

	private float fuel = 0f;
	private float Fuel { get => fuel; set { fuel = value; UpdateFuelDisplay(); } }

	private void Awake()
	{
		Init();
	}

	public void Init()
	{
		LoadUpgrades();
	}

	private void LoadUpgrades()
	{
		foreach (Upgrade upgrade in upgrades)
		{
			if (ContainsUpgrade(upgrade.name))
			{
				upgrade.item.SetActive(true);
				upgrade.action?.Invoke();
			}
		}
	}

	public bool ContainsUpgrade(string upgrade)
	{
		return Shop.OwnedUpgrades.ContainsKey(upgrade);
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
		StartCoroutine(Scoring());
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

	bool readyForScoring = false;

	private IEnumerator Scoring()
	{
		float startPosition = transform.position.x;

		yield return new WaitForSeconds(0.1f);

		readyForScoring = true;

		float distance = 0f;
		while (readyForScoring)
		{
			distance = transform.position.x - startPosition;
			string scoreText = $"Score: {(int)distance}";
			GameController.Instance.UpdateScoreDisplay(scoreText);

			yield return null;
		}

		Shop.Money += Mathf.Max(0, (int)distance);
		GameController.Instance.ActivateShop();
	}

	public void UpdateFuelDisplay()
	{
		GameController.Instance.UpdateStatDisplay($"Fuel: {(int)Fuel}");
	}

	// Upgrade actions
	public void UpgradeAfterburner()
	{
		fuelPotency += 50f;
	}

	public void UpgradeFood()
	{
		fuelPerServing += 5f;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (readyForScoring)
		{
			readyForScoring = false;
		}
	}
}
