using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : CharacterBody2D
{
	[Export] public Health health;
	public PlayerLevel Level { get; private set; } = PlayerLevel.ONE;

	public const float SPEED = 100.0f;
	public const float DECELERATION = 5000.0f;
	public const int ATTACK_DAMAGE = 35;
	public const int HEALTH_REGEN_VAL = 10;

	public bool playerAlive = true;
	public bool followPlayer = false;

	private AnimatedSprite2D sprite;
	public List<Enemy> enemies;

	public Area2D attackDetectionArea;
	public CollisionShape2D attackArea;

	private readonly List<Enemy> enemiesInAttackArea = new();
	private bool enemyInAttackArea = false;

	private float attackCooldown = 0.7f;
	private float timeSinceLastAttack = 0.8f;

	private float timeSinceLastHealthRegen = 0f;
	private float healthRegenCooldown = 15f;

	private Vector2 originalAttackAreaPosition;

	private const string PLAYER_IDLE_ANIMATION = "idle_animation";
	private const string PLAYER_RUN_ANIMATION = "run_animation";
	private const string PLAYER_ATTACK_ANIMATION = "attack_animation";

	public override void _Ready()
	{
		AddToGroup("Player");

		sprite = GetNode<AnimatedSprite2D>("PlayerSprite");
		enemies = GameManager.Instance.AllEnemies;

		attackDetectionArea = GetNode<Area2D>("DetectionArea");
		attackArea = GetNode<CollisionShape2D>("DetectionArea/AttackArea");
		originalAttackAreaPosition = attackArea.Position;

		attackDetectionArea.Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
		attackDetectionArea.Connect("body_exited", new Callable(this, nameof(OnBodyExited)));
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!playerAlive)
			return;

		Vector2 velocity = Velocity;
		timeSinceLastAttack += (float)delta;

		Vector2 direction = Input.GetVector("left", "right", "up", "down");

		if (direction != Vector2.Zero)
		{
			if (sprite.Animation != PLAYER_ATTACK_ANIMATION)
				sprite.Play(PLAYER_RUN_ANIMATION);

			velocity = direction * SPEED;

			if (direction.X != 0)
			{
				sprite.FlipH = direction.X < 0;

				Vector2 flippedAttackPos = originalAttackAreaPosition;
				flippedAttackPos.X = Mathf.Abs(originalAttackAreaPosition.X) * (sprite.FlipH ? -1 : 1);
				attackArea.Position = flippedAttackPos;
			}
		}
		else
		{
			if (sprite.Animation == PLAYER_RUN_ANIMATION)
				sprite.Play(PLAYER_IDLE_ANIMATION);

			velocity.X = Mathf.MoveToward(velocity.X, 0, DECELERATION * (float)delta);
			velocity.Y = Mathf.MoveToward(velocity.Y, 0, DECELERATION * (float)delta);
		}

		if (Input.IsActionJustPressed("attack") && timeSinceLastAttack >= attackCooldown)
		{
			if (sprite.Animation != PLAYER_ATTACK_ANIMATION)
				sprite.Play(PLAYER_ATTACK_ANIMATION);

			foreach (var enemy in enemies)
			{
				if (enemiesInAttackArea.Contains(enemy))
				{
					enemy.health.Damage(Combat.CalculateAttackDamage(ATTACK_DAMAGE));
				}
			}

			timeSinceLastAttack = 0f;
		}

		if (sprite.Animation == PLAYER_ATTACK_ANIMATION && !sprite.IsPlaying())
			sprite.Play(PLAYER_IDLE_ANIMATION);

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		if (health.CurrentHealth <= 0 && playerAlive)
		{
			sprite.Play("death_animation");
			playerAlive = false;
			// TODO: block input or trigger respawn
		}

		HealthRegen(delta);
	}

	public override void _Input(InputEvent @event)
	{
		if(@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo){

			if (Input.IsKeyPressed(Key.Q))
			{
				followPlayer = true;
			}

			if (Input.IsKeyPressed(Key.E))
			{
				followPlayer = false;
			}
		}
	}


	private void HealthRegen(double delta)
	{
		if (playerAlive && health.CurrentHealth < 100 && health.CurrentHealth > 0)
		{
			timeSinceLastHealthRegen += (float)delta;

			if (timeSinceLastHealthRegen > healthRegenCooldown)
			{
				health.Heal(HEALTH_REGEN_VAL);
				timeSinceLastHealthRegen = 0f;
			}
		}
	}

	public PlayerLevel LevelUp()
	{
		this.Level = LevellingUtil<PlayerLevel>.LevelUp((int)this.Level);
		return this.Level;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Enemy enemy)
		{
			enemiesInAttackArea.Add(enemy);
		}
	}

	private void OnBodyExited(Node body)
	{
		if (body is Enemy enemy)
		{
			enemiesInAttackArea.Remove(enemy);
		}
	}
}
