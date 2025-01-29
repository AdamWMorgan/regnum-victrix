using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	// Reference to the Area2D node
	[Export] public Area2D DetectionArea;
	[Export] public Area2D AttackArea;
	public static int MAX_HEALTH = 60;
	public const int ATTACK_DAMAGE = 10;
	public int enemyHealth = MAX_HEALTH;
	public bool enemyAlive = true;
	public bool playerInAttackRange = false;
	public bool playerDetected = false;
	private float attackCooldown = 1.2f; // Cooldown duration in seconds
	private float timeSinceLastAttack = 1.2f; // Tracks time since the last attack
	public int Health {
			get => enemyHealth;
			set {
				enemyHealth = Mathf.Clamp(value, 0, MAX_HEALTH);
				enemyHealthBar.Value = enemyHealth;
			}
	}
	public ProgressBar enemyHealthBar;
	public AnimatedSprite2D sprite;
	public Player player;	
	public const float SPEED = 1.0f;
	public const float DECELERATION = 5000.0f;

	public override void _Ready()
	{
		// Connect the body_entered and body_exited signals to the methods
		AttackArea.BodyEntered += OnBodyEnteredAttackArea;
		AttackArea.BodyExited += OnBodyExitedAttackArea;
		DetectionArea.BodyEntered += OnBodyEnteredDetectionArea;
		DetectionArea.BodyExited += OnBodyExitedDetectionArea;
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");  
		player = GetNode<Player>("/root/Main/Player");
		enemyHealthBar = GetNode<ProgressBar>("EnemyHealth");
		enemyHealthBar.Value = enemyHealth;
		enemyHealthBar.MaxValue = MAX_HEALTH;  
	}
	
	public override void _PhysicsProcess(double delta)
	{		
		if(enemyAlive){
		Vector2 velocity = Velocity;

		Vector2 directionToPlayer = player.Position - GlobalPosition;
		if(directionToPlayer.X > 0){
			sprite.FlipH = false;
		}
		else if (directionToPlayer.X < 0){
			sprite.FlipH = true;	
		}
		
		if(playerDetected){
			velocity = directionToPlayer * SPEED;
		}
		
		timeSinceLastAttack += (float)delta;
		
		if(playerInAttackRange && timeSinceLastAttack >= attackCooldown){
			AttackPlayer(player);
			timeSinceLastAttack = 0.0f;
		} else if(!playerInAttackRange){
			NormalState();
		}
		// Update velocity and move the character
		Velocity = velocity;
		MoveAndSlide();
		}
	}
	
	public override void _Process(double delta){
		if(enemyHealth <= 0 && enemyAlive){
			enemyAlive = false;
			sprite.Play("enemy_death_animation");
		}
	}
	
	// Called when a body enters the Area2D
	private void OnBodyEnteredAttackArea(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			playerInAttackRange = true;
		}
	}

	// Called when a body exits the Area2D
	private void OnBodyExitedAttackArea(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			playerInAttackRange = false;
		}
	}
	
	private void OnBodyEnteredDetectionArea(Node body){
		if(body.IsInGroup("Player")){
			playerDetected = true;
		}
	}
	
	private void OnBodyExitedDetectionArea(Node body){
		if(body.IsInGroup("Player")){
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
		GD.Print("Player is in range! Attack!");
		sprite.Play("enemy_attack_animation");
		player.Health -= ATTACK_DAMAGE;
		GD.Print(player.Health);
	}

	private void NormalState()
	{
		sprite.Play("enemy_idle_animation");
	}
}
