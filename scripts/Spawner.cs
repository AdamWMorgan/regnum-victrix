using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node2D
{
	[Export] public PackedScene EnemyScene { get; set; }
	[Export] public int SpawnCount { get; set; } = 5;
	[Export] public Rect2 SpawnArea { get; set; } = new Rect2(Vector2.Zero, new Vector2(400, 400));
	[Export] public float MinSpawnDistance { get; set; } = 400f;

	public AnimatedSprite2D Player { get; set; }

	private List<Vector2> _spawnedPositions = new List<Vector2>();
	private List<Enemy> enemies = new List<Enemy>();

	public override void _Ready()
	{
		Player = GetNode<AnimatedSprite2D>("/root/Main/Player/PlayerSprite");
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

		// Instantiate the enemy scene
		Enemy enemy = (Enemy)EnemyScene.Instantiate();
		
		enemies.Add(enemy);
		
		if(enemy==null){
			GD.Print("enemy is null");
		}
		if(enemy is not Enemy){
			GD.Print("enemy is not enemy");
		}
		
		// Randomize the spawn position
		do
		{
			spawnPosition = SpawnArea.Position + new Vector2(
				(float)random.NextDouble() * SpawnArea.Size.X,
				(float)random.NextDouble() * SpawnArea.Size.Y
			);
			attempt++;
		} while (IsTooClose(spawnPosition) && attempt < maxAttempt);

		enemy.Position = spawnPosition;

		_spawnedPositions.Add(spawnPosition);

		// Add the enemy to the scene (parent it to the root or another node)
		GetParent().AddChild(enemy);
	}
}

	private bool IsTooClose(Vector2 position)
	{
		foreach (var otherPosition in _spawnedPositions)
		{
			if (position.DistanceTo(otherPosition) < MinSpawnDistance)
			{
				return true;
			}
		}
		return false;
	}
}
