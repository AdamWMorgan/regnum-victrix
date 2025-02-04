using Godot;
using System;
using System.Collections.Generic;

public partial class Ally : CharacterBody2D
{
	// Reference to the Area2D node
	[Export] public Area2D DetectionArea;
	[Export] public Area2D AttackArea;
	[Export] public CollisionShape2D collisionShape;
	public static int MAX_HEALTH = 60;
	public const int ATTACK_DAMAGE = 10;
	public int allyHealth = MAX_HEALTH;
	public bool allyAlive = true;
	public bool playerInAttackRange = false;
	public bool enemyInAttackRange = false;
	public bool allyInAttackRange = false;
	private List<Ally> nearbyAllies = new List<Ally>();
	private List<Enemy> detectedEnemies = new List<Enemy>();
	private List<Enemy> attackableEnemies = new List<Enemy>();
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
		timeSinceLastAttack += (float)delta;
		Vector2 velocity = Vector2.Zero;
		
		detectedEnemies.RemoveAll(enemy => enemy.Health <= 0);
		if(detectedEnemies.Count > 0){
			Vector2 directionToEnemy = (detectedEnemies[0].GlobalPosition - GlobalPosition).Normalized();
			velocity += directionToEnemy * SPEED;
			
			if(directionToEnemy.X > 0){
				sprite.FlipH = false;
			}
			else if (directionToEnemy.X < 0){
				sprite.FlipH = true;	
			}
			if(attackableEnemies.Count > 0 && attackableEnemies[0].Health > 0 && timeSinceLastAttack >= attackCooldown){
				if (!sprite.IsPlaying() || sprite.Animation != "ally_attack_animation")
				{
					sprite.Play("ally_attack_animation");
				}
				attackableEnemies[0].Health -= ATTACK_DAMAGE;
				timeSinceLastAttack = 0.0f;
			} else if(attackableEnemies.Count == 0) {
				// If attack animation finishes, return to idle animation
				if (sprite.Animation == "ally_attack_animation")
				{
					sprite.Play("ally_idle_animation");
				}
			}
		} 
		else if (playerDetected && player.GlobalPosition.DistanceTo(GlobalPosition) > ALLY_PLAYER_GAP){
			// If attack animation finishes, return to idle animation
			if (sprite.Animation == "ally_attack_animation")
			{
				sprite.Play("ally_idle_animation");
			}
			
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
			collisionShape.Disabled = true;
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
		
		if(body.IsInGroup("Ally")){
			allyInAttackRange = true;
			nearbyAllies.Add((Ally) body);
		}
		
		if(body.IsInGroup("Enemy")){
			enemyInAttackRange = true;
			attackableEnemies.Add((Enemy) body);
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
			nearbyAllies.Remove((Ally)body);
		}
		
		if(body.IsInGroup("Enemy")){
			enemyInAttackRange = false;
			attackableEnemies.Remove((Enemy) body);
		}
	}
	
	private void OnBodyEnteredDetectionArea(Node body){
		if(body.IsInGroup("Player")){
			playerDetected = true;
		}
		
		if(body.IsInGroup("Enemy")){
			detectedEnemies.Add((Enemy) body);
		}
	}
	
	private void OnBodyExitedDetectionArea(Node body){
		if(body.IsInGroup("Player")){
			playerDetected = false;
		}
		
		
		if(body.IsInGroup("Enemy")){
			detectedEnemies.Remove((Enemy) body);
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
