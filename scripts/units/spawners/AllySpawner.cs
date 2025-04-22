using Godot;
using System;
using System.Collections.Generic;

public partial class AllySpawner : Node2D
{
	[Export] public PackedScene AllyScene { get; set; }
	[Export] public int AllySpawnCount { get; set; }
	[Export] public Rect2 AllySpawnArea { get; set; }
	[Export] public float AllyMinSpawnDistance { get; set; } = 0.8f;
	[Export] public float AllyMaxSpawnDistance { get; set; } = 1.2f;

	public CharacterBody2D Player { get; set; }

	private List<Vector2> _allySpawnedPositions = new List<Vector2>();
	private readonly List<Ally> allies = new();
	public string baseId;

	public override void _Ready()
	{
		Player = GetNode<CharacterBody2D>("/root/Main/Player");
		CallDeferred(nameof(DeferredCheck));
	}

	private void DeferredCheck()
	{
		Node parent = GetParent();

		if (parent is AllyBase allyBase && allyBase.BaseID != null)
		{
			baseId = allyBase.BaseID;
			SpawnAllies();
		}
	}
	public void DespawnAlly(Node body)
	{
		RemoveChild(body);
	}
	private void SpawnAllies()
	{
		Random random = new Random();

		for (int i = 0; i < AllySpawnCount; i++)
		{
			int attempt = 0;
			int maxAttempt = 10;
			Vector2 spawnPosition;

			// Instantiate the ally scene
			Ally ally = AllyScene.Instantiate<Ally>();

			// Randomize the spawn position
			do
			{
				spawnPosition = (this.GlobalPosition + AllySpawnArea.Position + new Vector2(
					(float)random.NextDouble() * AllySpawnArea.Size.X,
					(float)random.NextDouble() * AllySpawnArea.Size.Y
				));
				attempt++;
			} while (IsTooClose(spawnPosition) && attempt < maxAttempt);

			ally.Position = spawnPosition;
			ally.spawnPosition = spawnPosition;

			_allySpawnedPositions.Add(spawnPosition);

			// Add the ally to the scene (parent it to the root or another node)
			AddChild(ally);
			GameManager.Instance.RegisterAllyWithBase(ally, baseId);
		}
	}

	private bool IsTooClose(Vector2 position)
	{
		foreach (var otherPosition in _allySpawnedPositions)
		{
			if (position.DistanceTo(otherPosition) < AllyMinSpawnDistance && position.DistanceTo(otherPosition) > AllyMaxSpawnDistance)
			{
				return true;
			}
		}
		return false;
	}
}
