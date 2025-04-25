using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract partial class ResourceNode : Node2D
{
	[Export] public CaptureProgress captureProgress;
	public Resource Resource { get; private set; }
	public int GeneratingCapacity { get; private set; }
	public int SendAmount { get; private set; }
	public ResourceLevel Level { get; private set; } = ResourceLevel.ONE;
	public Base attachedBase;
	public Faction currentOwner;
	// the number at which the resource should be sent to the associated base
	private int DEFAULT_SEND_TRIGGER_CAPACITY = 50;
	private float DEFAULT_GENERATION_SPEED = 1f;
	private float timeSinceLastGen = 0f;
	public Area2D captureArea;
	public Label ownerLabel;
	private bool captureInProgress = false;
	private float CAPTURE_SPEED = 1f;
	private float timeSinceLastCaptureDeplete = 0f;

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
		// Connect the body_entered and body_exited signals to the methods
		captureArea.BodyEntered += OnBodyEnteredCaptureArea;
		captureArea.BodyExited += OnBodyExitedCaptureArea;
		ownerLabel = GetNode<Label>("OwnerLabel");
		ownerLabel.Text = attachedBase.CurrentBaseOwner.ToString();
		// initial setting of resource node ownership
		currentOwner = attachedBase.CurrentBaseOwner;
		SetProgressBar();
	}

	public override void _Process(double delta)
	{
		if (!captureInProgress)
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
		else
		{
			if (timeSinceLastCaptureDeplete > CAPTURE_SPEED)
			{
				captureProgress.Decrease(10);
				timeSinceLastCaptureDeplete = 0.0f;
			}
			else
			{
				timeSinceLastCaptureDeplete += (float)delta;
			}
		}
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
			attachedBase.ReceiveResource(Resource.Type, Resource.Quantity);
			Resource.Quantity -= Resource.Quantity;
		}
	}

	private void SetProgressBar()
	{
		var colour = ColourPalette.NEUTRAL.ToColor();

		if (attachedBase.CurrentBaseOwner == Faction.ALLY)
		{
			colour = ColourPalette.ALLY.ToColor();
		}
		else if (attachedBase.CurrentBaseOwner == Faction.ENEMY)
		{
			colour = ColourPalette.ENEMY.ToColor();
		}

		var style = new StyleBoxFlat
		{
			BgColor = colour
		};

		captureProgress.ColourChange(style);
	}

	// Called when a body enters the Area2D
	private void OnBodyEnteredCaptureArea(Node body)
	{
		if ((body.IsInGroup("Player") || body.IsInGroup("Ally")) && currentOwner != Faction.ALLY)
		{
			captureInProgress = true;
		}
		if (body.IsInGroup("Enemy") && currentOwner != Faction.ENEMY)
		{
			captureInProgress = true;
		}
	}

	// Called when a body exits the Area2D
	private void OnBodyExitedCaptureArea(Node body)
	{
		if ((body.IsInGroup("Player") || body.IsInGroup("Ally")) && currentOwner != Faction.ALLY)
		{
			captureInProgress = false;
		}
		if (body.IsInGroup("Enemy") && currentOwner != Faction.ENEMY)
		{
			captureInProgress = false;
		}
	}
}
