using Godot;
using System;

public partial class WoodSource : ResourceNode
{
	public WoodSource() : base(new Resource(ResourceType.WOOD)){}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		GD.Print(base.resource.Quantity);
	}
}
