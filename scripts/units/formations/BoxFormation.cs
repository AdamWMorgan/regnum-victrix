using System.Collections.Generic;
using Godot;

public partial class BoxFormation : Node
{
	public static BoxFormation Instance { get; private set; }
	private Player player;
	private int rows = 3;
	private float spacing = 20f;
	private List<Ally> allies = new List<Ally>();

	public override void _Ready()
	{
		GD.Print("Loaded");
		if (Instance != null)
		{
			QueueFree();
			return;
		}
		Instance = this;

		player = GetNode<Player>("/root/Main/Player");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);       
				
		int totalNeeded = rows * rows;
		if (allies.Count < totalNeeded) return;
		Vector2 playerPos = player.GlobalPosition;
		
		float distanceBehind = 100f;
		Vector2 formationCenter = playerPos + new Vector2(-distanceBehind, 0); 

		int i = 0;

		for (int y = 0; y < rows; y++)
		{
			for (int x = 0; x < rows; x++)
			{
				if (i >= allies.Count) break;

				// Calculate offset from center in formation
				float offsetX = (x - (rows - 1) / 2f) * spacing;
				float offsetY = (y - (rows - 1) / 2f) * spacing;
				Vector2 offset = new Vector2(offsetX, offsetY);

				Vector2 targetPos = formationCenter + offset;

				// Move the soldier toward the target position
				MoveSoldier(allies[i], targetPos, delta);
				i++;
			}
		}

	}

	public void registerAlly(Ally ally)
	{
		allies.Add(ally);
	}
	
	public void deRegisterAlly(Ally ally)
	{
		allies.Remove(ally);
	}
	
	private void MoveSoldier(Node2D ally, Vector2 targetPos, double delta)
	{
		float speed = 80f;
		Vector2 currentPos = ally.GlobalPosition;
		Vector2 newPos = currentPos.MoveToward(targetPos, speed * (float)delta);
		ally.GlobalPosition = newPos;
	}
}
