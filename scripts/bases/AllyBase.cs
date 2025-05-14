using Godot;

public partial class AllyBase : Node2D, IBaseProvider
{
	[Export] public CaptureProgress captureProgress;
	public string BaseID;
	private Base attachedBase;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var style = new StyleBoxFlat
		{
			BgColor = ColourPalette.ALLY.ToColor() 
		};
		captureProgress.ColourChange(style);
		attachedBase = new Base.Builder().SetOwner(Faction.ALLY).Build();
		BaseID = GameManager.Instance.BaseRegister(attachedBase);
		AddChild(attachedBase);
	}
	
	public Base GetAttachedBase(){
		return attachedBase;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
