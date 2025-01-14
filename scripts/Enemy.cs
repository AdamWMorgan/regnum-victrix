using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	// Reference to the Area2D node
	[Export] public Area2D DetectionArea;
	public AnimatedSprite2D sprite;
	public CharacterBody2D _playerSprite;	
	public const float SPEED = 100.0f;
	public const float DECELERATION = 5000.0f;

	public override void _Ready()
	{
		// Connect the body_entered and body_exited signals to the methods
		DetectionArea.BodyEntered += OnBodyEntered;
		DetectionArea.BodyExited += OnBodyExited;
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");  
		_playerSprite = GetNode<CharacterBody2D>("/root/Main/Player");	  
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Vector2 directionToPlayer = _playerSprite.Position - Position;
		
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
	private void AttackPlayer(Node player)
	{
		GD.Print("Player is in range! Attack!");
		sprite.Play("enemy_attack_animation");
	}

	private void NormalState()
	{
		GD.Print("Enemy is idle.");
		sprite.Play("enemy_idle_animation");
	}
}
