using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Ally : CharacterBody2D, IUnit
{
	public string ID { get; private set; }
	public UnitLevel Level { get; private set; } = UnitLevel.VELITES;
	// Reference to the Area2D node
	[Export] public Area2D DetectionArea;
	[Export] public Area2D PlayerDetectionArea;
	[Export] public Area2D AttackArea;
	[Export] public CollisionShape2D collisionShape;
	[Export] public Health health;
	public Vector2 spawnPosition;
	public const int ATTACK_DAMAGE = 10;
	public const int HEALTH_REGEN_VAL = 10;
	public bool allyAlive = true;
	public bool playerInAttackRange = false;
	public bool enemyInAttackRange = false;
	public bool allyInAttackRange = false;
	private readonly List<Ally> nearbyAllies = new();
	private readonly List<Enemy> detectedEnemies = new();
	private readonly List<Enemy> attackableEnemies = new();
	public bool playerDetected = false;
	private bool followPlayer = false;
	private float attackCooldown = 1.2f; // Cooldown duration in seconds
	private float timeSinceLastAttack = 1.2f; // Tracks time since the last attack
	private float timeSinceLastHealthRegen = 0f;
	private float healthRegenCooldown = 30f;
	public AnimatedSprite2D sprite;
	public Player player;
	public const float SPEED = 60.0f;
	public const float DECELERATION = 5000.0f;
	public const float ALLY_PLAYER_GAP = 50.0f;
	private const string ALLY_IDLE_ANIMATION = "ally_idle_animation";
	private const string ALLY_ATTACK_ANIMATION = "ally_attack_animation";
	private bool inFormation = false;

	public Ally()
	{
		this.ID = Guid.NewGuid().ToString();
	}

	public string GetId()
	{
		return ID;
	}

	public override void _Ready()
	{
		spawnPosition = GlobalPosition;
		AddToGroup("Ally");
		// Connect the body_entered and body_exited signals to the methods
		AttackArea.BodyEntered += OnBodyEnteredAttackArea;
		AttackArea.BodyExited += OnBodyExitedAttackArea;
		DetectionArea.BodyEntered += OnBodyEnteredDetectionArea;
		DetectionArea.BodyExited += OnBodyExitedDetectionArea;
		PlayerDetectionArea.BodyEntered += OnBodyEnteredPlayerDetectionArea;
		PlayerDetectionArea.BodyExited += OnBodyExitedPlayerDetectionArea;
		sprite = GetNode<AnimatedSprite2D>("AllySprite");
		player = GetNode<Player>("/root/Main/Player");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (allyAlive)
		{
			timeSinceLastAttack += (float)delta;
			Vector2 velocity = Vector2.Zero;

			detectedEnemies.RemoveAll(enemy => enemy.health.CurrentHealth <= 0);
			if (detectedEnemies.Count > 0)
			{
				Vector2 directionToEnemy = (detectedEnemies[0].GlobalPosition - GlobalPosition).Normalized();
				velocity += directionToEnemy * SPEED;

				if (directionToEnemy.X > 0)
				{
					sprite.FlipH = false;
				}
				else if (directionToEnemy.X < 0)
				{
					sprite.FlipH = true;
				}
				if (attackableEnemies.Count > 0 && attackableEnemies[0].health.CurrentHealth > 0 && timeSinceLastAttack >= attackCooldown)
				{
					if (!sprite.IsPlaying() || sprite.Animation != ALLY_ATTACK_ANIMATION)
					{
						sprite.Play(ALLY_ATTACK_ANIMATION);
					}
					attackableEnemies[0].health.Damage(Combat.CalculateAttackDamage(ATTACK_DAMAGE));
					timeSinceLastAttack = 0.0f;
				}
				else if (attackableEnemies.Count == 0)
				{
					// If attack animation finishes, return to idle animation
					if (sprite.Animation == ALLY_ATTACK_ANIMATION)
					{
						sprite.Play(ALLY_IDLE_ANIMATION);
					}
				}
			}
			else if (detectedEnemies.Count == 0 && sprite.Animation == ALLY_ATTACK_ANIMATION)
			{
				sprite.Play(ALLY_IDLE_ANIMATION);
			}
			if (playerDetected)
			{
				if (player.followPlayer)
				{
					Vector2 directionToPlayer = (player.GlobalPosition - GlobalPosition).Normalized();

					if (directionToPlayer.X > 0)
					{
						sprite.FlipH = false;
					}
					else if (directionToPlayer.X < 0)
					{
						sprite.FlipH = true;
					}

					if (!inFormation && BoxFormation.Instance != null)
					{
						BoxFormation.Instance.registerAlly(this);
						inFormation = true;
					}
				}
				else
				{
					BoxFormation.Instance.deRegisterAlly(this);

					if (this.GlobalPosition.DistanceTo(this.spawnPosition) > 5.0f)
					{
						Vector2 directionToSpawn = this.spawnPosition - this.GlobalPosition;
						velocity += directionToSpawn * SPEED;

						if (directionToSpawn.X > 0)
						{
							sprite.FlipH = false;
						}
						else if (directionToSpawn.X < 0)
						{
							sprite.FlipH = true;
						}
					}
				}

				// if (player.GlobalPosition.DistanceTo(GlobalPosition) > ALLY_PLAYER_GAP && followPlayer)
				// {
				// 	// If attack animation finishes, return to idle animation
				// 	if (sprite.Animation == ALLY_ATTACK_ANIMATION)
				// 	{
				// 		sprite.Play(ALLY_IDLE_ANIMATION);
				// 	}

				// 	Vector2 directionToPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
				// 	velocity += directionToPlayer * SPEED;
				// 	if (directionToPlayer.X > 0)
				// 	{
				// 		sprite.FlipH = false;
				// 	}
				// 	else if (directionToPlayer.X < 0)
				// 	{
				// 		sprite.FlipH = true;
				// 	}
				// }
			}

			// foreach (Ally otherAlly in nearbyAllies)
			// {
			// 	Vector2 moveAway = (GlobalPosition - otherAlly.GlobalPosition).Normalized();
			// 	velocity += moveAway * (SPEED * 0.7f); // Reduce weight so it doesn't overpower player movement
			// }

			if (velocity.Length() > SPEED)
			{
				velocity = velocity.Normalized() * SPEED;
			}

			timeSinceLastAttack += (float)delta;

			//if(playerInAttackRange && timeSinceLastAttack >= attackCooldown){
			//AttackPlayer(player);
			//timeSinceLastAttack = 0.0f;
			//} else if(!playerInAttackRange){
			//NormalState();
			//}

			// Update velocity and move the character
			Velocity = velocity;
			MoveAndSlide();
		}
		else
		{
			if (inFormation)
			{
				BoxFormation.Instance.deRegisterAlly(this);
				inFormation = false;
				followPlayer = false;
			}
			GameManager.Instance.UnregisterAlly(this);
			collisionShape.Disabled = true;
			//GetParent().RemoveChild(this);
		}
	}

	public override void _Process(double delta)
	{
		if (health.CurrentHealth <= 0 && allyAlive)
		{
			allyAlive = false;
			sprite.Play("ally_death_animation");
		}
		HealthRegen(delta);
	}

	public UnitLevel LevelUp()
	{
		this.Level = LevellingUtil<UnitLevel>.LevelUp((int)this.Level);
		return this.Level;
	}

	private void HealthRegen(double delta)
	{
		if (allyAlive && (health.CurrentHealth < 100 || health.CurrentHealth > 0))
		{
			if (timeSinceLastHealthRegen > healthRegenCooldown)
			{
				health.Heal(HEALTH_REGEN_VAL);
				timeSinceLastHealthRegen = 0.0f;
			}
			else
			{
				timeSinceLastHealthRegen += (float)delta;
			}
		}
	}

	// Called when a body enters the Area2D
	private void OnBodyEnteredAttackArea(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			playerInAttackRange = true;
		}

		if (body.IsInGroup("Ally"))
		{
			allyInAttackRange = true;
			nearbyAllies.Add((Ally)body);
		}

		if (body.IsInGroup("Enemy"))
		{
			enemyInAttackRange = true;
			attackableEnemies.Add((Enemy)body);
		}
	}

	// Called when a body exits the Area2D
	private void OnBodyExitedAttackArea(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			playerInAttackRange = false;
		}

		if (body.IsInGroup("Ally"))
		{
			allyInAttackRange = false;
			nearbyAllies.Remove((Ally)body);
		}

		if (body.IsInGroup("Enemy"))
		{
			enemyInAttackRange = false;
			attackableEnemies.Remove((Enemy)body);
		}
	}

	private void OnBodyEnteredDetectionArea(Node body)
	{
		if (body.IsInGroup("Enemy"))
		{
			detectedEnemies.Add((Enemy)body);
		}
	}

	private void OnBodyExitedDetectionArea(Node body)
	{
		if (body.IsInGroup("Enemy"))
		{
			detectedEnemies.Remove((Enemy)body);
		}
	}

	private void OnBodyEnteredPlayerDetectionArea(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			playerDetected = true;
		}
	}

	private void OnBodyExitedPlayerDetectionArea(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			playerDetected = false;
		}
	}

	// Function to handle the attack logic
	private void AttackPlayer(Node node)
	{
		// Todo: need to improve this so that it's only after the 
		// swing is finished that health is taken.
		// Also it only takes into account 1 swing per time the player enters 
		// the attack zone currently. 
		sprite.Play(ALLY_ATTACK_ANIMATION);
		player.health.Damage(ATTACK_DAMAGE);
	}

	private void NormalState()
	{
		sprite.Play(ALLY_IDLE_ANIMATION);
	}
}
