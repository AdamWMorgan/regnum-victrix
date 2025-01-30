using Godot;
using System;

public partial class Ally : CharacterBody2D
{
	// Reference to the Area2D node
	[Export] public Area2D DetectionArea;
	[Export] public Area2D AttackArea;
	public static int MAX_HEALTH = 60;
	public const int ATTACK_DAMAGE = 10;
	public int allyHealth = MAX_HEALTH;
	public bool allyAlive = true;
	public bool playerInAttackRange = false;
	public bool playerDetected = false;
	private float attackCooldown = 1.2f; // Cooldown duration in seconds
	private float timeSinceLastAttack = 1.2f; // Tracks time since the last attack
	public int Health {
			get => allyHealth;
			set {
				allyHealth = Mathf.Clamp(value, 0, MAX_HEALTH);
				allyHealthBar.Value = allyHealth;
			}
	}
	public ProgressBar allyHealthBar;
	public AnimatedSprite2D sprite;
	public Player player;	
	public const float SPEED = 0.5f;
	public const float DECELERATION = 5000.0f;
	public const float ALLY_PLAYER_GAP = 30.0f;

	public override void _Ready()
	{
		// Connect the body_entered and body_exited signals to the methods
		AttackArea.BodyEntered += OnBodyEnteredAttackArea;
		AttackArea.BodyExited += OnBodyExitedAttackArea;
		DetectionArea.BodyEntered += OnBodyEnteredDetectionArea;
		DetectionArea.BodyExited += OnBodyExitedDetectionArea;
		sprite = GetNode<AnimatedSprite2D>("AllySprite");  
		player = GetNode<Player>("/root/Main/Player");
		allyHealthBar = GetNode<ProgressBar>("AllyHealth");
		allyHealthBar.Value = allyHealth;
		allyHealthBar.MaxValue = MAX_HEALTH;  
	}
	
	public override void _PhysicsProcess(double delta)
	{		
		if(allyAlive){
		Vector2 velocity = Velocity;

		Vector2 directionToPlayer = player.Position - GlobalPosition ;
		if(directionToPlayer.X > 0){
			sprite.FlipH = false;
		}
		else if (directionToPlayer.X < 0){
			sprite.FlipH = true;	
		}
		GD.Print("distance = " + player.GlobalPosition.DistanceTo(this.GlobalPosition));
		if(playerDetected && player.GlobalPosition.DistanceTo(this.GlobalPosition) > ALLY_PLAYER_GAP)
		{
			velocity = directionToPlayer * SPEED;
		}
		else
		{
			velocity = Vector2.Zero;
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
		} else {
			GameManager.Instance.UnregisterAlly(this);
			//GetParent().RemoveChild(this);
		}
	}
	
	public override void _Process(double delta){
		if(allyHealth <= 0 && allyAlive){
			allyAlive = false;
			sprite.Play("ally_death_animation");
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
		GD.Print("in detection area");
		if(body.IsInGroup("Player")){
			GD.Print("in group");
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
		sprite.Play("ally_attack_animation");
		player.Health -= ATTACK_DAMAGE;
		GD.Print(player.Health);
	}

	private void NormalState()
	{
		sprite.Play("ally_idle_animation");
	}
}
