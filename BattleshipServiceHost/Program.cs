using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Battleship;

namespace BattleshipServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a URI for the base address
            Uri baseAddress = new Uri("http://localhost:8000/BattleshipService");

            // Create the ServiceHost
            using (ServiceHost host = new ServiceHost(typeof(BattleshipService), baseAddress))
            {
                // Add an endpoint for the service
                host.AddServiceEndpoint(typeof(IBattleshipService), new BasicHttpBinding(), "");

                // Enable metadata exchange
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                host.Description.Behaviors.Add(smb);

                // Open the ServiceHost to start listening for messages
                host.Open();

                Console.WriteLine("The Battleship service is ready at {0}", baseAddress);
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost
                host.Close();
            }
        }
    }
}
