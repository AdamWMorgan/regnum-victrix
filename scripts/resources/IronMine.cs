using Godot;
using System;

public partial class IronMine : ResourceNode
{
	public string ID { get; private set; }
	
	public IronMine() : base(new Resource(ResourceType.IRON)){
		this.ID = Guid.NewGuid().ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
