using Godot;
using System;

public partial class EnemyBase : Node2D
{
	public string BaseID;
	private Base enemyBase;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		enemyBase = new Base.Builder().SetOwner(Base.BaseOwner.ENEMY).Build();
		BaseID = GameManager.Instance.BaseRegister(enemyBase);
		AddChild(enemyBase);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{}
}
