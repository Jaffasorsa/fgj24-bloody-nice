using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeInventoryItem", menuName = "UpgradeInventoryItem", order = 1)]
public class UpgradeInventoryItem : ScriptableObject
{
	public string Name;
	public float Price;
	public Sprite Icon;
}
