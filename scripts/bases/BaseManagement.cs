using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BaseManagement : Control
{
	private double REFRESH_RATE = 0.5;
	private double _timeAccumulator = 0.0;
	public string BUTTON_PATH = "BaseManagementPanel/VBoxContainer/";
	public string ASSET_PATH_FORMAT = "res://game/assets/materials/{0}.png";
	public Base _base;
	public PanelContainer baseUpgradeContainer;
	
	public PanelContainer troopUpgradeContainer;
	private HBoxContainer baseHBox;
	private HBoxContainer troopHBox;
	private Button baseUpgradeBtn = new();
	private Button troopUpgradeBtn = new();
	private GameConfig _gameConfig;
	private Color _enableColour = new(1, 1, 1, 1);
	private Color _disableColour = new(0.6f, 0.6f, 0.6f, 0.8f);
	private Color _insufficientColour = new(1, 0, 0, 1); // Red for insufficient resources
	
	// Track labels for updating colors dynamically
	private List<Label> baseUpgradeLabels = new();
	private List<Label> troopUpgradeLabels = new();
	private Control baseManagementPanel;

	public override void _Ready()
	{
		_base = GetParent<Base>();
		_gameConfig = GameManager.Instance.GameConfig;
		
		baseManagementPanel = GetNode<Control>("BaseManagementPanel");
		
		// Only show for Ally bases
		if (_base is AllyBase)
		{
			baseManagementPanel.Visible = true;
			BaseUpgradeContainerSetup();
			TroopUpgradeContainerSetup();

			//Initial check whilst setting up container
			BaseUpgradeCheck();
			TroopUpgradeCheck();
		}
		else
		{
			baseManagementPanel.Visible = false;
		}
	}

	public override void _Process(double delta)
	{	
		_timeAccumulator += delta;

		if (_timeAccumulator >= REFRESH_RATE)
		{
			_timeAccumulator = 0;
			BaseUpgradeCheck();
			TroopUpgradeCheck();
		}
	}

	private void OnBaseUpgradeClicked()
	{
		var resourceCosts = new Dictionary<ResourceType, int>();
		
		// Convert config to dictionary
		foreach (var upgrade in _gameConfig.BaseLevelConfigPanel.BaseUpgrade)
		{
			ResourceType type = (ResourceType)Enum.Parse(typeof(ResourceType), upgrade.ResourceType);
			resourceCosts[type] = upgrade.Amount;
		}

		// Try to spend resources
		if (_base.SpendResources(resourceCosts))
		{
			_base.LevelUp();
			GD.Print($"Base upgraded to level {_base.Level}");
		}
	}	
	
	private void OnTroopUpgradeClicked()
	{
		var resourceCosts = new Dictionary<ResourceType, int>();
		
		// Convert config to dictionary
		foreach (var upgrade in _gameConfig.BaseLevelConfigPanel.TroopUpgrade)
		{
			ResourceType type = (ResourceType)Enum.Parse(typeof(ResourceType), upgrade.ResourceType);
			resourceCosts[type] = upgrade.Amount;
		}

		// Try to spend resources
		if (_base.SpendResources(resourceCosts))
		{
			// Level up all units at this base
			foreach (var unit in _base.Units)
			{
				unit.LevelUp();
			}
			GD.Print($"Troops upgraded");
		}
	}

	// Todo: resource type should really be mapped to enum in model
	private int RetrieveResourceQuantity(string resourceType)
	{
		Resource resource = _base.Resources.Find(res => res.Type.ToString() == resourceType);
		return resource != null ? resource.Quantity : 0;
	}

	private void BaseUpgradeContainerSetup()
	{
		baseUpgradeContainer = GetNode<PanelContainer>(BUTTON_PATH + "BaseUpgradeContainer");
		baseHBox = new HBoxContainer();

		foreach(var baseUpgrade in _gameConfig.BaseLevelConfigPanel.BaseUpgrade)
		{
			var icon = new TextureRect
			{
				Texture = GD.Load<Texture2D>(string.Format(ASSET_PATH_FORMAT, baseUpgrade.ResourceType.ToLower())),
				CustomMinimumSize = new Vector2(32, 32)
			};

			var label = new Label
			{
				Text = baseUpgrade.Amount.ToString()
			};
			
			baseUpgradeLabels.Add(label);

			baseHBox.AddChild(icon);
			baseHBox.AddChild(label);
		}
		
		baseUpgradeBtn.Text = "";
		baseUpgradeBtn.Flat = true;
		baseUpgradeBtn.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		baseUpgradeBtn.SizeFlagsVertical = SizeFlags.ExpandFill;
		baseUpgradeBtn.MouseFilter = MouseFilterEnum.Stop;

		baseUpgradeBtn.Pressed += OnBaseUpgradeClicked;

		baseUpgradeContainer.AddChild(baseHBox);
		baseUpgradeContainer.AddChild(baseUpgradeBtn);
	}
	
	private void TroopUpgradeContainerSetup()
	{
		troopUpgradeContainer = GetNode<PanelContainer>(BUTTON_PATH + "TroopUpgradeContainer");
		troopHBox = new HBoxContainer();

		foreach(var troopUpgrade in _gameConfig.BaseLevelConfigPanel.TroopUpgrade)
		{
			var icon = new TextureRect
			{
				Texture = GD.Load<Texture2D>(string.Format(ASSET_PATH_FORMAT, troopUpgrade.ResourceType.ToLower())),
				CustomMinimumSize = new Vector2(32, 32)
			};

			var label = new Label
			{
				Text = troopUpgrade.Amount.ToString()
			};
			
			troopUpgradeLabels.Add(label);

			troopHBox.AddChild(icon);
			troopHBox.AddChild(label);
		}

		troopUpgradeBtn.Text = "";
		troopUpgradeBtn.Flat = true;
		troopUpgradeBtn.SizeFlagsHorizontal = SizeFlags.ExpandFill;
		troopUpgradeBtn.SizeFlagsVertical = SizeFlags.ExpandFill;
		troopUpgradeBtn.MouseFilter = MouseFilterEnum.Stop;

		troopUpgradeBtn.Pressed += OnTroopUpgradeClicked;

		troopUpgradeContainer.AddChild(troopHBox);
		troopUpgradeContainer.AddChild(troopUpgradeBtn);
	}

	private void BaseUpgradeCheck()
	{
		var baseUpgradeReady = true;
		int labelIndex = 0;

		foreach(var baseUpgradeItem in _gameConfig.BaseLevelConfigPanel.BaseUpgrade)
		{
			int availableQuantity = RetrieveResourceQuantity(baseUpgradeItem.ResourceType);
			
			if(availableQuantity >= baseUpgradeItem.Amount)
			{
				// Sufficient resources - use default color
				if (labelIndex < baseUpgradeLabels.Count)
				{
					baseUpgradeLabels[labelIndex].AddThemeColorOverride("font_color", new Color(1, 1, 1, 1));
				}
			}
			else
			{
				// Insufficient resources - use red
				baseUpgradeReady = false;
				if (labelIndex < baseUpgradeLabels.Count)
				{
					baseUpgradeLabels[labelIndex].AddThemeColorOverride("font_color", _insufficientColour);
				}
			}
			labelIndex++;
		}

		if (baseUpgradeReady)
		{
			baseUpgradeBtn.Disabled = false;
			baseUpgradeContainer.Modulate = _enableColour;
		}
		else
		{
			baseUpgradeBtn.Disabled = true;
			baseUpgradeContainer.Modulate = _disableColour;
		}
	}

	private void TroopUpgradeCheck()
	{
		var troopUpgradeReady = true;
		int labelIndex = 0;

		foreach(var troopUpgradeItem in _gameConfig.BaseLevelConfigPanel.TroopUpgrade)
		{
			int availableQuantity = RetrieveResourceQuantity(troopUpgradeItem.ResourceType);
			
			if(availableQuantity >= troopUpgradeItem.Amount)
			{
				// Sufficient resources - use default color
				if (labelIndex < troopUpgradeLabels.Count)
				{
					troopUpgradeLabels[labelIndex].AddThemeColorOverride("font_color", new Color(1, 1, 1, 1));
				}
			}
			else
			{
				// Insufficient resources - use red
				troopUpgradeReady = false;
				if (labelIndex < troopUpgradeLabels.Count)
				{
					troopUpgradeLabels[labelIndex].AddThemeColorOverride("font_color", _insufficientColour);
				}
			}
			labelIndex++;
		}

		if (troopUpgradeReady)
		{
			troopUpgradeBtn.Disabled = false;
			troopUpgradeContainer.Modulate = _enableColour;
		}
		else
		{
			troopUpgradeBtn.Disabled = true;
			troopUpgradeContainer.Modulate = _disableColour;
		}
	}
}
