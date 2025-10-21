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
string tenantId = "ffdc3cc6-4bb5-41cc-a403-80cc927c43ab";

//  Lista med alla devices
var devices = new[]
{
    new { Id = "3fa85f64-5717-4562-b3fc-2c963f66afa6", ApiKey = "dev-101-key" },
    new { Id = "9a84e956-560d-4530-9d0c-c95d5d2ec1eb", ApiKey = "dev-102-key" },
    new { Id = "e2f6d83b-a61b-4766-9832-d954244549b8", ApiKey = "dev-103-key" },
    new { Id = "516796e3-6304-42d2-adb4-ff1d8b480d70", ApiKey = "dev-104-key" },
    new { Id = "0a6c18bb-7cc3-4a0c-a481-157aa43534d1", ApiKey = "dev-105-key" },
    new { Id = "afd73d89-5053-4f0a-b94f-df05b9289da0", ApiKey = "dev-106-key" },
    new { Id = "6b449f07-f6f5-47b9-b562-823210f857fe", ApiKey = "dev-107-key" },
    new { Id = "2f6ba951-7c58-4f9d-af58-1cb9b45d07a8", ApiKey = "dev-108-key" },
    new { Id = "893a209f-0f06-444b-a647-c96b349bb1fa", ApiKey = "dev-109-key" },
    new { Id = "874a438b-c88a-470c-8607-79881dfb0297", ApiKey = "dev-110-key" }
};


// Loopa över alla devices var 10:e sekund
while (true)
{
    foreach (var device in devices)
    {
        var payload = new
        {
            deviceId = device.Id,
            apiKey = device.ApiKey,
            timestamp = DateTimeOffset.UtcNow,
            metrics = new object[]
            {
                new { type = "temperature", value = 21 + rand.NextDouble(), unit = "C" },
                new { type = "co2", value = 900 + rand.Next(0, 700), unit = "ppm" }
            }
        };

        var topic = $"tenants/{tenantId}/devices/{device.Id}/measurements";
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