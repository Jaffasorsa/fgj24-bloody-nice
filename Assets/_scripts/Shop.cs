using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
	[SerializeField]
	protected List<UpgradeInventoryItem> upgradeInventoryItems;

	[SerializeField]
	protected GameObject buttonPrefab;

	[SerializeField]
	protected Transform canvasRoot;

	public void Start()
	{
		SetButtons();
		ShowPurchases();
	}

	public void SetButtons()
	{
		float currentScreenYPosition = Screen.height;

		foreach (UpgradeInventoryItem item in upgradeInventoryItems)
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

	public void PurchaseItem(UpgradeInventoryItem upgradeInventoryItem)
	{
		Debug.Log(upgradeInventoryItem.name);

		List<string> currentUpgrades = JsonUtility.FromJson<List<string>>(PlayerPrefs.GetString("upgrades", ""));

		currentUpgrades.Add(upgradeInventoryItem.name);

		//string reSerializedUpgrades = 
	}

	public void ShowPurchases()
	{
		if (PlayerPrefs.GetString("upgrades", "") == "")
		{
			Debug.Log("No upgrade items");
			return;
		}


		foreach (string upgradeItem in JsonUtility.FromJson<List<string>>(PlayerPrefs.GetString("upgrades", "")))
		{
			Debug.Log(upgradeItem);
		}
	}
}
