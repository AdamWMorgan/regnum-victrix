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

	public override void _Ready()
	{
		_base = GetParent<Base>();
		
		baseUpgradeContainer = GetNode<PanelContainer>(BUTTON_PATH + "BaseUpgradeContainer");
		HBoxContainer baseHBox = new();

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

			baseUpgradeContainer.AddChild(baseHBox);
		}


		// need to add all of this logic for base upgrade, as well as looping through the values to build up
		// the hbox with the prices. This price definition will need to come from some higher level rule conifg.
		troopUpgradeContainer = GetNode<PanelContainer>(BUTTON_PATH + "TroopUpgradeContainer");
		HBoxContainer troopHBox = new();

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

			troopUpgradeContainer.AddChild(troopHBox);
		}
	}

	public override void _Process(double delta)
	{
	}
}
