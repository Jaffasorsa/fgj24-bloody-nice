using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	enum Panel { Eating, Aiming, Launch, Score, None }

	[Header("General")]
	[SerializeField]
	private GameObject beanEaterPrefab;

	[SerializeField]
	private Transform beanEaterSpawn;

	[SerializeField]
	private CameraController cameraController;

	[SerializeField]
	private TextMeshProUGUI statDisplayText;

	[SerializeField]
	private GameObject shop;

	[Header("Eating")]
	[SerializeField]
	private GameObject eatingPanel;

	[SerializeField]
	private GameObject foodBowl;

	[SerializeField]
	private Button eatButton;

	[SerializeField]
	private RectTransform eatingTimerBar;

	[SerializeField]
	private float eatingTimeLimit = 5f;

	[Header("Aiming")]
	[SerializeField]
	private GameObject aimingPanel;

	[SerializeField]
	private Button aimButton;

	[SerializeField]
	private GameObject launchPad;

	[SerializeField]
	private float launchPadMaxAngle = 45f;

	[Header("Launch")]
	[SerializeField]
	private GameObject launchPanel;

	[SerializeField]
	private Button launchButton;

	[SerializeField]
	private RectTransform powerBar;

	[Header("Score")]
	[SerializeField]
	private GameObject scorePanel;

	[SerializeField]
	private TextMeshProUGUI scoreDisplayText;

	private BeanEater beanEater;

	public static GameController Instance { get; private set; }

	private void Awake()
	{
		if (Instance != this && Instance != null)
		{
			Debug.LogError("You can't have more than one GameController.");
			Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		Play();
	}

	public void Play()
	{
		Init();
		Eating();
	}

	private void Init()
	{
		launchPad.transform.rotation = Quaternion.Euler(Vector3.zero);

		if (beanEater) Destroy(beanEater.gameObject);
		beanEater = Instantiate(beanEaterPrefab, launchPad.transform).GetComponent<BeanEater>();
		beanEater.transform.position = beanEaterSpawn.position;
		beanEater.transform.rotation = Quaternion.Euler(Vector3.zero);
		cameraController.target = beanEater.transform;
		launchPad.transform.localRotation = Quaternion.Euler(Vector3.zero);
	}

	public void EatBeans(bool value) => beanEater.EatBeans(value);

	private void Eating()
	{
		ChangePanel(Panel.Eating);

		eatButton.onClick.AddListener(StartEatingTimer);

		foodBowl.SetActive(true);
	}

	private void StartEatingTimer()
	{
		eatButton.onClick.RemoveListener(StartEatingTimer);

		StartCoroutine(EatingTimer());
	}

	private IEnumerator EatingTimer()
	{
		Vector2 startLength = eatingTimerBar.sizeDelta;

		float timer = eatingTimeLimit;
		while (timer > 0f)
		{
			timer -= Time.deltaTime;

			eatingTimerBar.sizeDelta = new Vector2(startLength.x * (timer / eatingTimeLimit), startLength.y);

			yield return null;
		}

		eatingTimerBar.sizeDelta = new Vector2(startLength.x, startLength.y);
		foodBowl.SetActive(false);

		Aiming();
	}

	private void Aiming()
	{
		ChangePanel(Panel.Aiming);

		beanEater.EatBeans(false);
		StartMovingLaunchPad();
	}

	private void StartMovingLaunchPad()
	{
		aimButton.onClick.AddListener(() => { lockedIn = true; });

		StartCoroutine(MoveLaunchPad());
	}

	private bool lockedIn = false;

	private IEnumerator MoveLaunchPad()
	{
		float timer = 0f;
		lockedIn = false;
		while (!lockedIn)
		{
			timer += Time.deltaTime;

			float angle = Mathf.PingPong(timer * 50, launchPadMaxAngle);
			launchPad.transform.localRotation = Quaternion.Euler(Vector3.back * angle);

			yield return null;
		}

		Launch();
	}

	private void Launch()
	{
		ChangePanel(Panel.Launch);

		StartLaunchPowerUp();
	}

	private void StartLaunchPowerUp()
	{
		launchButton.onClick.AddListener(() => { launched = true; });
		StartCoroutine(LaunchPowerUp());
	}

	private bool launched = false;

	private IEnumerator LaunchPowerUp()
	{
		Vector2 startLength = powerBar.sizeDelta;
		float timer = 0f;
		float power = 0f;
		launched = false;
		while (!launched)
		{
			timer += Time.deltaTime;

			power = Mathf.PingPong(timer, 1f);

			powerBar.sizeDelta = new Vector2(startLength.x * power, startLength.y);

			yield return null;
		}

		powerBar.sizeDelta = new Vector2(startLength.x, startLength.y);
		launchButton.onClick.RemoveAllListeners();
		Launched(power);
	}

	private void Launched(float power)
	{
		beanEater.Launch(power);
		ChangePanel(Panel.Score);
	}

	private void ChangePanel(Panel panel)
	{
		eatingPanel.SetActive(panel == Panel.Eating);
		aimingPanel.SetActive(panel == Panel.Aiming);
		launchPanel.SetActive(panel == Panel.Launch);
		scorePanel.SetActive(panel == Panel.Score);
	}

	public void UpdateStatDisplay(string text)
	{
		statDisplayText.text = text;
	}

	public void UpdateScoreDisplay(string text)
	{
		scoreDisplayText.text = text;
	}

	public void ActivateShop()
	{
		shop.SetActive(true);
	}
}
