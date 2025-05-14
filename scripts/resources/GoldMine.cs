using Godot;
using System;

public partial class GoldMine : ResourceNode
{
	public string ID { get; private set; }
	
	public GoldMine() : base(new Resource(ResourceType.GOLD)){
		this.ID = Guid.NewGuid().ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
