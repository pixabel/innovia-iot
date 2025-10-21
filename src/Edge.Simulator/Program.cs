using MQTTnet;
using MQTTnet.Client;
using System.Text.Json;
using System.Text;

var factory = new MqttFactory();
var client = factory.CreateMqttClient();

var options = new MqttClientOptionsBuilder()
    .WithTcpServer("localhost", 1883)
    .Build();

Console.WriteLine("Edge.Simulator starting… connecting to MQTT at localhost:1883");
try
{
    await client.ConnectAsync(options);
    Console.WriteLine("✅ Connected to MQTT broker.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Failed to connect to MQTT broker: {ex.Message}");
    throw;
}

var rand = new Random();
string tenantSlug = "innovia";

//  Lista med alla devices
var devices = new[]
{
    new { Serial = "dev-101", ApiKey = "dev-101-key" },
    new { Serial = "dev-102", ApiKey = "dev-102-key" },
    new { Serial = "dev-103", ApiKey = "dev-103-key" },
    new { Serial = "dev-104", ApiKey = "dev-104-key" },
    new { Serial = "dev-105", ApiKey = "dev-105-key" },
    new { Serial = "dev-106", ApiKey = "dev-106-key" },
    new { Serial = "dev-107", ApiKey = "dev-107-key" },
    new { Serial = "dev-108", ApiKey = "dev-108-key" },
    new { Serial = "dev-109", ApiKey = "dev-109-key" },
    new { Serial = "dev-110", ApiKey = "dev-110-key" }
};


// Loopa över alla devices var 10:e sekund
while (true)
{
    foreach (var device in devices)
    {
        var payload = new
        {
            deviceId = device.Serial, // <- SERIAL, not GUID
            apiKey = device.ApiKey,
            timestamp = DateTimeOffset.UtcNow,
            metrics = new object[]
            {
                new { type = "temperature", value = 21 + rand.NextDouble(), unit = "C" },
                new { type = "co2", value = 900 + rand.Next(0, 700), unit = "ppm" }
            }
        };

        var topic = $"tenants/{tenantSlug}/devices/{device.Serial}/measurements";
        var json = JsonSerializer.Serialize(payload);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.UTF8.GetBytes(json))
            .Build();

        await client.PublishAsync(message);
        Console.WriteLine($"[{DateTimeOffset.UtcNow:o}] Published to '{topic}': {json}");
    }

    await Task.Delay(TimeSpan.FromSeconds(10));
}