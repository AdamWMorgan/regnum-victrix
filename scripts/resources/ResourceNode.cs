using Godot;
using System;
using System.Collections.Generic;

public abstract partial class ResourceNode : Node2D
{
	public Resource resource {get; private set;}
	public int generatingCapacity {get; private set;}
	public Base attachedBase;
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
	
	public override void _Ready(){
		// Get the position of this ResourceNode
		Vector2 resourceNodePosition = this.GlobalPosition;

		// Initialize the closest base and distance tracker
		Base closestBase = null;
		float closestDistance = float.MaxValue; // Start with a very high distance

		// Iterate through all the bases in the "Bases" group
		foreach (Node node in GetTree().GetNodesInGroup("Bases"))
		{
	 	   if (node is Base baseNode)
	  	  {
	  		  // Calculate the distance between the ResourceNode and the current base
	 		   float distance = resourceNodePosition.DistanceTo(baseNode.GlobalPosition);

	 		   // Check if this base is closer than the current closest
	 		   if (distance < closestDistance)
				{
		 		   closestDistance = distance;
		 		   closestBase = baseNode;
		 	   }
	 	   }
		}
	  // Set the closest base if found
  	  attachedBase = closestBase;
	}
	
	public override void _Process(double delta)
	{
		if(timeSinceLastGen > DEFAULT_GENERATION_SPEED){
			resource.IncrementResourceQuantity(generatingCapacity);
			timeSinceLastGen = 0.0f;
		} else {
			timeSinceLastGen += (float)delta;
		}
	}
}
