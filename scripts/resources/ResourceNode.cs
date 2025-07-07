using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract partial class ResourceNode : Node2D
{
	[Export] public CaptureProgress captureProgress;
	public string ID { get; private set; }
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
	private int capturingUnits = 0;

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
		ID = Guid.NewGuid().ToString();
		attachedBase = CalculateClosestBase();
		captureArea = GetNode<Area2D>("CaptureArea");
		// Connect the body_entered and body_exited signals to the methods
		captureArea.BodyEntered += OnBodyEnteredCaptureArea;
		captureArea.BodyExited += OnBodyExitedCaptureArea;
		ownerLabel = GetNode<Label>("OwnerLabel");
		ownerLabel.Text = attachedBase.CurrentBaseOwner.ToString();
		// initial setting of resource node ownership
		currentOwner = attachedBase.CurrentBaseOwner;
		SetProgressBar();
		GameManager.Instance.RegisterResourceNode(this);
	}

	public override void _Process(double delta)
	{
		if (capturingUnits != 0) { captureInProgress = true; } else { captureInProgress = false; }

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
			if (captureProgress.CurrentCaptureProgess == 0)
			{
				captureInProgress = false;
				capturingUnits = 0;
				SwitchOwnership(currentOwner == Faction.ENEMY ? Faction.ALLY : Faction.ENEMY);
			}

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
		// TODO: Bug here somewhere where an ally can capture the base for the enemy?!
		if ((body.IsInGroup("Player") || body.IsInGroup("Ally")) && currentOwner != Faction.ALLY)
		{
			capturingUnits++;
		}
		else if (body.IsInGroup("Enemy") && currentOwner != Faction.ENEMY)
		{
			capturingUnits++;
		}
	}

	// Called when a body exits the Area2D
	private void OnBodyExitedCaptureArea(Node body)
	{
		// TODO: Bug here somewhere where an ally can capture the base for the enemy?!
		if ((body.IsInGroup("Player") || body.IsInGroup("Ally")) && currentOwner != Faction.ALLY)
		{
			capturingUnits--;
		}
		else if (body.IsInGroup("Enemy") && currentOwner != Faction.ENEMY)
		{
			capturingUnits--;
		}
	}

	public void SwitchOwnership(Faction newOwner)
	{
		// todo: also need to switch the resource nodes ownership to the nearest capturers base
		if (newOwner == Faction.ALLY)
		{
			GD.Print("Switch to ally");
			currentOwner = Faction.ALLY;
			UpdateStyleAfterCapture(ColourPalette.ALLY.ToColor());
			attachedBase = CalculateClosestBase(Faction.ALLY);
		}
		else
		{
			GD.Print("Switch to enemy");
			currentOwner = Faction.ENEMY;
			UpdateStyleAfterCapture(ColourPalette.ENEMY.ToColor());
			attachedBase = CalculateClosestBase(Faction.ENEMY);
		}
		ownerLabel.Text = currentOwner.ToString();
	}

	private void UpdateStyleAfterCapture(Color colour)
	{
		var style = new StyleBoxFlat
		{
			BgColor = colour
		};
		captureProgress.ColourChange(style);
		captureProgress.ResetCapturePoint();
	}

	// Returns closest base by distance. Can filter by particular faction if necessary by passing Faction enum value as param.
	private Base CalculateClosestBase(Faction? faction = null)
	{
		// Initialize the closest base and distance tracker
		Base closestBase = null;
		float closestDistance = float.MaxValue; // Start with a very high distance

		// Iterate through all the bases in the "Bases" group
		// todo: should be able to get this from GameManager
		foreach (Node node in GetTree().GetNodesInGroup("Bases"))
		{
			if (node is Base baseNode)
			{
				if (faction == null || baseNode.CurrentBaseOwner == faction)
				{
					// Calculate the distance between the ResourceNode and the current base
					float distance = this.GlobalPosition.DistanceTo(baseNode.GlobalPosition);

					// Check if this base is closer than the current closest
					if (distance < closestDistance)
					{
						closestDistance = distance;
						closestBase = baseNode;
					}
				}
			}
		}
		return closestBase;
	}
}
