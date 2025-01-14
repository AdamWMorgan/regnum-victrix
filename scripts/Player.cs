using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float SPEED = 100.0f;
	public const float DECELERATION = 5000.0f;
	public int Health { get; set; } = 100; 
	
	private AnimatedSprite2D sprite;

	public override void _Ready()
	{
		AddToGroup("Player");
		sprite = GetNode<AnimatedSprite2D>("PlayerSprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Get the input direction
		Vector2 direction = Input.GetVector("left", "right", "up", "down");

		// Apply movement if input is detected
		if (direction != Vector2.Zero)
		{
			velocity = direction * SPEED;

			// Flip the sprite based on movement direction
			if (direction.X != 0)
			{
				sprite.Scale = new Vector2(Mathf.Sign(direction.X), sprite.Scale.Y);
			}
		}
		else
		{
			// Decelerate smoothly when no input is detected
			velocity.X = Mathf.MoveToward(velocity.X, 0, DECELERATION * (float)delta);
			velocity.Y = Mathf.MoveToward(velocity.Y, 0, DECELERATION * (float)delta);
		}

		// Play attack animation when the attack action is pressed
		if (Input.IsActionJustPressed("attack"))
		{
			if (!sprite.IsPlaying() || sprite.Animation != "attack_animation")
			{
				sprite.Play("attack_animation");
			}
		}

		// If attack animation finishes, return to idle animation
		if (sprite.Animation == "attack_animation" && !sprite.IsPlaying())
		{
			sprite.Play("idle_animation");
		}

		// Update velocity and move the character
		Velocity = velocity;
		MoveAndSlide();
	}
}
