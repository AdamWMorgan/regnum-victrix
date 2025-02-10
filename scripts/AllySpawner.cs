using Godot;
using System;
using System.Collections.Generic;

public partial class AllySpawner : Node2D
{
	[Export] public PackedScene AllyScene { get; set; }
	[Export] public int AllySpawnCount { get; set; } = 1;
	[Export] public Rect2 AllySpawnArea { get; set; } = new Rect2(Vector2.Zero, new Vector2(400, 400));
	[Export] public float AllyMinSpawnDistance { get; set; } = 0.2f;
	[Export] public float AllyMaxSpawnDistance { get; set; } = 0.5f;

	public CharacterBody2D Player { get; set; }

	private List<Vector2> _allySpawnedPositions = new List<Vector2>();
	private List<Ally> allies = new List<Ally>();

	public override void _Ready()
	{
		Player = GetNode<CharacterBody2D>("/root/Main/Player");
		SpawnAllies();
	}
public void DespawnAlly(Node body){
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
			spawnPosition = AllySpawnArea.Position + new Vector2(
				(float)random.NextDouble() * AllySpawnArea.Size.X,
				(float)random.NextDouble() * AllySpawnArea.Size.Y
			);
			attempt++;
		} while (IsTooClose(spawnPosition) && attempt < maxAttempt);

		ally.Position = spawnPosition;

		_allySpawnedPositions.Add(spawnPosition);

		// Add the ally to the scene (parent it to the root or another node)
		AddChild(ally);
		GameManager.Instance.RegisterAlly(ally);
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
