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
        if (config.Type != ModbusType.Tcp)
        {
            throw new NotImplementedException($"Modbus type {config.Type} is not yet supported");
        }

        using TcpClient tcpClient = new();
        
        Console.WriteLine($"Connecting to {config.Host}:{config.Port}...");
        
        int timeout = config.Timeout * 1000;

        Task connectTask = tcpClient.ConnectAsync(config.Host, config.Port);
        if (await Task.WhenAny(connectTask, Task.Delay(timeout)) != connectTask)
        {
            throw new TimeoutException($"Connection timeout after {config.Timeout} seconds");
        }

        if (config.Delay > 0)
        {
            Console.WriteLine($"Waiting initial delay of {config.Delay} seconds...");
            await Task.Delay(config.Delay * 1000);
        }
        
        tcpClient.ReceiveTimeout = timeout;
        tcpClient.SendTimeout = timeout;
        
        ModbusFactory factory = new();
        IModbusMaster master = factory.CreateMaster(tcpClient);

        bool isFirstRead = true;
        foreach (ModbusSensor sensor in config.Sensors)
        {
            try
            {
                if (!isFirstRead && config.MessageWaitMilliseconds > 0)
                {
                    Console.WriteLine($"Waiting {config.MessageWaitMilliseconds}ms before next read...");
                    await Task.Delay(config.MessageWaitMilliseconds);
                }
                isFirstRead = false;

                Console.WriteLine($"Testing sensor: {sensor.Name}");

                ushort[] data = await master.ReadHoldingRegistersAsync(
                    (byte)sensor.SlaveId,
                    (ushort)sensor.Address,
                    (ushort)sensor.Count);
                
                Console.WriteLine($"Success! Read {data.Length} registers.");
                Console.WriteLine($"Values: {string.Join(", ", data)}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading sensor {sensor.Name}: {ex.Message}{Environment.NewLine}");
            }
        }
    }
}
