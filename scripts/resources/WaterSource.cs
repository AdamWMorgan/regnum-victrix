using Godot;
using System;

public partial class WaterSource : ResourceNode
{
	public string ID { get; private set; }

	public WaterSource() : base(new Resource(ResourceType.WATER))
	{
		this.ID = Guid.NewGuid().ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
