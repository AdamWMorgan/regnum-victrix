using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : CharacterBody2D
{
	[Export] public Health health;
	public const float SPEED = 100.0f;
	public const float DECELERATION = 5000.0f;
	public const int ATTACK_DAMAGE = 30;
	public bool playerAlive = true;
	private AnimatedSprite2D sprite;
	public List<Enemy> enemies;
	public Area2D attackDetectionArea;
	public CollisionShape2D attackArea;
	private bool enemyInAttackArea = false;
	private List<Enemy> enemiesInAttackArea = new List<Enemy>();
	private float attackCooldown = 0.8f; // Cooldown duration in seconds
	private float timeSinceLastAttack = 0.8f; // Tracks time since the last attack	
	private float timeSinceLastHealthRegen = 0f;
	private float healthRegenCooldown = 20f;
	private float attackPosX = 0.0f;
	private float attackPosY = 0.0f;
	private const String PLAYER_IDLE_ANIMATION = "idle_animation";
	private const String PLAYER_ATTACK_ANIMATION = "attack_animation";
	
	public override void _Ready()
	{
		AddToGroup("Player");
		sprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		// this doesn't get enemies from spawner
		enemies = GameManager.Instance.AllEnemies;
		attackDetectionArea = GetNode<Area2D>("DetectionArea");
		attackArea = GetNode<CollisionShape2D>("DetectionArea/AttackArea");
		attackPosX = attackArea.Position.X;
		attackPosY = attackArea.Position.Y;
		// Connect signals for the detection area
		attackDetectionArea.Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
		attackDetectionArea.Connect("body_exited", new Callable(this, nameof(OnBodyExited)));
	}

	public override void _PhysicsProcess(double delta)
	{
		if(playerAlive){
		Vector2 velocity = Velocity;
		timeSinceLastAttack += (float)delta;
		// Get the input direction
		Vector2 direction = Input.GetVector("left", "right", "up", "down");

		// Apply movement if input is detected
		if (direction != Vector2.Zero)
		{
			velocity = direction * SPEED;
			// Flip the sprite based on movement direction
			if (direction.X != 0)
			{				
				float directionSign = Mathf.Sign(direction.X);
				sprite.Scale = new Vector2(directionSign, sprite.Scale.Y);
				float currX = attackArea.Position.X;
				float currY = attackArea.Position.Y;
				
				// Attack working in both directions by moving attack area
				if(direction.X < 0){
					attackPosX = -Math.Abs(attackPosX);
					attackPosY = -Math.Abs(attackPosY);
					attackArea.Position = new Vector2(attackPosX, attackPosY);
				} else {
					attackPosX = +Math.Abs(attackPosX);
					attackPosY = +Math.Abs(attackPosY);
					attackArea.Position = new Vector2(attackPosX, attackPosY);
				}
			}
		}
		else
		{
			// Decelerate smoothly when no input is detected
			velocity.X = Mathf.MoveToward(velocity.X, 0, DECELERATION * (float)delta);
			velocity.Y = Mathf.MoveToward(velocity.Y, 0, DECELERATION * (float)delta);
		}

		// Play attack animation when the attack action is pressed
		if (Input.IsActionJustPressed("attack") && timeSinceLastAttack >= attackCooldown)
		{
			if (!sprite.IsPlaying() || sprite.Animation != PLAYER_ATTACK_ANIMATION)
			{
				sprite.Play(PLAYER_ATTACK_ANIMATION);
			}
			for(int i = 0; i < enemies.Count; i++)
			{	
				Enemy enemy = enemies[i];
				if(enemiesInAttackArea.Any(e => e == enemy))
				{
					enemy.health.Damage(ATTACK_DAMAGE);
				}
			}	
			timeSinceLastAttack = 0.0f;
		}

		// If attack animation finishes, return to idle animation
		if (sprite.Animation == PLAYER_ATTACK_ANIMATION && !sprite.IsPlaying())
		{
			sprite.Play(PLAYER_IDLE_ANIMATION);
		}

		// Update velocity and move the character
		Velocity = velocity;
		MoveAndSlide();
		}
	}
	
	public override void _Process(double delta){
		if(health.CurrentHealth <= 0 && playerAlive){
			sprite.Play("death_animation");
			playerAlive = false;
			// Todo: will either need block control input here or straight into respawn
		}
		
		HealthRegen(delta);
	}
	
	private void HealthRegen(double delta){
		if( playerAlive && (health.CurrentHealth < 100 || health.CurrentHealth > 0) ){
			if(timeSinceLastHealthRegen > healthRegenCooldown){
				health.Heal(10);
				timeSinceLastHealthRegen = 0.0f;
			} else {
				timeSinceLastHealthRegen += (float)delta;
			}
		}
	}
	
	private void OnBodyEntered(Node body)
	{
		Enemy enemy = body as Enemy;
		
		if(enemy != null){
			enemiesInAttackArea.Add(enemy);
		}
	}

	private void OnBodyExited(Node body)
	{
		Enemy enemy = body as Enemy;
		if(enemy != null){
			enemiesInAttackArea.Remove(enemy);
		}
	}
}
