using Godot;
using System;

public partial class WheatSource : ResourceNode
{
	public String ID { get; private set; }
	
	public WheatSource() : base(new Resource(ResourceType.WHEAT)){
		this.ID = Guid.NewGuid().ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
