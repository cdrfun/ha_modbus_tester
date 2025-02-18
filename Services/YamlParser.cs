using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using ha_modbus_tester.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ha_modbus_tester.Services;

public class YamlParser
{
    public static ModbusHub ParseModbusConfig(string yamlContent)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        Dictionary<string, object> config = deserializer.Deserialize<Dictionary<string, object>>(yamlContent);
        
        if (!config.TryGetValue("modbus", out object? value))
        {
            throw new Exception("No modbus configuration found in YAML file");
        }

        List<object> modbusList = (List<object>)value;
        Dictionary<object, object> firstModbusConfig = (Dictionary<object, object>)modbusList[0];

        string host = firstModbusConfig["host"]?.ToString() 
            ?? throw new Exception("Host is required in modbus configuration");

        ModbusHub modbusHub = new(host)
        {
            Name = firstModbusConfig["name"]?.ToString(),
            Port = int.Parse(firstModbusConfig["port"]?.ToString() ?? "502"),
            Timeout = int.Parse(firstModbusConfig["timeout"]?.ToString() ?? "5")
        };

        if (firstModbusConfig.ContainsKey("sensors"))
        {
            List<object> sensors = (List<object>)firstModbusConfig["sensors"];
            foreach (Dictionary<object, object> sensor in sensors.Cast<Dictionary<object, object>>())
            {
                modbusHub.Sensors.Add(new ModbusSensor
                {
                    Name = sensor["name"]?.ToString(),
                    SlaveId = int.Parse(sensor["slave"]?.ToString() ?? "1"),
                    Address = int.Parse(sensor["address"]?.ToString() ?? "0"),
                    InputType = sensor["input_type"]?.ToString(),
                    DataType = sensor["data_type"]?.ToString()
                });
            }
        }

        return modbusHub;
    }
}
