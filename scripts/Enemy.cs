using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : CharacterBody2D
{
	// Reference to the Area2D node
	[Export] public Area2D DetectionArea;
	[Export] public Area2D AttackArea;
	[Export] public CollisionShape2D collisionShape;
	[Export] public Health health;
	public const int ATTACK_DAMAGE = 10;
	public bool enemyAlive = true;
	public bool allyInAttackRange = false;
	public bool playerInAttackRange = false;
	public bool playerDetected = false;
	private List<Ally> detectedAllies = new List<Ally>();
	private List<Ally> attackableAllies = new List<Ally>();
	private float attackCooldown = 1.2f; // Cooldown duration in seconds
	private float timeSinceLastAttack = 1.2f; // Tracks time since the last attack
	public AnimatedSprite2D sprite;
	public Player player;	
	public const float SPEED = 0.5f;
	public const float DECELERATION = 5000.0f;

	public override void _Ready()
	{
		AddToGroup("Enemy");
		// Connect the body_entered and body_exited signals to the methods
		AttackArea.BodyEntered += OnBodyEnteredAttackArea;
		AttackArea.BodyExited += OnBodyExitedAttackArea;
		DetectionArea.BodyEntered += OnBodyEnteredDetectionArea;
		DetectionArea.BodyExited += OnBodyExitedDetectionArea;
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");  
		player = GetNode<Player>("/root/Main/Player");
	}
	
	public override void _PhysicsProcess(double delta)
	{		
		if(enemyAlive){
			
		timeSinceLastAttack += (float)delta;
		Vector2 velocity = Velocity;
		
		detectedAllies.RemoveAll(ally => ally.health.CurrentHealth <= 0);
		attackableAllies.RemoveAll(ally => ally.health.CurrentHealth <= 0);
		
		if (attackableAllies.Count == 0 && !playerInAttackRange && sprite.Animation == "enemy_attack_animation")
		{
			NormalState();
		}
	
		if(detectedAllies.Count > 0){
			Vector2 directionToAlly = detectedAllies[0].GlobalPosition - GlobalPosition;
			velocity = directionToAlly * SPEED;
			
			if(directionToAlly.X > 0){
				sprite.FlipH = false;
			}
			else if (directionToAlly.X < 0){
				sprite.FlipH = true;	
			}
			
			if(attackableAllies.Count > 0 && attackableAllies[0].health.CurrentHealth > 0 && timeSinceLastAttack >= attackCooldown){
				if (!sprite.IsPlaying() || sprite.Animation != "enemy_attack_animation")
				{
					sprite.Play("enemy_attack_animation");
				}
				attackableAllies[0].health.Damage(ATTACK_DAMAGE);
				timeSinceLastAttack = 0.0f;
			} 
		} else if(playerDetected){
			Vector2 directionToPlayer = player.GlobalPosition - GlobalPosition;
			if(directionToPlayer.X > 0){
				sprite.FlipH = false;
			}
			else if (directionToPlayer.X < 0){
				sprite.FlipH = true;	
			}
			velocity = directionToPlayer * SPEED;
			
			if(playerInAttackRange && timeSinceLastAttack >= attackCooldown){
				AttackPlayer(player);
				timeSinceLastAttack = 0.0f;
			}
		} 
		 else{
			velocity = Vector2.Zero;
		}
		// Update velocity and move the character
		Velocity = velocity;
		MoveAndSlide();
		} else {
			GameManager.Instance.UnregisterEnemy(this);
			collisionShape.Disabled = true;
			//GetParent().RemoveChild(this);
		}
	}

	public override void _Process(double delta)
	{
		if(health.CurrentHealth <= 0 && enemyAlive){
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
		if(body.IsInGroup("Ally")){
			allyInAttackRange = true;
			attackableAllies.Add((Ally)body);
		}
	}

	// Called when a body exits the Area2D
	private void OnBodyExitedAttackArea(Node body)
	{
		if (body.IsInGroup("Player"))
		{
			playerInAttackRange = false;
		}		
		if(body.IsInGroup("Ally")){
			allyInAttackRange = false;
			attackableAllies.Remove((Ally)body);
		}
	}
	
	private void OnBodyEnteredDetectionArea(Node body)
	{
		if(body.IsInGroup("Player")){
			playerDetected = true;
		}		
		if(body.IsInGroup("Ally")){
			detectedAllies.Add((Ally) body);
		}
	}
	
	private void OnBodyExitedDetectionArea(Node body){
		if(body.IsInGroup("Player")){
			playerDetected = false;
		}
		if(body.IsInGroup("Ally")){
			detectedAllies.Remove((Ally) body);
		}
	}

	// Function to handle the attack logic
	private void AttackPlayer(Node node)
	{
		// Todo: need to improve this so that it's only after the 
		// swing is finished that health is taken.
		// Also it only takes into account 1 swing per time the player enters 
		// the attack zone currently. 
		sprite.Play("enemy_attack_animation");
		player.health.Damage(ATTACK_DAMAGE);
	}

	private void NormalState()
	{
		sprite.Play("enemy_idle_animation");
	}
}
