using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node2D
{
	[Export] public PackedScene Scene { get; set; }
	[Export] public int SpawnCount { get; set; } = 12;
	[Export] public Rect2 SpawnArea { get; set; }
	[Export] public float MinSpawnDistance { get; set; } = 0.3f;
	[Export] public float MaxSpawnDistance { get; set; } = 0.6f;
	[Export] public Faction spawnerFaction { get; set; } = Faction.ENEMY;
	[Export] public Area2D PlayerDetectionArea;

	public string baseId;
	private List<Vector2> _spawnedPositions = new();

	// Required for Godot
	public Spawner(){}

	public void Init(Faction faction)
	{
		spawnerFaction = faction;
	}

	public override void _Ready()
	{

		CallDeferred(nameof(DeferredCheck));
	}

	public override void _Draw()
	{
		// DrawRect(SpawnArea, new Color(1, 1, 0, 0.4f), false); -- USEFUL FOR DEBUGGING SPAWN AREA
	}

	private void DeferredCheck()
	{
		if (PlayerDetectionArea != null)
		{
			PlayerDetectionArea.BodyEntered += OnBodyEnteredDetectionArea;
			PlayerDetectionArea.BodyExited += OnBodyExitedDetectionArea;
		}
		else
		{
			GD.PrintErr("PlayerDetectionArea is not set or missing in the scene!");
		}

		Node parent = GetParent();

		if (spawnerFaction == Faction.ENEMY && parent is EnemyBase enemyBase)
		{
			baseId = enemyBase.ID;
			Spawn();
		}
		else if (spawnerFaction == Faction.ALLY && parent is AllyBase allyBase)
		{
			baseId = allyBase.ID;
			Spawn();
		}
	}

	public void Despawn(Node body)
	{
		RemoveChild(body);
	}

	private void Spawn()
	{
		Random random = new();

		for (int i = 0; i < SpawnCount; i++)
		{
			int attempt = 0;
			int maxAttempt = 10;
			Vector2 spawnPosition;

			do
			{
				spawnPosition = GlobalPosition + SpawnArea.Position + new Vector2(
					(float)random.NextDouble() * SpawnArea.Size.X,
					(float)random.NextDouble() * SpawnArea.Size.Y
				);
				attempt++;
			} while (IsTooClose(spawnPosition) && attempt < maxAttempt);

			_spawnedPositions.Add(spawnPosition);

			if (spawnerFaction == Faction.ENEMY)
			{
				Enemy enemy = Scene.Instantiate<Enemy>();
				AddChild(enemy);
				enemy.GlobalPosition = spawnPosition;
				GameManager.Instance.RegisterEnemyWithBase(enemy, baseId);
			}
			else if (spawnerFaction == Faction.ALLY)
			{
				Ally ally = Scene.Instantiate<Ally>();
				AddChild(ally);
				ally.GlobalPosition = spawnPosition;
				ally.spawnPosition = spawnPosition;
				GameManager.Instance.RegisterAllyWithBase(ally, baseId);
			}
		}
	}

	private bool IsTooClose(Vector2 position)
	{
		foreach (var otherPosition in _spawnedPositions)
		{
			if (position.DistanceTo(otherPosition) < MinSpawnDistance && position.DistanceTo(otherPosition) > MaxSpawnDistance)
			{
				return true;
			}
		}
		return false;
	}

	private void OnBodyEnteredDetectionArea(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			GD.Print("in area");
		}
	}

	private void OnBodyExitedDetectionArea(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			GD.Print("out area");
		}
	}
	
}
