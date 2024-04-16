using MassTransit;
using System;
using System.Threading.Tasks;

namespace MassTransitPublisher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var busControl = Bus.Factory.CreateUsingAzureServiceBus(cfg =>
            {
                cfg.Host("SUA_AZURE_SERVICE_BUS_CONNECTION_STRING");

                // Configurações adicionais podem ser feitas aqui se necessário
            });

            await busControl.StartAsync();
            try
            {
                var message = new CustomEvent
                {
                    Id = 1,
                    Timestamp = DateTime.UtcNow,
                    Message = "Hello, Service Bus!"
                };

                await busControl.Publish(message);

                Console.WriteLine("Mensagem enviada com sucesso para o Azure Service Bus.");
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
