using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract partial class ResourceNode : Node2D
{
	public Resource Resource { get; private set; }
	public int GeneratingCapacity { get; private set; }
	public int SendAmount { get; private set; }
	public ResourceLevel Level { get; private set; } = ResourceLevel.ONE;
	public Base attachedBase;
	// the number at which the resource should be sent to the associated base
	private int DEFAULT_SEND_TRIGGER_CAPACITY = 50;
	private float DEFAULT_GENERATION_SPEED = 1f;
	private float timeSinceLastGen = 0f;
	public Area2D captureArea;
	public Label ownerLabel;

	public ResourceNode(Resource resource)
	{
		this.Resource = resource;
		this.GeneratingCapacity = 1;
		this.SendAmount = DEFAULT_SEND_TRIGGER_CAPACITY;
	}

	public ResourceNode(Resource resource, int generatingCapacity, int sendAmount)
	{
		this.Resource = resource;
		this.GeneratingCapacity = generatingCapacity;
		this.SendAmount = sendAmount;
	}

	public override void _Ready()
	{
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
		captureArea = GetNode<Area2D>("CaptureArea");
		ownerLabel = GetNode<Label>("OwnerLabel");
		ownerLabel.Text = attachedBase.CurrentBaseOwner.ToString();
	}

	public override void _Process(double delta)
	{
		if (timeSinceLastGen > DEFAULT_GENERATION_SPEED)
		{
			Resource.IncrementResourceQuantity(GeneratingCapacity);
			timeSinceLastGen = 0.0f;
		}
		else
		{
			timeSinceLastGen += (float)delta;
		}
		SendResourceToBase();
	}

	public ResourceLevel LevelUp()
	{
		this.Level = LevellingUtil<ResourceLevel>.LevelUp((int)this.Level);
		return this.Level;
	}

	private void SendResourceToBase()
	{
		// assign here so that any additional resource generated whilst 
		// executing if statement is not overwritten
		int currResourceQuantity = Resource.Quantity;

		if (currResourceQuantity >= SendAmount)
		{
			attachedBase.receiveResource(Resource.Type, Resource.Quantity);
			Resource.Quantity -= Resource.Quantity;
		}
	}
}
