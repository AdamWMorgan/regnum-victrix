using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float SPEED = 100.0f;
	public const float DECELERATION = 5000.0f;
	public const int ATTACK_DAMAGE = 30;
	public static int MAX_HEALTH = 100;
	public int playerHealth = MAX_HEALTH;
	public bool playerAlive = true;
	public int Health {
			get => playerHealth;
			set {
				playerHealth = Mathf.Clamp(value, 0, MAX_HEALTH);
				playerHealthBar.Value = playerHealth;
			}
	}
	public ProgressBar playerHealthBar;
	private AnimatedSprite2D sprite;
	public Enemy enemy;
	public Area2D attackDetectionArea;
	public CollisionShape2D attackArea;
	private bool enemyInAttackArea = false;
	private float attackCooldown = 1.0f; // Cooldown duration in seconds
	private float timeSinceLastAttack = 1.0f; // Tracks time since the last attack
	private float attackPosX = 0.0f;
	private float attackPosY = 0.0f;
	
	public override void _Ready()
	{
		AddToGroup("Player");
		sprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		playerHealthBar = GetNode<ProgressBar>("PlayerHealth");
		playerHealthBar.Value = Health;
		playerHealthBar.MaxValue = MAX_HEALTH;
		enemy = GetNode<Enemy>("/root/Main/Enemy");
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
			if (!sprite.IsPlaying() || sprite.Animation != "attack_animation")
			{
				sprite.Play("attack_animation");
			}
			if(enemyInAttackArea)
			{
				enemy.Health -= ATTACK_DAMAGE;
			}
			timeSinceLastAttack = 0.0f;
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
	
	public override void _Process(double delta){
		if(playerHealth <= 0 && playerAlive){
			sprite.Play("death_animation");
			playerAlive = false;
			// Todo: will either need block control input here or straight into respawn
		}
	}
	
	private void OnBodyEntered(Node body)
	{
		if (body == enemy)
		{
			enemyInAttackArea = true;
		}
	}

	private void OnBodyExited(Node body)
	{
		if (body == enemy)
		{
			enemyInAttackArea = false;
		}
	}
}
