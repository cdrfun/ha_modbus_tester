using NModbus;
using System.Net.Sockets;
using ha_modbus_tester.Models;
using System;
using System.Threading.Tasks;

namespace ha_modbus_tester.Services;

public class ModbusTester
{
    public async Task TestConnection(ModbusHub config)
    {
        using TcpClient tcpClient = new();

        int timeout = config.Timeout * 1000;

        Task connectTask = tcpClient.ConnectAsync(config.Host, config.Port);
        if (await Task.WhenAny(connectTask, Task.Delay(timeout)) != connectTask)
        {
            throw new TimeoutException($"Connection timeout after {config.Timeout} seconds");
        }
        
        tcpClient.ReceiveTimeout = timeout;
        tcpClient.SendTimeout = timeout;
        
        ModbusFactory factory = new();
        IModbusMaster master = factory.CreateMaster(tcpClient);

        foreach (ModbusSensor sensor in config.Sensors)
        {
            try
            {
                Console.WriteLine($"Testing sensor: {sensor.Name}{Environment.NewLine}");

                ushort[] data = await master.ReadHoldingRegistersAsync(
                    (byte)sensor.SlaveId,
                    (ushort)sensor.Address,
                    (ushort)sensor.Count);
                
                Console.WriteLine($"Success! Read {data.Length} registers.{Environment.NewLine}");
                Console.WriteLine($"Values: {string.Join(", ", data)}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading sensor {sensor.Name}: {ex.Message}{Environment.NewLine}");
            }
        }
    }
}
