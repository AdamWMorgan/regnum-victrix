using Godot;

public partial class AllyBase : Node2D
{
	[Export] public CaptureProgress captureProgress;
	public string BaseID;
	private Base allyBase;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var style = new StyleBoxFlat
		{
			BgColor = ColourPalette.ALLY.ToColor() 
		};
		captureProgress.ColourChange(style);
		allyBase = new Base.Builder().SetOwner(Base.BaseOwner.ALLY).Build();
		BaseID = GameManager.Instance.BaseRegister(allyBase);
		AddChild(allyBase);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
