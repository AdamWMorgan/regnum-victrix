using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class BaseManagement : Control
{
	private double REFRESH_RATE = 5.0;
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

	public override void _Ready()
	{
		_base = GetParent<Base>();
		_gameConfig = GameManager.Instance.GameConfig;

		BaseUpgradeContainerSetup();
		TroopUpgradeContainerSetup();

		//Initial check whilst setting up container
		BaseUpgradeCheck();
		TroopUpgradeCheck();
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
		GD.Print("OnBaseUpgradeClicked!");
	}	
	
	private void OnTroopUpgradeClicked()
	{
		GD.Print("OnTroopUpgradeClicked!");
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
		baseHBox = new();

		foreach(var baseUpgrade in _gameConfig.BaseLevelConfigPanel.BaseUpgrade)
		{
			var icon = new TextureRect
			{
				Texture = GD.Load<Texture2D>(string.Format(ASSET_PATH_FORMAT, baseUpgrade.ResourceType.ToLower()))
			};

			var label = new Label
			{
				Text = baseUpgrade.Amount.ToString()
			};

			baseHBox.AddChild(icon);
			baseHBox.AddChild(label);
			baseUpgradeBtn.Text = "";
			baseUpgradeBtn.Flat = true;
			baseUpgradeBtn.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			baseUpgradeBtn.SizeFlagsVertical = SizeFlags.ExpandFill;
			baseUpgradeBtn.MouseFilter = MouseFilterEnum.Stop;

			baseUpgradeBtn.Pressed += OnBaseUpgradeClicked;

			baseUpgradeContainer.AddChild(baseHBox);
			baseUpgradeContainer.AddChild(baseUpgradeBtn);
		}
	}
	
	private void TroopUpgradeContainerSetup()
	{
		troopUpgradeContainer = GetNode<PanelContainer>(BUTTON_PATH + "TroopUpgradeContainer");
		troopHBox = new();

		foreach(var troopUpgrade in _gameConfig.BaseLevelConfigPanel.TroopUpgrade)
		{
			var icon = new TextureRect
			{
				Texture = GD.Load<Texture2D>(string.Format(ASSET_PATH_FORMAT, troopUpgrade.ResourceType.ToLower()))
			};

			var label = new Label
			{
				Text = troopUpgrade.Amount.ToString()
			};

			troopHBox.AddChild(icon);
			troopHBox.AddChild(label);

			troopUpgradeBtn.Text = "";
			troopUpgradeBtn.Flat = true;
			troopUpgradeBtn.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			troopUpgradeBtn.SizeFlagsVertical = SizeFlags.ExpandFill;
			troopUpgradeBtn.MouseFilter = MouseFilterEnum.Stop;

			troopUpgradeBtn.Pressed += OnTroopUpgradeClicked;

			troopUpgradeContainer.AddChild(troopHBox);
			troopUpgradeContainer.AddChild(troopUpgradeBtn);
		}
	}

	private void BaseUpgradeCheck()
	{
		var baseUpgradeReady = false;

		foreach(var baseUpgradeItem in _gameConfig.BaseLevelConfigPanel.BaseUpgrade)
		{
			if(RetrieveResourceQuantity(baseUpgradeItem.ResourceType) >= baseUpgradeItem.Amount)
			{
				baseUpgradeReady = true;
			}
			else
			{
				baseUpgradeReady = false;
				break;
			}
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
		var troopUpgradeReady = false;

		foreach(var troopUpgradeItem in _gameConfig.BaseLevelConfigPanel.TroopUpgrade)
		{
			if(RetrieveResourceQuantity(troopUpgradeItem.ResourceType) >= troopUpgradeItem.Amount)
			{
				troopUpgradeReady = true;
			}
			else
			{
				troopUpgradeReady = false;
				break;
			}
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
