using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace reveph
{
    class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static string StorageConnectionString(string storageAccountName, string storageAccountKey) => string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey);

        private static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Registering EventProcessor...");

            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true).Build();
            var eventHubs = config.GetSection("eventHubs");
            var storageAccount = config.GetSection("storageAccount");

            var eventProcessorHost = new EventProcessorHost(
                eventHubs["eventHubName"],
                PartitionReceiver.DefaultConsumerGroupName,
                eventHubs["connectionString"],
                StorageConnectionString(storageAccount["accountName"], storageAccount["accountKey"]),
                storageAccount["containerName"]);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>();

            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }
    }
}
