using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using HAModbusTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HAModbusTools.Services;

public class YamlParser
{
    public ModbusHub ParseModbusConfig(string yamlContent)
    {
        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        Dictionary<string, object> config = deserializer.Deserialize<Dictionary<string, object>>(yamlContent);
        
        if (!config.ContainsKey("modbus"))
            throw new Exception("No modbus configuration found in YAML file");

        List<object> modbusList = (List<object>)config["modbus"];
        Dictionary<object, object> firstModbusConfig = (Dictionary<object, object>)modbusList[0];

        string host = firstModbusConfig["host"]?.ToString() 
            ?? throw new Exception("Host is required in modbus configuration");

        ModbusHub modbusHub = new ModbusHub(host)
        {
            Name = firstModbusConfig["name"]?.ToString(),
            Port = int.Parse(firstModbusConfig["port"]?.ToString() ?? "502"),
            Timeout = int.Parse(firstModbusConfig["timeout"]?.ToString() ?? "5")
        };

        if (firstModbusConfig.ContainsKey("sensors"))
        {
            List<object> sensors = (List<object>)firstModbusConfig["sensors"];
            foreach (Dictionary<object, object> sensor in sensors)
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
