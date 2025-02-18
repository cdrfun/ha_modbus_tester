using NModbus;
using System.Net.Sockets;
using HAModbusTools.Models;
using System;
using System.Threading.Tasks;

namespace HAModbusTools.Services;

public class ModbusTester
{
    public async Task TestConnection(ModbusHub config)
    {
        using TcpClient tcpClient = new TcpClient();
        
        var connectTask = tcpClient.ConnectAsync(config.Host, config.Port);
        if (await Task.WhenAny(connectTask, Task.Delay(config.Timeout * 1000)) != connectTask)
        {
            throw new TimeoutException($"Connection timeout after {config.Timeout} seconds");
        }
        
        tcpClient.ReceiveTimeout = config.Timeout * 1000;
        tcpClient.SendTimeout = config.Timeout * 1000;
        
        ModbusFactory factory = new ModbusFactory();
        IModbusMaster master = factory.CreateMaster(tcpClient);

        foreach (ModbusSensor sensor in config.Sensors)
        {
            try
            {
                Console.WriteLine($"Testing sensor: {sensor.Name}");

                ushort[] data = await master.ReadHoldingRegistersAsync(
                    (byte)sensor.SlaveId,
                    (ushort)sensor.Address,
                    (ushort)sensor.Count);
                
                Console.WriteLine($"Success! Read {data.Length} registers.");
                Console.WriteLine($"Values: {string.Join(", ", data)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading sensor {sensor.Name}: {ex.Message}");
            }
        }
    }
}
