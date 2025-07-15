using Godot;
using System;

public partial class IronMine : ResourceNode
{
	public string ID { get; private set; }
	private int levelUpThreshold = 50;

	public IronMine() : base(new Resource(ResourceType.IRON))
	{
		this.ID = Guid.NewGuid().ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
	
	public override void levelUp()
	{
		if (lifetimeResourceCreation % 50 == 0)
		{
			LevelUp();
			GD.Print("iron level up = " + Level);
		}
	}
}
