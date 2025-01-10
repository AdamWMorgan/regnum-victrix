using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node2D
{
	[Export] public PackedScene EnemyScene { get; set; }
	[Export] public int SpawnCount { get; set; } = 5;
	[Export] public Rect2 SpawnArea { get; set; } = new Rect2(Vector2.Zero, new Vector2(400, 400));
	[Export] public float MinSpawnDistance {get; set;} = 400f;
	
	private List<Vector2> _spawnedPositions = new List<Vector2>();

	public override void _Ready()
	{
		SpawnEnemies();
	}

	private void SpawnEnemies()
	{
		Random random = new Random();

		for (int i = 0; i < SpawnCount; i++)
		{
			int attempt = 0;
			int maxAttempt = 10;
			Vector2 spawnPosition;
			// Instance the enemy
			Node2D enemy = (Node2D)EnemyScene.Instantiate();
			do {
				// Randomize the spawn position within the defined area
				spawnPosition = SpawnArea.Position + new Vector2(
					(float)random.NextDouble() * SpawnArea.Size.X,
					(float)random.NextDouble() * SpawnArea.Size.Y
				);
				attempt++;
			} while(IsTooClose(spawnPosition) && attempt < maxAttempt);
			enemy.Position = spawnPosition;

			_spawnedPositions.Add(spawnPosition);
			// Add the enemy to the scene
			AddChild(enemy);
		}
	}
	
	private bool IsTooClose(Vector2 position) {
		foreach (var otherPosition in _spawnedPositions){
			if(position.DistanceTo(otherPosition) > MinSpawnDistance){
				return true;
			}
		}
		return false;
	}
}
