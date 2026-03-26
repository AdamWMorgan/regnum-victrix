using System.Collections.Generic;

public class BaseLevelConfigPanel {
    public float Multiplier { get; set; }
    public float Max { get; set; }
    public List<ResourceCost> TroopUpgrade { get; set; }
    public List<ResourceCost> BaseUpgrade { get; set; }
}