using Godot;
using System;

public partial class Spawner : Node2D
{
	[Export] public PackedScene EnemyScene { get; set; }
	[Export] public int SpawnCount { get; set; } = 5;
	[Export] public Rect2 SpawnArea { get; set; } = new Rect2(Vector2.Zero, new Vector2(400, 400));

	public override void _Ready()
	{
		SpawnEnemies();
	}

	private void SpawnEnemies()
	{
		Random random = new Random();

		for (int i = 0; i < SpawnCount; i++)
		{
			// Instance the enemy
			Node2D enemy = (Node2D)EnemyScene.Instantiate();

			// Randomize the spawn position within the defined area
			Vector2 spawnPosition = SpawnArea.Position + new Vector2(
				(float)random.NextDouble() * SpawnArea.Size.X,
				(float)random.NextDouble() * SpawnArea.Size.Y
			);
			enemy.Position = spawnPosition;

			// Add the enemy to the scene
			AddChild(enemy);
		}
	}
}
