using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float SPEED = 300.0f;
	public const float DECELERATION = 5000.0f; // Deceleration speed when no input is detected

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Get the input direction
		Vector2 direction = Input.GetVector("left", "right", "up", "down");

		// Apply movement if input is detected
		if (direction != Vector2.Zero)
		{
			velocity = direction * SPEED;
		}
		else
		{
			// Decelerate smoothly when no input is detected
			velocity.X = Mathf.MoveToward(velocity.X, 0, DECELERATION * (float)delta);
			velocity.Y = Mathf.MoveToward(velocity.Y, 0, DECELERATION * (float)delta);
		}

		// Update velocity and move the character
		Velocity = velocity;
		MoveAndSlide();
	}
}
