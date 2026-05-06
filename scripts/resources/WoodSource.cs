using Godot;
using System;

public partial class WoodSource : ResourceNode
{
	public string ID { get; private set; }
	private int LEVEL_UP_THRESHOLD = 250;
	private int nextLevelUpAt = 250;
	private float DEFAULT_UPGRADE_CHECK = 50f;
	private float timeSinceLastResourceLevelUpCheck = 0f;

	public WoodSource() : base(new Resource(ResourceType.WOOD))
	{
		this.ID = Guid.NewGuid().ToString();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
		
	public override void levelUp(double delta)
	{
		if (timeSinceLastResourceLevelUpCheck >= DEFAULT_UPGRADE_CHECK)
		{
			if (lifetimeResourceCreation >= nextLevelUpAt)
			{
				LevelUp();
				GD.Print("wood level up = " + Level);
				nextLevelUpAt += LEVEL_UP_THRESHOLD;
			}
			timeSinceLastResourceLevelUpCheck = 0f;
		}
		else
		{
			timeSinceLastResourceLevelUpCheck += (float)delta;
		}
	}
}
