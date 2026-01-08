using Godot;
using System.Collections.Generic;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }
	public List<Enemy> AllEnemies { get; private set; } = new List<Enemy>();
	public List<Ally> AllAllies { get; private set; } = new List<Ally>();
	public List<Base> AllBases { get; set; } = new List<Base>();
	public List<ResourceNode> ResourceNodes { get; set; } = new List<ResourceNode>();
	public GameConfig gameConfig {get; set;}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Instance != null)
		{
			QueueFree();
			return;
		}
		Instance = this;

		gameConfig = ConfigLoader.Load();
	}

	public override void _Process(double delta)
	{
		GD.Print("Game config = " + gameConfig.BaseLevelConfigPanel.BaseUpgrade[0].Amount);
	}

	// Registering
	public void RegisterEnemy(Enemy enemy)
	{
		AllEnemies.Add(enemy);
	}

	public void RegisterEnemyWithBase(Enemy enemy, string baseId)
	{
		AllEnemies.Add(enemy);
		Base targetBase = AllBases.Find(b => b.ID == baseId);
		targetBase.AddUnit(enemy);
	}

	public void UnregisterEnemy(Enemy enemy)
	{
		AllEnemies.Remove(enemy);
	}

	// can avoid duplication by checking instance of object
	public void RegisterAlly(Ally ally)
	{
		AllAllies.Add(ally);
	}

	public void RegisterAllyWithBase(Ally ally, string baseId)
	{
		AllAllies.Add(ally);
		Base targetBase = AllBases.Find(b => b.ID == baseId);
		targetBase.AddUnit(ally);
	}

	public void UnregisterAlly(Ally ally)
	{
		AllAllies.Remove(ally);
	}

	public string BaseRegister(Base newBase)
	{
		AllBases.Add(newBase);
		return newBase.ID;
	}

	public void BaseUnregister(Base removalBase)
	{
		AllBases.Remove(removalBase);
	}

	public void BaseSwitch(Base currBase)
	{
		Base newBase = null;

		if (currBase.CurrentBaseOwner == Faction.ENEMY)
		{
			// Load the AllyBase scene
			PackedScene allyBaseScene = ResourceLoader.Load<PackedScene>("res://scenes/bases/ally_base.tscn");
			if (allyBaseScene == null)
			{
				GD.PrintErr("Failed to load AllyBase.tscn");
				return;
			}

			// Instantiate and add to the scene
			newBase = allyBaseScene.Instantiate<AllyBase>();
		}
		else if (currBase.CurrentBaseOwner == Faction.ALLY)
		{
			// Load the EnemyBase scene
			PackedScene enemyBaseScene = ResourceLoader.Load<PackedScene>("res://scenes/bases/enemy_base.tscn");
			if (enemyBaseScene == null)
			{
				GD.PrintErr("Failed to load EnemyBase.tscn");
				return;
			}

			// Instantiate and add to the scene
			newBase = enemyBaseScene.Instantiate<EnemyBase>();
		}

		if (newBase != null)
		{
			newBase.GlobalPosition = currBase.GlobalPosition;
			newBase.ZIndex = 10;
			AddChild(newBase);
			ResourceNodeSwitch(currBase, newBase);
			BaseUnregister(currBase);
			currBase.QueueFree();
		}
	}

	public void RegisterResourceNode(ResourceNode resourceNode)
	{
		ResourceNodes.Add(resourceNode);
	}

	private void ResourceNodeSwitch(Base oldBase, Base newBase)
	{
		ResourceNodes.FindAll(node => node.attachedBase.ID == oldBase.ID).ForEach(node =>
		{
			node.SwitchOwnership(newBase.CurrentBaseOwner);
		});
	}

	// Resource State
}
