using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Threading;

namespace ReadDeviceToCloudMessages
{
    class Program
    {
        static string connectionString = "HostName=seyonIOT.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=k6SuRGpDQIhlL/WvP9Ldy0ANYYhPnsvLcGsU87mR6Zs=";
        static string iotHubD2cEndpoint = "messages/events";
        static EventHubClient eventHubClient;
        static CancellationTokenSource cts;

        static void Main(string[] args)
        {
            Console.WriteLine("Receive Messages. Ctrl-C to exit.\n");
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, iotHubD2cEndpoint);
            var d2cPartitions = eventHubClient.GetRuntimeInformation().PartitionIds;
            cts = new CancellationTokenSource();
            System.Console.CancelKeyPress += Console_CancelKeyPress;
            var tasks = new List<Task>();
            foreach(string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }
            Task.WaitAll(tasks.ToArray());
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            cts.Cancel();
            Console.WriteLine("Exiting....");
           
        }

        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            var eventHubReceiver = eventHubClient.GetDefaultConsumerGroup().CreateReceiver(partition, DateTime.UtcNow);
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                EventData eventData = await eventHubReceiver.ReceiveAsync();
                if (eventData == null) continue;

                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine("Message received. Partition: {0} Data:'{1}'", partition, data);
            }
        }
    }
}
