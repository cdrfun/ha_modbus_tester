# 🏠 Home Assistant Modbus Tester 🔧

A simple command-line tool to test Modbus configurations for Home Assistant. Written in C# because... why not? 😄

## 🤔 Why C#?

Yes, I know Home Assistant is written in Python, and a Python tool would make more sense here. But this is a personal project created for:
- 🎮 Fun with programming
- 📚 Learning more about C#
- 🔍 Exploring Modbus communication
- 🛠️ Getting hands-on experience with YAML parsing

Sometimes it's not about choosing the most obvious tool, but about enjoying the journey! 🚀

## ✨ Features

- 📝 Parses Home Assistant's Modbus YAML configuration
- 🌐 Tests Modbus TCP connections
- 🔄 Reads sensor values
- ⏱️ Configurable timeouts
- 🎯 Simple and straightforward usage

## 🚀 Getting Started

1. Build the project:
```bash
dotnet build
```

2. Run the tester:
```bash
dotnet run -- path/to/your/configuration.yaml
```

## 📋 Example Configuration

```yaml
modbus:
  - name: MyModbusDevice
    host: 192.168.1.100
    port: 502
    timeout: 5
    sensors:
      - name: Temperature
        slave: 1
        address: 100
        input_type: holding
        data_type: int16
```

## 🔧 Requirements

- .NET 9.0 SDK
- A valid Home Assistant Modbus configuration
- A Modbus TCP device to test against

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

This is a personal project, but feel free to:
- 🐛 Report bugs
- 💡 Suggest improvements
- 🌟 Star the repo if you find it useful

## 📝 Note

This is not an official Home Assistant tool. It's just a fun side project to help test Modbus configurations! 🎈
