using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
	// Assign in editor

	[SerializeField]
	protected GameObject UpgradeUIItemPrefab;

	[SerializeField]
	protected Transform upgradesRoot;

	[SerializeField]
	protected TextMeshProUGUI moneyDisplay;

	[SerializeField]
	protected TextMeshProUGUI mainDisplayText;

	[SerializeField]
	protected List<ScriptableUpgradeInventoryItem> upgradeInventoryInEditor = new();

	// Private/Protected

	protected Dictionary<string, ScriptableUpgradeInventoryItem> upgradeInventory = new();
	protected Dictionary<string, UpgradeUIItem> visibleUIItems = new();

	// Public

	public static Dictionary<string, UpgradeInventoryItem> OwnedUpgrades = new();
	public static float Money = 0f;

	public static Shop Instance { get; private set; }

	public void Awake()
	{
		if (Instance != this && Instance != null)
		{
			Debug.LogError("You can't have more than one Shop.");
			Destroy(this);
		}
		else
		{
			Instance = this;
		}

		ConvertUpgradeInventoryItemsFromListToDictionary();
	}

	public void OnEnable()
	{
		OpenTheShop();
	}

	private void Start()
	{
		CloseTheShop();
	}

	protected void OpenTheShop()
	{
		LoadData();
		SetUpgradesAndMoney();
	}

	public void CloseTheShop()
	{
		SaveData();
		DestroyUpgrades();
		GameController.Instance.Play();
		transform.parent.transform.gameObject.SetActive(false);
	}

	/// <summary>
	/// Sets the buttons on the screen
	/// </summary>
	protected void SetUpgradesAndMoney()
	{
		DestroyUpgrades();

		foreach (ScriptableUpgradeInventoryItem scriptableUpgradeInventoryItem in upgradeInventory.Values)
		{
			CreateUpgradeUIItemIfRequired(scriptableUpgradeInventoryItem);
		}

		moneyDisplay.text = $"Money: {Money}";

		if (visibleUIItems.Count == 0)
		{
			mainDisplayText.text = "No upgrades to purchase";
		}
	}

	/// <summary>
	/// Checks all the buttons to see if they need disabling
	/// </summary>
	protected void CreateUpgradeUIItemIfRequired(ScriptableUpgradeInventoryItem scriptableUpgradeInventoryItem)
	{
		// Don't show if already owned
		if (OwnedUpgrades.ContainsKey(scriptableUpgradeInventoryItem.Name) &&
		OwnedUpgrades[scriptableUpgradeInventoryItem.name]?.UpgradeLevel >= scriptableUpgradeInventoryItem.maxUpgradeLevel)
			return;

		// CREATE BUTTON

		// Instantiation
		GameObject upgradeUiItemGameObject = Instantiate(UpgradeUIItemPrefab, upgradesRoot);
		UpgradeUIItem upgradeUIItem = upgradeUiItemGameObject.GetComponentInChildren<UpgradeUIItem>();

		// Add the UI item into the visible list
		visibleUIItems[scriptableUpgradeInventoryItem.Name] = upgradeUIItem;

		// Add the text
		upgradeUIItem.SetMainText(scriptableUpgradeInventoryItem.Name);
		upgradeUIItem.SetButtonText($"Buy me ({scriptableUpgradeInventoryItem.Price})");

		// Set the icon - TODO: Maybe later

		// Set up the click event
		upgradeUIItem.Button.onClick.AddListener(delegate { PurchaseItem(scriptableUpgradeInventoryItem); });

		// Disable button if not enough cash
		if (Money < scriptableUpgradeInventoryItem.Price)
			upgradeUIItem.Button.interactable = false;
	}

	/// <summary>
	/// Destroy all the buttons and event subscriptions
	/// </summary>
	protected void DestroyUpgrades()
	{
		foreach (UpgradeUIItem upgradeUIItem in visibleUIItems.Values)
		{
			upgradeUIItem.DestroyMe();
		}

		visibleUIItems.Clear();
	}

	/// <summary>
	/// Purchase upgrade
	/// </summary>
	/// <param name="upgradeInventoryItem"></param>
	protected void PurchaseItem(ScriptableUpgradeInventoryItem upgradeInventoryItem)
	{
		if (Money < upgradeInventoryItem.Price ||
		(OwnedUpgrades.ContainsKey(upgradeInventoryItem.Name) && OwnedUpgrades[upgradeInventoryItem.name]?.UpgradeLevel >= upgradeInventoryItem.maxUpgradeLevel)) return;

		Money -= upgradeInventoryItem.Price;

		// If the item is owned already, iterate the upgrade level if not already at max
		if (OwnedUpgrades.ContainsKey(upgradeInventoryItem.Name))
		{
			OwnedUpgrades[upgradeInventoryItem.name].UpgradeLevel++;
		}
		// Otherwise just add to dictionary with level 1
		else
		{
			OwnedUpgrades[upgradeInventoryItem.name] = new UpgradeInventoryItem(upgradeInventoryItem.name, 1);
		}

		SetUpgradesAndMoney();
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

	public void ResetShop()
	{
		Money = 0;
		OwnedUpgrades.Clear();
		SetUpgradesAndMoney();
		SaveData();
	}
}
