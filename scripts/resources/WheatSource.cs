using Godot;
using System;

public partial class WheatSource : ResourceNode
{
	public string ID { get; private set; }
	private int LEVEL_UP_THRESHOLD = 200;
	private int nextLevelUpAt = 200;
	private float DEFAULT_UPGRADE_CHECK = 100f;
	private float timeSinceLastResourceLevelUpCheck = 0f;

	public WheatSource() : base(new Resource(ResourceType.WHEAT))
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
				GD.Print("wheat level up = " + Level);
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
