using System.Collections.Generic;

namespace ha_modbus_tester.Models;

public enum ModbusType
{
    Tcp,            // TCP/IP connection with socket framer
    Udp,            // UDP connection with socket framer
    RtuOverTcp,     // TCP/IP connection with RTU framer
    Serial          // Serial connection with RTU framer
}

public class ModbusHub(string host)
{
    public string? Name { get; set; }
    public string Host { get; set; } = host;
    public int Port { get; set; } = 502;  // Default port
    public int Timeout { get; set; } = 5;  // Default timeout 5 seconds
    public int Delay { get; set; } = 0;  // Default delay 0 seconds
    public int MessageWaitMilliseconds { get; set; } = 0;  // Delay between requests in milliseconds
    public ModbusType Type { get; set; } = ModbusType.Tcp;
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
