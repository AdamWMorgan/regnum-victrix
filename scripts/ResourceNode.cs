using Godot;
using System;

public abstract partial class ResourceNode : Node
{
	public Resource resource {get; private set;}
	public int generatingCapacity {get; private set;}
	private float DEFAULT_GENERATION_SPEED = 1f;
	private float timeSinceLastGen = 0f;
	
	public ResourceNode(Resource resource){
		this.resource = resource;
		this.generatingCapacity = 1;
	}
	
	public ResourceNode(Resource resource, int generatingCapacity){
		this.resource = resource;	
		this.generatingCapacity = generatingCapacity;
	}
	
	public override void _Process(double delta)
	{
		if(timeSinceLastGen > DEFAULT_GENERATION_SPEED){
			resource.IncrementResourceQuantity(generatingCapacity);
			timeSinceLastGen = 0.0f;
		} else {
			timeSinceLastGen += (float)delta;
		}
		GD.Print(resource.Type + " " + resource.Quantity);
	}
}
