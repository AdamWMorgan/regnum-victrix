using Godot;
using System;

public partial class BaseManagement : Control
{
	public String BUTTON_PATH = "BaseManagementPanel/VBoxContainer/";
	public Base _base;
	public Button baseUpgradeBtn;
	public Button troopUpgradeBtn;

	public override void _Ready()
	{
		_base = GetParent<Base>();
		baseUpgradeBtn = GetNode<Button>( BUTTON_PATH + "BaseUpgradeBtn");
		troopUpgradeBtn = GetNode<Button>(BUTTON_PATH + "TroopUpgradeBtn");

		baseUpgradeBtn.Text = "base upgrade values placeholder";
		troopUpgradeBtn.Text = "base upgrade values placeholder";
	}

	public override void _Process(double delta)
	{
	}
}
