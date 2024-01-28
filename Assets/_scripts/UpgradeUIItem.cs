using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI ButtonText, MainText;

	public Button Button;

	public void SetMainText(string text)
	{
		MainText.text = text;
	}

	public void SetButtonText(string text)
	{
		ButtonText.text = text;
	}

	public void DestroyMe()
	{
		Button.onClick.RemoveAllListeners();
		Destroy(this.gameObject);
	}
}
