using Godot;

public partial class AllyBase : Base, IBaseProvider
{
	[Export] public CaptureProgress captureProgress;

	public override void _Ready()
	{
		base._Ready(); // Initialize shared base logic

		var style = new StyleBoxFlat
		{
			BgColor = ColourPalette.ALLY.ToColor()
		};
		captureProgress.ColourChange(style);

		CurrentBaseOwner = Faction.ALLY;
		GameManager.Instance.BaseRegister(this); // Optional: only needed if not already registered in base
	}

	public Base GetAttachedBase()
	{
		return this;
	}
}
