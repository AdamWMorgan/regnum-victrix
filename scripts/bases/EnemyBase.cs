using Godot;

public partial class EnemyBase : Base, IBaseProvider
{
	public override void _Ready()
	{
		base._Ready(); // Call shared logic from Base

		var style = new StyleBoxFlat
		{
			BgColor = ColourPalette.ENEMY.ToColor()
		};
		captureProgress.ColourChange(style);

		CurrentBaseOwner = Faction.ENEMY;
		GameManager.Instance.BaseRegister(this); // Optional: Only if needed again
	}

	public Base GetAttachedBase()
	{
		return this;
	}
}
