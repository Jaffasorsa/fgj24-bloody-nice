/// <summary>
/// Data class object for an upgrade item
/// </summary>
public class UpgradeInventoryItem
{
	public string Name;
	public int UpgradeLevel;
	public UpgradeInventoryItem(string name, int upgradeLevel)
	{
		Name = name;
		UpgradeLevel = upgradeLevel;
	}
}
