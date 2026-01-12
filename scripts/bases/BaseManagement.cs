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

	public override void _Ready()
	{
		_base = GetParent<Base>();

		GD.Print(_base.Level);
		
		baseUpgradeContainer = GetNode<PanelContainer>(BUTTON_PATH + "BaseUpgradeContainer");
		baseHBox = new();

		var GameConfig = GameManager.Instance.GameConfig;

		foreach(var baseUpgrade in GameConfig.BaseLevelConfigPanel.BaseUpgrade)
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
			var baseUpgradeBtn = new Button();
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

		foreach(var troopUpgrade in GameConfig.BaseLevelConfigPanel.TroopUpgrade)
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

			var troopUpgradeBtn = new Button();
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
	}

	private void OnBaseUpgradeClicked()
	{
		GD.Print("OnBaseUpgradeClicked!");
	}	
	
	private void OnTroopUpgradeClicked()
	{
		GD.Print("OnTroopUpgradeClicked!");
	}
}
