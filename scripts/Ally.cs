using Godot;
using System;
using System.Collections.Generic;

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
	public bool allyInAttackRange = false;
	private List<Ally> nearbyAllies = new List<Ally>();
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
	public const float SPEED = 50f;
	public const float DECELERATION = 5000.0f;
	public const float ALLY_PLAYER_GAP = 50.0f;

	public override void _Ready()
	{
		AddToGroup("Ally");
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
		Vector2 velocity = Vector2.Zero;
		
		

		if (playerDetected && player.GlobalPosition.DistanceTo(GlobalPosition) > ALLY_PLAYER_GAP){
			Vector2 directionToPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
			velocity += directionToPlayer * SPEED;
			if(directionToPlayer.X > 0){
				sprite.FlipH = false;
			}
			else if (directionToPlayer.X < 0){
				sprite.FlipH = true;	
			}
		}

		foreach (Ally otherAlly in nearbyAllies)
		{
			Vector2 moveAway = (GlobalPosition - otherAlly.GlobalPosition).Normalized();
			velocity += moveAway * (SPEED * 0.7f); // Reduce weight so it doesn't overpower player movement
		}
		
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
		else if(body.IsInGroup("Ally")){
			allyInAttackRange = true;
			nearbyAllies.Add((Ally) body);
		}
	}

	// Called when a body exits the Area2D
	private void OnBodyExitedAttackArea(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			playerInAttackRange = false;
		}
		else if(body.IsInGroup("Ally")){
			allyInAttackRange = false;
			nearbyAllies.Remove((Ally)body);
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
		sprite.Play("ally_attack_animation");
		player.Health -= ATTACK_DAMAGE;
	}

	private void NormalState()
	{
		sprite.Play("ally_idle_animation");
	}
}
