using Godot;
using System;

public partial class AllyBase : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameManager.Instance.BaseRegister(new Base.Builder().SetOwner(Base.BaseOwner.ALLY).Build());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
