using Godot;

public partial class AllySpawner : Spawner
{
	public override void _Ready()
	{
		Init(Faction.ALLY);
		base._Ready();
	}
}
