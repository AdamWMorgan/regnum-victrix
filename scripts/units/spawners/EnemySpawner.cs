using Godot;

public partial class EnemySpawner : Spawner
{
	public override void _Ready()
	{
		Init(Faction.ENEMY);
		base._Ready();
	}
}
