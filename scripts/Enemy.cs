using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	// Reference to the Area2D node
	[Export] public Area2D DetectionArea;
	public AnimatedSprite2D sprite;
	public Player player;	
	public const float SPEED = 100.0f;
	public const float DECELERATION = 5000.0f;

	public override void _Ready()
	{
		// Connect the body_entered and body_exited signals to the methods
		DetectionArea.BodyEntered += OnBodyEntered;
		DetectionArea.BodyExited += OnBodyExited;
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");  
		player = GetNode<Player>("/root/Main/Player");	 
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 directionToPlayer = player.Position - Position;
		
		if(directionToPlayer.X > 0){
			sprite.FlipH = false;
		}
		else if (directionToPlayer.X < 0){
			sprite.FlipH = true;	
		}
		
		// Update velocity and move the character
		Velocity = velocity;
		MoveAndSlide();
	}
	
	// Called when a body enters the Area2D
	private void OnBodyEntered(Node body)
	{
		GD.Print("Player entered!");
		if (body.IsInGroup("Player"))
		{
			AttackPlayer(body);
		}
	}

	// Called when a body exits the Area2D
	private void OnBodyExited(Node body)
	{
		GD.Print("Player exited!");
		if (body.IsInGroup("Player"))
		{
			// Stop attacking or revert to normal state when the player exits
			NormalState();
		}
	}

	// Function to handle the attack logic
	private void AttackPlayer(Node node)
	{
		// Todo: need to improve this so that it's only after the 
		// swing is finished that health is taken.
		// Also it only takes into account 1 swing per time the player enters 
		// the attack zone currently. 
		GD.Print("Player is in range! Attack!");
		sprite.Play("enemy_attack_animation");
		player.Health -= 50;
		GD.Print(player.Health);
	}

	private void NormalState()
	{
		GD.Print("Enemy is idle.");
		sprite.Play("enemy_idle_animation");
	}
}
