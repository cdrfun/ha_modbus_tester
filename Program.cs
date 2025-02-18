using System;
using System.IO;
using System.Threading.Tasks;
using HAModbusTools.Services;
using HAModbusTools.Models;

namespace HAModbusTools;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            if (!ValidateArgs(args))
                return 1;

            Console.WriteLine("=== Home Assistant Modbus Configuration Tester ===");
            Console.WriteLine($"Reading configuration from: {args[0]}");

            ModbusHub? config = await ParseConfiguration(args[0]);
            if (config == null)
                return 1;

            Console.WriteLine($"\nModbus Configuration:");
            Console.WriteLine($"Host: {config.Host}");
            Console.WriteLine($"Port: {config.Port}");
            Console.WriteLine($"Number of sensors: {config.Sensors.Count}\n");

            await TestModbusConnection(config);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"\nUnexpected error: {ex.Message}");
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
            YamlParser parser = new YamlParser();
            return parser.ParseModbusConfig(yamlContent);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error parsing YAML configuration: {ex.Message}");
            return null;
        }
    }

    private static async Task TestModbusConnection(ModbusHub config)
    {
        Console.WriteLine("Starting Modbus connection test...\n");

        ModbusTester tester = new ModbusTester();
        try
        {
            await tester.TestConnection(config);
            Console.WriteLine("\nModbus test completed successfully.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"\nModbus test failed: {ex.Message}");
            throw;
        }
    }
}
