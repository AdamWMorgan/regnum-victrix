using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	// Reference to the Area2D node
	[Export] public Area2D DetectionArea;
	public static int MAX_HEALTH = 80;
	public const int ATTACK_DAMAGE = 20;
	public int enemyHealth = MAX_HEALTH;
	public bool enemyAlive = true;
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
	public const float SPEED = 100.0f;
	public const float DECELERATION = 5000.0f;

	public override void _Ready()
	{
		// Connect the body_entered and body_exited signals to the methods
		DetectionArea.BodyEntered += OnBodyEntered;
		DetectionArea.BodyExited += OnBodyExited;
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
		GD.Print("Position = " + GlobalPosition);
		GD.Print("Player Position = " + player.Position);
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
	}
	
	public override void _Process(double delta){
		if(enemyHealth <= 0 && enemyAlive){
			enemyAlive = false;
			sprite.Play("enemy_death_animation");
		}
	}
	
	// Called when a body enters the Area2D
	private void OnBodyEntered(Node body)
	{
		GD.Print("Player entered!");
		if (body.IsInGroup("Player") && enemyAlive)
		{
			AttackPlayer(body);
		}
	}

	// Called when a body exits the Area2D
	private void OnBodyExited(Node body)
	{
		GD.Print("Player exited!");
		if (body.IsInGroup("Player") && enemyAlive)
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
		player.Health -= ATTACK_DAMAGE;
		GD.Print(player.Health);
	}

	private void NormalState()
	{
		GD.Print("Enemy is idle.");
		sprite.Play("enemy_idle_animation");
	}
}
