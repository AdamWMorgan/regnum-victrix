using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BaseManagement : Control
{
	private double REFRESH_RATE = 0.5;
	private double _timeAccumulator = 0.0;
	public string ASSET_PATH_FORMAT = "res://game/assets/materials/{0}.png";
	public Base _base;
	public PanelContainer baseUpgradeContainer;
	public PanelContainer troopUpgradeContainer;
	private Button baseUpgradeBtn;
	private Button troopUpgradeBtn;
	private GameConfig _gameConfig;
	
	private Color _panelBg = new Color(0.95f, 0.92f, 0.85f, 1); // Cream
	private Color _titleBarBg = new Color(0.8f, 0.7f, 0.5f, 1); // Warm gold
	private Color _accentBaseGreen = new Color(0.6f, 0.8f, 0.5f, 1); // Sage green
	private Color _accentTroopRed = new Color(0.8f, 0.5f, 0.5f, 1); // Warm red
	private Color _enabledBtn = new Color(0.7f, 0.85f, 0.65f, 1); // Light green
	private Color _disabledBtn = new Color(0.7f, 0.7f, 0.7f, 1); // Gray
	private Color _sufficientColour = new Color(0.2f, 0.7f, 0.2f, 1); // Green
	private Color _insufficientColour = new Color(0.9f, 0.2f, 0.2f, 1); // Red
	private Color _costColour = new Color(0.85f, 0.7f, 0.1f, 1); // Gold
	private Color _textDark = new Color(0.15f, 0.15f, 0.15f, 1); // Dark text
	
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
			baseManagementPanel.Visible = false;
			ApplyWindowTheme();
			BaseUpgradeContainerSetup();
			TroopUpgradeContainerSetup();
			
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

	private void ApplyWindowTheme()
	{
		// Main panel styling
		var panelStyle = new StyleBoxFlat
		{
			BgColor = _panelBg,
			BorderColor = _textDark,
			BorderWidthLeft = 2,
			BorderWidthRight = 2,
			BorderWidthTop = 2,
			BorderWidthBottom = 2,
			CornerRadiusTopLeft = 4,
			CornerRadiusTopRight = 4,
			CornerRadiusBottomRight = 4,
			CornerRadiusBottomLeft = 4
		};
		baseManagementPanel.AddThemeStyleboxOverride("panel", panelStyle);

		// Title bar styling
		var titleBar = GetNode<PanelContainer>("BaseManagementPanel/MainContainer/TitleBar");
		var titleStyle = new StyleBoxFlat
		{
			BgColor = _titleBarBg,
			BorderColor = _textDark,
			BorderWidthBottom = 2
		};
		titleBar.AddThemeStyleboxOverride("panel", titleStyle);

		var titleLabel = GetNode<Label>("BaseManagementPanel/MainContainer/TitleBar/TitleLabel");
		titleLabel.AddThemeColorOverride("font_color", _textDark);
	}

	private void OnBaseUpgradeClicked()
	{
		var resourceCosts = new Dictionary<ResourceType, int>();
		
		foreach (var upgrade in _gameConfig.BaseLevelConfigPanel.BaseUpgrade)
		{
			ResourceType type = (ResourceType)Enum.Parse(typeof(ResourceType), upgrade.ResourceType);
			resourceCosts[type] = upgrade.Amount;
		}

		if (_base.SpendResources(resourceCosts))
		{
			_base.LevelUp();
			GD.Print($"Base upgraded to level {_base.Level}");
			BaseUpgradeCheck();
		}
	}	
	
	private void OnTroopUpgradeClicked()
	{
		var resourceCosts = new Dictionary<ResourceType, int>();
		
		foreach (var upgrade in _gameConfig.BaseLevelConfigPanel.TroopUpgrade)
		{
			ResourceType type = (ResourceType)Enum.Parse(typeof(ResourceType), upgrade.ResourceType);
			resourceCosts[type] = upgrade.Amount;
		}

		if (_base.SpendResources(resourceCosts))
		{
			foreach (var unit in _base.Units)
			{
				unit.LevelUp();
			}
			GD.Print($"Troops upgraded");
			TroopUpgradeCheck();
		}
	}

	private int RetrieveResourceQuantity(string resourceType)
	{
		Resource resource = _base.Resources.Find(res => res.Type.ToString() == resourceType);
		return resource != null ? resource.Quantity : 0;
	}

	private void BaseUpgradeContainerSetup()
	{
		baseUpgradeContainer = GetNode<PanelContainer>("BaseManagementPanel/MainContainer/ContentContainer/BaseUpgradeSection/BaseUpgradeContainer");

		// Setup container styling
		var containerStyle = new StyleBoxFlat
		{
			BgColor = new Color(1, 1, 1, 0.9f),
			BorderColor = _accentBaseGreen,
			BorderWidthLeft = 1,
			BorderWidthRight = 1,
			BorderWidthTop = 1,
			BorderWidthBottom = 1
		};
		baseUpgradeContainer.AddThemeStyleboxOverride("panel", containerStyle);

		var mainVBox = new VBoxContainer();
		mainVBox.Alignment = BoxContainer.AlignmentMode.Center;
		mainVBox.AddThemeConstantOverride("separation", 4);

		// Create HBox for resources (icon + text arranged nicely)
		var resourcesHBox = new HBoxContainer();
		resourcesHBox.Alignment = BoxContainer.AlignmentMode.Center;
		resourcesHBox.AddThemeConstantOverride("separation", 8);

		foreach (var baseUpgrade in _gameConfig.BaseLevelConfigPanel.BaseUpgrade)
		{
			if (baseUpgrade.Amount > 0) // Only show if cost > 0
			{
				var itemVBox = new VBoxContainer();
				itemVBox.Alignment = BoxContainer.AlignmentMode.Center;
				itemVBox.AddThemeConstantOverride("separation", 1);

				var icon = new TextureRect
				{
					Texture = GD.Load<Texture2D>(string.Format(ASSET_PATH_FORMAT, baseUpgrade.ResourceType.ToLower())),
					CustomMinimumSize = new Vector2(20, 20),
					ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
					StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
				};

				var costLabel = new Label { Text = baseUpgrade.Amount.ToString() };
				costLabel.AddThemeFontSizeOverride("font_size", 9);
				costLabel.AddThemeColorOverride("font_color", _costColour);
				costLabel.HorizontalAlignment = HorizontalAlignment.Center;

				baseUpgradeLabels.Add(costLabel);

				itemVBox.AddChild(icon);
				itemVBox.AddChild(costLabel);
				resourcesHBox.AddChild(itemVBox);
			}
		}

		mainVBox.AddChild(resourcesHBox);

		// Upgrade button
		baseUpgradeBtn = new Button
		{
			Text = "UPGRADE",
			CustomMinimumSize = new Vector2(140, 18),
			SizeFlagsHorizontal = SizeFlags.ShrinkCenter
		};
		baseUpgradeBtn.Pressed += OnBaseUpgradeClicked;

		var buttonNormalStyle = new StyleBoxFlat
		{
			BgColor = _enabledBtn,
			BorderColor = new Color(0.4f, 0.6f, 0.3f, 1),
			BorderWidthLeft = 1,
			BorderWidthRight = 1,
			BorderWidthTop = 1,
			BorderWidthBottom = 1,
			CornerRadiusTopLeft = 3,
			CornerRadiusTopRight = 3,
			CornerRadiusBottomRight = 3,
			CornerRadiusBottomLeft = 3,
			ContentMarginLeft = 6,
			ContentMarginRight = 6,
			ContentMarginTop = 4,
			ContentMarginBottom = 4
		};

		var buttonDisabledStyle = new StyleBoxFlat
		{
			BgColor = _disabledBtn,
			BorderColor = new Color(0.5f, 0.5f, 0.5f, 1),
			BorderWidthLeft = 1,
			BorderWidthRight = 1,
			BorderWidthTop = 1,
			BorderWidthBottom = 1,
			CornerRadiusTopLeft = 3,
			CornerRadiusTopRight = 3,
			CornerRadiusBottomRight = 3,
			CornerRadiusBottomLeft = 3,
			ContentMarginLeft = 6,
			ContentMarginRight = 6,
			ContentMarginTop = 4,
			ContentMarginBottom = 4
		};

		baseUpgradeBtn.AddThemeStyleboxOverride("normal", buttonNormalStyle);
		baseUpgradeBtn.AddThemeStyleboxOverride("disabled", buttonDisabledStyle);
		baseUpgradeBtn.AddThemeColorOverride("font_color", _textDark);

		mainVBox.AddChild(baseUpgradeBtn);
		baseUpgradeContainer.AddChild(mainVBox);
	}
	
	private void TroopUpgradeContainerSetup()
	{
		troopUpgradeContainer = GetNode<PanelContainer>("BaseManagementPanel/MainContainer/ContentContainer/TroopUpgradeSection/TroopUpgradeContainer");

		// Setup container styling
		var containerStyle = new StyleBoxFlat
		{
			BgColor = new Color(1, 1, 1, 0.9f),
			BorderColor = _accentTroopRed,
			BorderWidthLeft = 1,
			BorderWidthRight = 1,
			BorderWidthTop = 1,
			BorderWidthBottom = 1
		};
		troopUpgradeContainer.AddThemeStyleboxOverride("panel", containerStyle);

		var mainVBox = new VBoxContainer();
		mainVBox.Alignment = BoxContainer.AlignmentMode.Center;
		mainVBox.AddThemeConstantOverride("separation", 4);

		// Create HBox for resources
		var resourcesHBox = new HBoxContainer();
		resourcesHBox.Alignment = BoxContainer.AlignmentMode.Center;
		resourcesHBox.AddThemeConstantOverride("separation", 8);

		foreach (var troopUpgrade in _gameConfig.BaseLevelConfigPanel.TroopUpgrade)
		{
			if (troopUpgrade.Amount > 0)
			{
				var itemVBox = new VBoxContainer();
				itemVBox.Alignment = BoxContainer.AlignmentMode.Center;
				itemVBox.AddThemeConstantOverride("separation", 1);

				var icon = new TextureRect
				{
					Texture = GD.Load<Texture2D>(string.Format(ASSET_PATH_FORMAT, troopUpgrade.ResourceType.ToLower())),
					CustomMinimumSize = new Vector2(20, 20),
					ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
					StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
				};

				var costLabel = new Label { Text = troopUpgrade.Amount.ToString() };
				costLabel.AddThemeFontSizeOverride("font_size", 9);
				costLabel.AddThemeColorOverride("font_color", _costColour);
				costLabel.HorizontalAlignment = HorizontalAlignment.Center;

				troopUpgradeLabels.Add(costLabel);

				itemVBox.AddChild(icon);
				itemVBox.AddChild(costLabel);
				resourcesHBox.AddChild(itemVBox);
			}
		}

		mainVBox.AddChild(resourcesHBox);

		// Upgrade button
		troopUpgradeBtn = new Button
		{
			Text = "UPGRADE",
			CustomMinimumSize = new Vector2(140, 18),
			SizeFlagsHorizontal = SizeFlags.ShrinkCenter
		};
		troopUpgradeBtn.Pressed += OnTroopUpgradeClicked;

		var buttonNormalStyle = new StyleBoxFlat
		{
			BgColor = _accentTroopRed,
			BorderColor = new Color(0.6f, 0.3f, 0.3f, 1),
			BorderWidthLeft = 1,
			BorderWidthRight = 1,
			BorderWidthTop = 1,
			BorderWidthBottom = 1,
			CornerRadiusTopLeft = 3,
			CornerRadiusTopRight = 3,
			CornerRadiusBottomRight = 3,
			CornerRadiusBottomLeft = 3,
			ContentMarginLeft = 6,
			ContentMarginRight = 6,
			ContentMarginTop = 4,
			ContentMarginBottom = 4
		};

		var buttonDisabledStyle = new StyleBoxFlat
		{
			BgColor = _disabledBtn,
			BorderColor = new Color(0.5f, 0.5f, 0.5f, 1),
			BorderWidthLeft = 1,
			BorderWidthRight = 1,
			BorderWidthTop = 1,
			BorderWidthBottom = 1,
			CornerRadiusTopLeft = 3,
			CornerRadiusTopRight = 3,
			CornerRadiusBottomRight = 3,
			CornerRadiusBottomLeft = 3,
			ContentMarginLeft = 6,
			ContentMarginRight = 6,
			ContentMarginTop = 4,
			ContentMarginBottom = 4
		};

		troopUpgradeBtn.AddThemeStyleboxOverride("normal", buttonNormalStyle);
		troopUpgradeBtn.AddThemeStyleboxOverride("disabled", buttonDisabledStyle);
		troopUpgradeBtn.AddThemeColorOverride("font_color", new Color(1, 1, 1, 1));

		mainVBox.AddChild(troopUpgradeBtn);
		troopUpgradeContainer.AddChild(mainVBox);
	}

	private void BaseUpgradeCheck()
	{
		var baseUpgradeReady = true;
		int labelIndex = 0;

		foreach (var baseUpgradeItem in _gameConfig.BaseLevelConfigPanel.BaseUpgrade)
		{
			if (baseUpgradeItem.Amount > 0)
			{
				int availableQuantity = RetrieveResourceQuantity(baseUpgradeItem.ResourceType);
				
				if (labelIndex < baseUpgradeLabels.Count)
				{
					if (availableQuantity >= baseUpgradeItem.Amount)
					{
						baseUpgradeLabels[labelIndex].AddThemeColorOverride("font_color", _sufficientColour);
					}
					else
					{
						baseUpgradeReady = false;
						baseUpgradeLabels[labelIndex].AddThemeColorOverride("font_color", _insufficientColour);
					}
				}
				labelIndex++;
			}
		}

		if (baseUpgradeReady && labelIndex > 0)
		{
			baseUpgradeBtn.Disabled = false;
		}
		else
		{
			baseUpgradeBtn.Disabled = true;
		}
	}

	private void TroopUpgradeCheck()
	{
		var troopUpgradeReady = true;
		int labelIndex = 0;

		foreach (var troopUpgradeItem in _gameConfig.BaseLevelConfigPanel.TroopUpgrade)
		{
			if (troopUpgradeItem.Amount > 0)
			{
				int availableQuantity = RetrieveResourceQuantity(troopUpgradeItem.ResourceType);
				
				if (labelIndex < troopUpgradeLabels.Count)
				{
					if (availableQuantity >= troopUpgradeItem.Amount)
					{
						troopUpgradeLabels[labelIndex].AddThemeColorOverride("font_color", _sufficientColour);
					}
					else
					{
						troopUpgradeReady = false;
						troopUpgradeLabels[labelIndex].AddThemeColorOverride("font_color", _insufficientColour);
					}
				}
				labelIndex++;
			}
		}

		if (troopUpgradeReady && labelIndex > 0)
		{
			troopUpgradeBtn.Disabled = false;
		}
		else
		{
			troopUpgradeBtn.Disabled = true;
		}
	}
}
