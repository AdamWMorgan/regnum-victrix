using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node2D
{
	[Export] public PackedScene Scene { get; set; }
	[Export] public int SpawnCount { get; set; } = 10;
	[Export] public Rect2 SpawnArea { get; set; }
	[Export] public float MinSpawnDistance { get; set; } = 0.2f;
	[Export] public float MaxSpawnDistance { get; set; } = 0.5f;

	public string baseId;

	private List<Vector2> _spawnedPositions = new();
	private Faction spawnerFaction;

	public Spawner(Faction faction)
	{
		this.spawnerFaction = faction;
	}

	public override void _Ready()
	{
		CallDeferred(nameof(DeferredCheck));
	}

	private void DeferredCheck()
	{
		Node parent = GetParent();

		if (Faction.ENEMY == spawnerFaction)
		{
			if (parent is EnemyBase enemyBase && enemyBase.ID != null)
			{
				baseId = enemyBase.ID;
				Spawn();
			}
		}
		else if (Faction.ALLY == spawnerFaction)
		{
			if (parent is AllyBase allyBase && allyBase.ID != null)
			{
				baseId = allyBase.ID;
				Spawn();
			}
		}
	}

	public void Despawn(Node body)
	{
		RemoveChild(body);
	}
	public override void _Draw()
	{
		DrawRect(SpawnArea, new Color(1, 1, 0, 0.4f), false); // Yellow outline
	}
	private void Spawn()
	{
		Random random = new();

		for (int i = 0; i < SpawnCount; i++)
		{
			int attempt = 0;
			int maxAttempt = 10;
			Vector2 spawnPosition;

			// Randomize the spawn position
			do
			{
				spawnPosition = GlobalPosition + SpawnArea.Position + new Vector2(
					(float)random.NextDouble() * SpawnArea.Size.X,
					(float)random.NextDouble() * SpawnArea.Size.Y
				);
				attempt++;
			} while (IsTooClose(spawnPosition) && attempt < maxAttempt);

			_spawnedPositions.Add(spawnPosition);

			if (Faction.ENEMY == spawnerFaction)
			{
				// Instantiate the enemy scene
				Enemy enemy = Scene.Instantiate<Enemy>();
				enemy.GlobalPosition  = spawnPosition;
				// Add the enemy to the scene (parent it to the root or another node)
				AddChild(enemy);
				GameManager.Instance.RegisterEnemyWithBase(enemy, baseId);
			}
			else if (Faction.ALLY == spawnerFaction)
			{
				// Instantiate the ally scene
				Ally ally = Scene.Instantiate<Ally>();
				ally.GlobalPosition  = spawnPosition;
				ally.spawnPosition = spawnPosition;

				// Add the ally to the scene (parent it to the root or another node)
				AddChild(ally);
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
}
