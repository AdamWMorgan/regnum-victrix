using Godot;

public partial class EnemyBase : Node2D
{
	[Export] public CaptureProgress captureProgress;
	public string BaseID;
	private Base enemyBase;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var style = new StyleBoxFlat
		{
			BgColor = ColourPalette.ENEMY.ToColor()
		};
		captureProgress.ColourChange(style);
		enemyBase = new Base.Builder().SetOwner(Faction.ENEMY).Build();
		BaseID = GameManager.Instance.BaseRegister(enemyBase);
		AddChild(enemyBase);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{ }
}
