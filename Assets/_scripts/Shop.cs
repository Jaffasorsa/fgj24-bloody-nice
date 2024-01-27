using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	[SerializeField]
	protected List<ScriptableUpgradeInventoryItem> upgradeInventoryInEditor = new();

	protected Dictionary<string, ScriptableUpgradeInventoryItem> upgradeInventory = new();

	public static Dictionary<string, UpgradeInventoryItem> OwnedUpgrades = new();
	public static float Money = 20000f;

	[SerializeField]
	protected GameObject buttonPrefab;

	[SerializeField]
	protected Transform canvasRoot;

	[SerializeField]
	protected Dictionary<string, Button> visibleButtons = new();

	public void Awake()
	{
		ConvertUpgradeInventoryItemsFromListToDictionary();
	}

	public void OnEnable()
	{
		OpenTheShop();
	}

	public void OnDisable()
	{
		CloseTheShop();
	}

	protected void OpenTheShop()
	{
		LoadData();
		SetButtons();
	}

	protected void CloseTheShop()
	{
		SaveData();
		DestroyButtons();
	}

	/// <summary>
	/// Sets the buttons on the screen
	/// </summary>
	protected void SetButtons()
	{
		float currentScreenYPosition = Screen.height;

		foreach (ScriptableUpgradeInventoryItem item in upgradeInventory.Values)
		{
			// Instantiation
			GameObject buttonGameObject = Instantiate(buttonPrefab, canvasRoot);

			TextMeshProUGUI textMeshProUGUI = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>();
			RectTransform rectTransform = buttonGameObject.GetComponentInChildren<RectTransform>();
			Button button = buttonGameObject.GetComponentInChildren<Button>();

			// Add the button into the visible list
			visibleButtons[item.Name] = button;

			// Name the button
			textMeshProUGUI.text = $"{item.Name}\n{item.Price} ";

			// Set up the click event
			button.onClick.AddListener(delegate { PurchaseItem(item); });

			// Set the position, also updating variable for next button 
			currentScreenYPosition -= rectTransform.sizeDelta.y * 0.5f;
			buttonGameObject.transform.position = new Vector3(rectTransform.sizeDelta.x * 0.5f, currentScreenYPosition, 0);
			currentScreenYPosition -= rectTransform.sizeDelta.y * 0.5f;
		}

		CheckAndDisableButtonsIfNeeded();
	}

	/// <summary>
	/// Checks all the buttons to see if they need disabling
	/// </summary>
	protected void CheckAndDisableButtonsIfNeeded()
	{
		foreach (var upgradeInventoryItem in upgradeInventory.Values)
		{
			// Disable if already purchased or not enough cash
			if (Money < upgradeInventoryItem.Price || (OwnedUpgrades.ContainsKey(upgradeInventoryItem.Name) && OwnedUpgrades[upgradeInventoryItem.name]?.UpgradeLevel == upgradeInventoryItem.maxUpgradeLevel))
				visibleButtons[upgradeInventoryItem.name].interactable = false;
		}
	}

	/// <summary>
	/// Destroy all the buttons and event subscriptions
	/// </summary>
	protected void DestroyButtons()
	{
		foreach (Button button in visibleButtons.Values)
		{
			button.onClick.RemoveAllListeners();
			Destroy(button.gameObject);
		}

		visibleButtons.Clear();
	}

	/// <summary>
	/// Purchase upgrade
	/// </summary>
	/// <param name="upgradeInventoryItem"></param>
	protected void PurchaseItem(ScriptableUpgradeInventoryItem upgradeInventoryItem)
	{
		if (Money < upgradeInventoryItem.Price) return;

		Money -= upgradeInventoryItem.Price;

		// If the item is owned already, iterate the upgrade level
		if (OwnedUpgrades.ContainsKey(upgradeInventoryItem.Name))
		{
			OwnedUpgrades[upgradeInventoryItem.name].UpgradeLevel++;
		}
		// Otherwise just add to dictionary with level 1
		else
		{
			OwnedUpgrades[upgradeInventoryItem.name] = new UpgradeInventoryItem(upgradeInventoryItem.name, 1);
		}

		CheckAndDisableButtonsIfNeeded();
	}

	/// <summary>
	/// Saves the dictionary of owned upgrades to PlayerPrefs
	/// </summary>
	protected void SaveData()
	{
		string serializedListOfUpgrades = JsonConvert.SerializeObject(OwnedUpgrades);
		PlayerPrefs.SetString("upgrades", serializedListOfUpgrades);
		PlayerPrefs.SetString("money", Money.ToString());
	}

	/// <summary>
	/// Loads the dictionary of owned upgrades from PlayerPrefs
	/// </summary>
	protected void LoadData()
	{
		string upgradesFromPlayerPrefs = PlayerPrefs.GetString("upgrades", "");

		if (upgradesFromPlayerPrefs == "")
		{
			return;
		}

		try
		{
			OwnedUpgrades = JsonConvert.DeserializeObject<Dictionary<string, UpgradeInventoryItem>>(upgradesFromPlayerPrefs);
		}
		catch (Exception e)
		{
			Debug.Log($"Some issue with getting the data, creating new dictionary for owned items: {e}");
			OwnedUpgrades = new();
		}

		float.TryParse(PlayerPrefs.GetString("money", ""), out Money);
	}

	protected void ConvertUpgradeInventoryItemsFromListToDictionary()
	{
		foreach (var item in upgradeInventoryInEditor)
		{
			upgradeInventory[item.Name] = item;
		}
	}

	// DEVELOPMENT

	/// <summary>
	/// TODO: Remove me. I am just for development purposes
	/// </summary>
	protected void ShowPurchases()
	{
		string purchases = "";
		foreach (string key in OwnedUpgrades.Keys)
		{
			purchases += key + ",";
		}

		Debug.Log(purchases);
	}
}
