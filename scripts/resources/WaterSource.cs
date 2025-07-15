using Godot;
using System;

public partial class WaterSource : ResourceNode
{
	public string ID { get; private set; }
	private int levelUpThreshold = 50;

	public WaterSource() : base(new Resource(ResourceType.WATER))
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
		if (lifetimeResourceCreation > 0 && lifetimeResourceCreation % 50 == 0)
		{
			LevelUp();
			GD.Print("water level up = " + Level);
		}
	}
}
