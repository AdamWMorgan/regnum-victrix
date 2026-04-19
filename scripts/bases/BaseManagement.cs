using Godot;
using System;
using System.Collections.Generic;
using System.Collections; // Required for IEnumerable

public partial class BaseManagement : Control
{
	private double REFRESH_RATE = 0.5;
	private double _timeAccumulator = 0.0;
	public string ASSET_PATH_FORMAT = "res://game/assets/materials/{0}.png";
	public Base _base;
	
	// UI Node References
	public PanelContainer baseUpgradeContainer;
	public PanelContainer troopUpgradeContainer;
	private Button baseUpgradeBtn;
	private Button troopUpgradeBtn;
	private Control baseManagementPanel;
	private GameConfig _gameConfig;

	// --- SOFT ROMAN PALETTE ---
	private Color _colorPapyrus = new Color("#fdf6e3");    
	private Color _colorRomanRed = new Color("#a64d4d");   
	private Color _colorSageGreen = new Color("#7a916b");  
	private Color _colorWoodBorder = new Color("#4a3728"); 
	private Color _colorTextDark = new Color("#2d241e");   
	private Color _colorGold = new Color("#d4af37");       
	private Color _colorSufficient = new Color("#4caf50"); 
	private Color _colorInsufficient = new Color("#e57373");

	private List<Label> baseUpgradeLabels = new();
	private List<Label> troopUpgradeLabels = new();

	public override void _Ready()
	{
		_base = GetParent<Base>();
		_gameConfig = GameManager.Instance.GameConfig;
		baseManagementPanel = GetNode<Control>("BaseManagementPanel");

		if (_base is AllyBase)
		{
			baseManagementPanel.Visible = false;
			ApplyWindowTheme();
			BaseUpgradeContainerSetup();
			TroopUpgradeContainerSetup();
			
			BaseUpgradeCheck();
			TroopUpgradeCheck();
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
		var panelStyle = new StyleBoxFlat
		{
			BgColor = _colorPapyrus,
			BorderColor = _colorWoodBorder,
			BorderWidthLeft = 5, BorderWidthRight = 5, BorderWidthTop = 5, BorderWidthBottom = 5,
			CornerRadiusTopLeft = 10, CornerRadiusTopRight = 10, CornerRadiusBottomRight = 10, CornerRadiusBottomLeft = 10,
			ShadowColor = new Color(0, 0, 0, 0.2f), ShadowSize = 10, ShadowOffset = new Vector2(4, 4)
		};
		baseManagementPanel.AddThemeStyleboxOverride("panel", panelStyle);

		var titleBar = GetNode<PanelContainer>("BaseManagementPanel/MainContainer/TitleBar");
		var titleStyle = new StyleBoxFlat
		{
			BgColor = _colorRomanRed,
			BorderColor = _colorWoodBorder,
			BorderWidthBottom = 3,
			CornerRadiusTopLeft = 6, CornerRadiusTopRight = 6,
			ContentMarginTop = 10, ContentMarginBottom = 10
		};
		titleBar.AddThemeStyleboxOverride("panel", titleStyle);

		var titleLabel = GetNode<Label>("BaseManagementPanel/MainContainer/TitleBar/TitleLabel");
		titleLabel.AddThemeColorOverride("font_color", Colors.White);
		titleLabel.AddThemeFontSizeOverride("font_size", 20);
	}

	private StyleBoxFlat CreateButtonStyle(Color baseColor, bool isPressed = false)
	{
		return new StyleBoxFlat
		{
			BgColor = baseColor,
			BorderColor = _colorWoodBorder,
			BorderWidthLeft = 2, BorderWidthRight = 2, BorderWidthTop = 2,
			BorderWidthBottom = isPressed ? 2 : 5,
			CornerRadiusTopLeft = 5, CornerRadiusTopRight = 5, CornerRadiusBottomRight = 5, CornerRadiusBottomLeft = 5,
			ContentMarginTop = isPressed ? 10 : 8, ContentMarginBottom = 8
		};
	}

	// Changed 'upgrades' to IEnumerable to fix the CS0426 error
	private void SetupUpgradeUI(PanelContainer container, IEnumerable upgrades, List<Label> labelList, ref Button upgradeBtn, Color themeColor, Action clickHandler)
	{
		container.AddThemeStyleboxOverride("panel", new StyleBoxFlat {
			BgColor = new Color(1, 1, 1, 0.3f), BorderColor = themeColor,
			BorderWidthLeft = 2, BorderWidthRight = 2, BorderWidthTop = 2, BorderWidthBottom = 2,
			CornerRadiusTopLeft = 8, CornerRadiusTopRight = 8, CornerRadiusBottomRight = 8, CornerRadiusBottomLeft = 8,
			ContentMarginLeft = 15, ContentMarginRight = 15, ContentMarginTop = 15, ContentMarginBottom = 15
		});
		
		foreach (var child in container.GetChildren()) child.QueueFree();

		var mainVBox = new VBoxContainer();
		mainVBox.AddThemeConstantOverride("separation", 15);

		var resourcesHBox = new HBoxContainer();
		resourcesHBox.Alignment = BoxContainer.AlignmentMode.Center;
		resourcesHBox.AddThemeConstantOverride("separation", 20);

		foreach (dynamic item in upgrades)
		{
			if (item.Amount <= 0) continue;

			var itemVBox = new VBoxContainer();
			var icon = new TextureRect {
				Texture = GD.Load<Texture2D>(string.Format(ASSET_PATH_FORMAT, item.ResourceType.ToLower())),
				CustomMinimumSize = new Vector2(35, 35),
				ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
				StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
			};

			var costLabel = new Label { Text = item.Amount.ToString() };
			costLabel.AddThemeFontSizeOverride("font_size", 16);
			costLabel.AddThemeColorOverride("font_color", _colorGold);
			costLabel.HorizontalAlignment = HorizontalAlignment.Center;
			
			labelList.Add(costLabel);
			itemVBox.AddChild(icon);
			itemVBox.AddChild(costLabel);
			resourcesHBox.AddChild(itemVBox);
		}

		upgradeBtn = new Button { Text = "IMPROVE", CustomMinimumSize = new Vector2(160, 45), SizeFlagsHorizontal = SizeFlags.ShrinkCenter };
		upgradeBtn.AddThemeStyleboxOverride("normal", CreateButtonStyle(themeColor));
		upgradeBtn.AddThemeStyleboxOverride("hover", CreateButtonStyle(themeColor.Lightened(0.1f)));
		upgradeBtn.AddThemeStyleboxOverride("pressed", CreateButtonStyle(themeColor, true));
		upgradeBtn.AddThemeStyleboxOverride("disabled", CreateButtonStyle(new Color(0.6f, 0.6f, 0.6f)));
		upgradeBtn.AddThemeColorOverride("font_color", Colors.White);
		upgradeBtn.Pressed += clickHandler;

		mainVBox.AddChild(resourcesHBox);
		mainVBox.AddChild(upgradeBtn);
		container.AddChild(mainVBox);
	}

	private void BaseUpgradeContainerSetup()
	{
		baseUpgradeContainer = GetNode<PanelContainer>("BaseManagementPanel/MainContainer/ContentContainer/BaseUpgradeSection/BaseUpgradeContainer");
		SetupUpgradeUI(baseUpgradeContainer, _gameConfig.BaseLevelConfigPanel.BaseUpgrade, baseUpgradeLabels, ref baseUpgradeBtn, _colorSageGreen, OnBaseUpgradeClicked);
	}

	private void TroopUpgradeContainerSetup()
	{
		troopUpgradeContainer = GetNode<PanelContainer>("BaseManagementPanel/MainContainer/ContentContainer/TroopUpgradeSection/TroopUpgradeContainer");
		SetupUpgradeUI(troopUpgradeContainer, _gameConfig.BaseLevelConfigPanel.TroopUpgrade, troopUpgradeLabels, ref troopUpgradeBtn, _colorRomanRed, OnTroopUpgradeClicked);
	}

	private void BaseUpgradeCheck()
	{
		if (baseUpgradeBtn == null) return;
		bool ready = true; int i = 0;
		foreach (dynamic item in _gameConfig.BaseLevelConfigPanel.BaseUpgrade) {
			if (item.Amount > 0 && i < baseUpgradeLabels.Count) {
				bool enough = RetrieveResourceQuantity(item.ResourceType) >= item.Amount;
				baseUpgradeLabels[i].AddThemeColorOverride("font_color", enough ? _colorSufficient : _colorInsufficient);
				if (!enough) ready = false;
				i++;
			}
		}
		baseUpgradeBtn.Disabled = !ready || i == 0;
	}

	private void TroopUpgradeCheck()
	{
		if (troopUpgradeBtn == null) return;
		bool ready = true; int i = 0;
		foreach (dynamic item in _gameConfig.BaseLevelConfigPanel.TroopUpgrade) {
			if (item.Amount > 0 && i < troopUpgradeLabels.Count) {
				bool enough = RetrieveResourceQuantity(item.ResourceType) >= item.Amount;
				troopUpgradeLabels[i].AddThemeColorOverride("font_color", enough ? _colorSufficient : _colorInsufficient);
				if (!enough) ready = false;
				i++;
			}
		}
		troopUpgradeBtn.Disabled = !ready || i == 0;
	}

	private void OnBaseUpgradeClicked()
	{
		var costs = new Dictionary<ResourceType, int>();
		foreach (dynamic u in _gameConfig.BaseLevelConfigPanel.BaseUpgrade)
			costs[(ResourceType)Enum.Parse(typeof(ResourceType), u.ResourceType)] = u.Amount;
		if (_base.SpendResources(costs)) { _base.LevelUp(); BaseUpgradeCheck(); }
	}

	private void OnTroopUpgradeClicked()
	{
		var costs = new Dictionary<ResourceType, int>();
		foreach (dynamic u in _gameConfig.BaseLevelConfigPanel.TroopUpgrade)
			costs[(ResourceType)Enum.Parse(typeof(ResourceType), u.ResourceType)] = u.Amount;
		if (_base.SpendResources(costs)) { foreach (var unit in _base.Units) unit.LevelUp(); TroopUpgradeCheck(); }
	}

	private int RetrieveResourceQuantity(string resourceType)
	{
		Resource res = _base.Resources.Find(r => r.Type.ToString() == resourceType);
		return res != null ? res.Quantity : 0;
	}
}
