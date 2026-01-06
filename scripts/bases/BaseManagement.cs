using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class BaseManagement : Control
{
	public string BUTTON_PATH = "BaseManagementPanel/VBoxContainer/";
	public Base _base;
	public Button baseUpgradeBtn;
	public Button troopUpgradeBtn;
	
	public PanelContainer troopUpgradeContainer;
	public RichTextLabel baseUpgradeBtnTxt;
	public RichTextLabel troopUpgradeBtnTxt;

	public override void _Ready()
	{
		_base = GetParent<Base>();
		

		// need to add all of this logic for base upgrade, as well as looping through the values to build up
		// the hbox with the prices. This price definition will need to come from some higher level rule conifg.
		troopUpgradeContainer = GetNode<PanelContainer>(BUTTON_PATH + "TroopUpgradeContainer");

		HBoxContainer hBox = new HBoxContainer();

		var icon = new TextureRect();
		icon.Texture = GD.Load<Texture2D>("res://game/assets/materials/wheat.png");

		var label = new Label();
		label.Text = "wheat";

		hBox.AddChild(icon);
		hBox.AddChild(label);

		troopUpgradeContainer.AddChild(hBox);
	}

	public override void _Process(double delta)
	{
	}
}
