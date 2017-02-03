using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace SimulatedDevice
{
    class Program
    {
        static DeviceClient deviceClient;
        static string iotHubUri = "seyonIOT.azure-devices.net";
        static string deviceKey = "+ZxDkXohpMf2ihrFNE/+NmOVxUaZDdS/3wioUWtGzqk=";
        static void Main(string[] args)
        {
            Console.WriteLine("Simulated device\n");
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(deviceKey);
            var encodedKey = System.Convert.ToBase64String(plainTextBytes);
            deviceClient = DeviceClient.Create(iotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey("mySecondDevice", deviceKey), TransportType.Mqtt);

            SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }

        private static async void SendDeviceToCloudMessagesAsync()
        {
            double avgWindSpeed = 10;
            Random rand = new Random();

            while (true)
            {
                double currentWindSpeed = avgWindSpeed + rand.NextDouble() * 4 - 2;
                var telemtryDataPoint = new
                {
                    deviceId = "mySecondDevice",
                    windSpeed = currentWindSpeed
                };
                var messageString = JsonConvert.SerializeObject(telemtryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

                await deviceClient.SendEventAsync(message);
                Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);

                Task.Delay(1000).Wait();
            }
        }
    }
}
