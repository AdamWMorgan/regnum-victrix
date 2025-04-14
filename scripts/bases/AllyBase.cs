using Godot;
using System;

public partial class AllyBase : Node2D
{
	public string BaseID;
	private Base allyBase;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		allyBase = new Base.Builder().SetOwner(Base.BaseOwner.ENEMY).Build();
		BaseID = GameManager.Instance.BaseRegister(allyBase);
		AddChild(allyBase);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
