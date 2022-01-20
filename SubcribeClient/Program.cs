using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SubcribeClient
{
    class Program
    {
        private const string MyTopic = "Topic1";
        private const string MQTTServerUrl = "localhost";
        private const int MQTTPort = 1883;
        static async Task Main(string[] args)
        {
            MqttFactory factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder().WithTcpServer(MQTTServerUrl, MQTTPort).WithCredentials("mqtt", "matkhau123").Build();

            
            mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));
                try
                {
                    await mqttClient.ConnectAsync(options, CancellationToken.None);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");
                // Subscribe to a topic
                await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(MyTopic).Build());

                // Subscribe all topics
                // await mqttClient.SubscribeAsync("#");
                Console.WriteLine("### SUBSCRIBED ###" + MyTopic);
                Console.WriteLine();
            });

            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            });

            await mqttClient.ConnectAsync(options, CancellationToken.None);

            while (true)
            {
                
            }
        }
    }
}
