namespace Yact.Domain.Entities.Cyclist;

public class Zones
{
    public Zone? Zone1 { get; set; }
    public Zone? Zone2 { get; set; }
    public Zone? Zone3 { get; set; }
    public Zone? Zone4 { get; set; }
    public Zone? Zone5 { get; set; }
    public Zone? Zone6 { get; set; }
    public Zone? Zone7 { get; set; }
}

public class Zone
{
    public int LowLimit { get; set; }
    public int HighLimit { get; set; }
}
