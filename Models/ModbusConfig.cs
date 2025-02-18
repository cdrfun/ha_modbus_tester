using System.Collections.Generic;

namespace HAModbusTools.Models;

public class ModbusHub(string host)
{
    public string? Name { get; set; }
    public string Host { get; set; } = host;
    public int Port { get; set; }
    public int Timeout { get; set; } = 5;  // Default timeout 5 seconds
    public List<ModbusSensor> Sensors { get; set; } = new();
}

public class ModbusSensor
{
    public string? Name { get; set; }
    public int SlaveId { get; set; }
    public int Address { get; set; }
    public string? ScanInterval { get; set; }
    public int Count { get; set; }
    public string? DataType { get; set; }
    public string? InputType { get; set; }
}
