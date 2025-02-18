using System;
using System.IO;
using System.Threading.Tasks;
using ha_modbus_tester.Services;
using ha_modbus_tester.Models;

namespace ha_modbus_tester;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            if (!ValidateArgs(args))
            {
                return 1;
            }

            Console.WriteLine("=== Home Assistant Modbus Configuration Tester ===");
            Console.WriteLine($"Reading configuration from: {args[0]}");

            ModbusHub? config = await ParseConfiguration(args[0]);
            if (config == null)
            {
                return 1;
            }

            Console.WriteLine($"{Environment.NewLine}Modbus Configuration:");
            Console.WriteLine($"Host: {config.Host}");
            Console.WriteLine($"Port: {config.Port}");
            Console.WriteLine($"Number of sensors: {config.Sensors.Count}{Environment.NewLine}");

            await TestModbusConnection(config);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"{Environment.NewLine}Unexpected error: {ex.Message}");
            return 1;
        }
    }

    private static bool ValidateArgs(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Error: Please provide the path to the YAML file as an argument.");
            Console.Error.WriteLine("Usage: HAModbusTools.exe <path-to-yaml-file>");
            return false;
        }

        if (!File.Exists(args[0]))
        {
            Console.Error.WriteLine($"Error: File not found: {args[0]}");
            return false;
        }

        return true;
    }

    private static async Task<ModbusHub?> ParseConfiguration(string yamlPath)
    {
        try
        {
            string yamlContent = await File.ReadAllTextAsync(yamlPath);
            YamlParser parser = new();
            return YamlParser.ParseModbusConfig(yamlContent);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error parsing YAML configuration: {ex.Message}");
            return null;
        }
    }

    private static async Task TestModbusConnection(ModbusHub config)
    {
        Console.WriteLine($"Starting Modbus connection test...{Environment.NewLine}");

        ModbusTester tester = new();
        try
        {
            await tester.TestConnection(config);
            Console.WriteLine($"{Environment.NewLine}Modbus test completed successfully.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"{Environment.NewLine}Modbus test failed: {ex.Message}");
            throw;
        }
    }
}
