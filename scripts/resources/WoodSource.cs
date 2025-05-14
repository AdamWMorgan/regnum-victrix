using Godot;
using System;

public partial class WoodSource : ResourceNode
{
	public String ID { get; private set; }
	
	public WoodSource() : base(new Resource(ResourceType.WOOD)){
		this.ID = Guid.NewGuid().ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
