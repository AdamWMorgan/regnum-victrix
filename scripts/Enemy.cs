using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	public const float SPEED = 50.0f;
	public const float DECELERATION = 5000.0f; // Deceleration speed when no input is detected

	private AnimatedSprite2D sprite;
	private AnimatedSprite2D _playerSprite;
	
	public void Initialize(AnimatedSprite2D playerSprite){
		_playerSprite = playerSprite;
	}
	
	public override void _Ready(){
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");	
	}
	
	public override void _PhysicsProcess(double delta)
	{		
		if(_playerSprite.Position.DistanceTo(sprite.Position) < 20){
			if(!sprite.IsPlaying() || sprite.Animation != "enemy_attack_animation"){
				sprite.Play("enemy_attack_animation");
			}
		}
		
		if (sprite.Animation == "enemy_attack_animation" && !sprite.IsPlaying())
		{
			sprite.Play("idle_animation");
		}

		MoveAndSlide();
	}
}
