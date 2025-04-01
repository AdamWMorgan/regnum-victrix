using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set;}
	public List<Enemy> AllEnemies { get; private set;} = new List<Enemy>();
	public List<Ally> AllAllies { get; private set;} = new List<Ally>();
	public List<Base> AllBases {get; set;} = new List<Base>();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (Instance != null){
			QueueFree();
			return;
		}
		Instance = this;
	}

	public void RegisterEnemy(Enemy enemy){
		AllEnemies.Add(enemy);
	}
	
	public void RegisterEnemyWithBase(Enemy enemy, string baseId){
		AllEnemies.Add(enemy);
		Base targetBase = AllBases.Find(b => b.ID == baseId);
		targetBase.AddUnit(enemy);
	}
	
	public void UnregisterEnemy(Enemy enemy){
		AllEnemies.Remove(enemy);
	}
	// can avoid duplication by checking instance of object
	public void RegisterAlly(Ally ally){
		AllAllies.Add(ally);
	}
	
	public void RegisterAllyWithBase(Ally ally, string baseId){
		AllAllies.Add(ally);
		Base targetBase = AllBases.Find(b => b.ID == baseId);
		targetBase.AddUnit(ally);
	}
	
	public void UnregisterAlly(Ally ally){
		AllAllies.Remove(ally);
	}
	
	public string BaseRegister(Base newBase){
		AllBases.Add(newBase);
		return newBase.ID;
	}
}
