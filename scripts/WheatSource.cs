using Godot;
using System;

public partial class WheatSource : ResourceNode
{
	public String ID { get; private set; }
	
	public WheatSource() : base(new Resource(ResourceType.WHEAT)){
		this.ID = Guid.NewGuid().ToString();
	}
	
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
