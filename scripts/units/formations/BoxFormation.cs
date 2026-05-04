using System;
using System.Collections.Generic;
using Godot;

public partial class BoxFormation : Node
{
	public static BoxFormation Instance { get; private set; }
	private Player player;
	private int rowWidth = 5;
	private float spacing = 20f;
	private float SPEED = 80f;
	private List<Ally> allies = new List<Ally>();

	public override void _Ready()
	{
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
		if (allies.Count > 0)
		{
			Vector2 playerPos = player.GlobalPosition;

			float distanceBehind = 120f;
			Vector2 formationCenter = playerPos + new Vector2(-distanceBehind, 0);

			int i = 0;

			int rows = (int) Math.Ceiling((double)allies.Count/rowWidth);

			for (int y = 0; y < rowWidth; y++)
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
		Vector2 currentPos = ally.GlobalPosition;
		Vector2 newPos = currentPos.MoveToward(targetPos, SPEED * (float)delta);
		ally.GlobalPosition = newPos;
	}
}
