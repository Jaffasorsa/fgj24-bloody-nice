using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeInventoryItem", menuName = "UpgradeInventoryItem", order = 1), Serializable]
public class ScriptableUpgradeInventoryItem : ScriptableObject
{
	public string Name;
	public float Price;
	public int maxUpgradeLevel;
	public List<Sprite> Icon;
}
