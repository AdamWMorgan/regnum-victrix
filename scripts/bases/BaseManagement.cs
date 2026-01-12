using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class BaseManagement : Control
{
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

	public override void _Ready()
	{
		_base = GetParent<Base>();

		baseUpgradeContainer = GetNode<PanelContainer>(BUTTON_PATH + "BaseUpgradeContainer");
		baseHBox = new();

		_gameConfig = GameManager.Instance.GameConfig;

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

	public override void _Process(double delta)
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
			baseUpgradeContainer.Modulate = new Color(1, 1, 1, 1);
		}
		else
		{
			baseUpgradeBtn.Disabled = true;
			baseUpgradeContainer.Modulate = new Color(0.6f, 0.6f, 0.6f, 0.8f);
		}

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
			troopUpgradeContainer.Modulate = new Color(1, 1, 1, 1);
		}
		else
		{
			troopUpgradeBtn.Disabled = true;
			troopUpgradeContainer.Modulate = new Color(0.6f, 0.6f, 0.6f, 0.8f);
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
		return resource.Quantity;
	}
}
