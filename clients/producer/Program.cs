using System;
using Microsoft.Azure.EventHubs;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace producer
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddCommandLine(args).Build();
            var eventHubs = config.GetSection("eventHubs");
            var clientFiles = config["clientFiles"];

            var connectionStringBuilder = new EventHubsConnectionStringBuilder(eventHubs["connectionString"])
            {
                EntityPath = eventHubs["eventHubName"]
            };

            Console.WriteLine("Press any key to quit");

            var driveSimulators = clientFiles.Split(new char[] { ',', ';' })
                .Select(o => {
                    var sim = new TDriveSimulator()
                    {
                        DriveName = o.Split('.')[0],
                        EventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString()),
                    };
                    sim.LoadPositionCollection(o);
                    sim.Start();
                    return sim;
                 }).ToArray();

            Console.ReadLine();
            foreach(var driveSimulator in driveSimulators)
            {
                driveSimulator.Stop();
            }
        }
    }
}
