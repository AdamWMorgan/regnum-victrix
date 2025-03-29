using Godot;
using System;

public partial class Base : Node
{
	
	public String ID { get; private set; }
	public BaseOwner CurrentBaseOwner { get; set; }
	public Vector2 Position;
	public string Name;
	public int Level;
	
	// IMPLEMENT BUILDER PATTERN
	
	public Base(){
		Guid uuid = Guid.NewGuid();
		this.ID = uuid.ToString();
		this.CurrentBaseOwner = BaseOwner.NONE;
	}
	
	public Base(BaseOwner baseOwner){
		Guid uuid = Guid.NewGuid();
		this.ID = uuid.ToString();
		this.CurrentBaseOwner = baseOwner;
	}
	
	public Base(BaseOwner baseOwner, Vector2 position, string name, int level){
		Guid uuid = Guid.NewGuid();
		this.ID = uuid.ToString();
		this.CurrentBaseOwner = baseOwner;
		this.Position = position;
		this.Name = name;
		this.Level = level;
	}
	
	public enum BaseOwner
	{
		ALLY,
		ENEMY,
		NONE
	}
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
