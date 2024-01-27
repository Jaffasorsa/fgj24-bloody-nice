using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
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

	public void Play()
	{

	}

	public void EatBeans()
	{

	}

	public void Aim()
	{

	}

	public void Launch()
	{

	}
}
