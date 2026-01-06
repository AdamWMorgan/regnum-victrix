using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class BaseManagement : Control
{
	public string BUTTON_PATH = "BaseManagementPanel/VBoxContainer/";
	public Base _base;
	public PanelContainer baseUpgradeContainer;
	
	public PanelContainer troopUpgradeContainer;

	public override void _Ready()
	{
		_base = GetParent<Base>();
		
		baseUpgradeContainer = GetNode<PanelContainer>(BUTTON_PATH + "BaseUpgradeContainer");

		HBoxContainer baseHBox = new HBoxContainer();

		var icon = new TextureRect();
		icon.Texture = GD.Load<Texture2D>("res://game/assets/materials/wheat.png");

		var label = new Label();
		label.Text = "100";

		baseHBox.AddChild(icon);
		baseHBox.AddChild(label);

		baseUpgradeContainer.AddChild(baseHBox);

		// need to add all of this logic for base upgrade, as well as looping through the values to build up
		// the hbox with the prices. This price definition will need to come from some higher level rule conifg.
		troopUpgradeContainer = GetNode<PanelContainer>(BUTTON_PATH + "TroopUpgradeContainer");

		HBoxContainer troopHBox = new HBoxContainer();
		var icon2 = new TextureRect();
		icon2.Texture = GD.Load<Texture2D>("res://game/assets/materials/wheat.png");

		var label2 = new Label();
		label2.Text = "300";
		troopHBox.AddChild(icon2);
		troopHBox.AddChild(label2);

		troopUpgradeContainer.AddChild(troopHBox);
	}

	public override void _Process(double delta)
	{
	}
}
