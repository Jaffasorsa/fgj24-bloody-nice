using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	[SerializeField]
	protected List<ScriptableUpgradeInventoryItem> allUpgradeInventoryItems;

	public static Dictionary<string, UpgradeInventoryItem> OwnedUpgradeInventoryItems;

	[SerializeField]
	protected GameObject buttonPrefab;

	[SerializeField]
	protected Transform canvasRoot;

	public void Start()
	{
		LoadData();
		SetButtons();
	}

	/// <summary>
	/// Sets the buttons on the screen
	/// </summary>
	public void SetButtons()
	{
		float currentScreenYPosition = Screen.height;

		foreach (ScriptableUpgradeInventoryItem item in allUpgradeInventoryItems)
		{
			// Instantiation
			GameObject buttonGameObject = Instantiate(buttonPrefab, canvasRoot);

			TextMeshProUGUI textMeshProUGUI = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>();
			RectTransform rectTransform = buttonGameObject.GetComponentInChildren<RectTransform>();
			Button button = buttonGameObject.GetComponentInChildren<Button>();

			// Name the button
			textMeshProUGUI.text = item.Name;

			// Set up the click event
			button.onClick.AddListener(delegate { PurchaseItem(item); });

			// Set the position, also updating variable for next button 
			currentScreenYPosition -= rectTransform.sizeDelta.y * 0.5f;
			buttonGameObject.transform.position = new Vector3(rectTransform.sizeDelta.x * 0.5f, currentScreenYPosition, 0);
			currentScreenYPosition -= rectTransform.sizeDelta.y * 0.5f;
		}
	}

	/// <summary>
	/// Purchase upgrade
	/// </summary>
	/// <param name="upgradeInventoryItem"></param>
	public void PurchaseItem(ScriptableUpgradeInventoryItem upgradeInventoryItem)
	{
		OwnedUpgradeInventoryItems[upgradeInventoryItem.name] = new UpgradeInventoryItem(upgradeInventoryItem.name, 0);
	}

	/// <summary>
	/// TODO: Remove me. I am just for development purposes
	/// </summary>
	public void ShowPurchases()
	{
		string purchases = "";
		foreach ((string s, UpgradeInventoryItem u) in OwnedUpgradeInventoryItems)
		{
			purchases += u.Name + ",";
		}

		Debug.Log(purchases);
	}

	/// <summary>
	/// Saves the dictionary of owned upgrades to PlayerPrefs
	/// </summary>
	protected void SaveData()
	{
		string serializedListOfUpgrades = JsonConvert.SerializeObject(OwnedUpgradeInventoryItems);
		PlayerPrefs.SetString("upgrades", serializedListOfUpgrades);
	}

	/// <summary>
	/// Loads the dictionary of owned upgrades from PlayerPrefs
	/// </summary>
	protected void LoadData()
	{
		string upgradesFromPlayerPrefs = PlayerPrefs.GetString("upgrades", "");

		if (upgradesFromPlayerPrefs == "")
		{
			OwnedUpgradeInventoryItems = new();
			return;
		}

		try
		{
			OwnedUpgradeInventoryItems = JsonConvert.DeserializeObject<Dictionary<string, UpgradeInventoryItem>>(upgradesFromPlayerPrefs);
		}
		catch (Exception e)
		{
			Debug.Log($"Some issue with getting the data, creating new dictionary for owned items: {e}");
			OwnedUpgradeInventoryItems = new();
		}
	}
}
