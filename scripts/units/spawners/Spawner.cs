using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node2D
{
	[Export] public PackedScene EnemyScene { get; set; }
	[Export] public int SpawnCount { get; set; } = 10;
	[Export] public Rect2 SpawnArea { get; set; }
	[Export] public float MinSpawnDistance { get; set; } = 0.2f;
	[Export] public float MaxSpawnDistance { get; set; } = 0.5f;
	
	public CharacterBody2D Player { get; set; }
	public string baseId;
	
	private List<Vector2> _spawnedPositions = new List<Vector2>();
	private readonly List<Enemy> enemies = new();

	public override void _Ready()
	{
		Player = GetNode<CharacterBody2D>("/root/Main/Player");
		CallDeferred(nameof(DeferredCheck));
	}
	
	private void DeferredCheck()
	{	
	 	Node parent = GetParent();
		EnemyBase enemyBase = parent as EnemyBase;
		
		if(enemyBase != null && enemyBase.BaseID != null){
			baseId = enemyBase.BaseID;
			SpawnEnemies();
		}
	}
	
public void DespawnEnemy(Node body){
	RemoveChild(body);
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
		Enemy enemy = EnemyScene.Instantiate<Enemy>();
		
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
		AddChild(enemy);
		GameManager.Instance.RegisterEnemyWithBase(enemy, baseId);
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
