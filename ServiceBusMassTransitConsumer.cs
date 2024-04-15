using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace ServiceBusTopicConsumer
{
    class Program
    {
        public static async Task Main()
        {
            // Configuração do provedor de serviços e do MassTransit
            var services = new ServiceCollection();
            services.AddMassTransit(x =>
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host("SUA_SERVICE_BUS_CONNECTION_STRING");

                    cfg.SubscriptionEndpoint<YourMessage>("SEU_TOPIC_NAME", "SUA_SUBSCRIPTION_NAME", e =>
                    {
                        e.ConfigureConsumer<MessageConsumer>(context);
                    });
                });

                // Registra o consumidor
                x.AddConsumer<MessageConsumer>();
            });

            var serviceProvider = services.BuildServiceProvider();

            var busControl = serviceProvider.GetRequiredService<IBusControl>();
            await busControl.StartAsync(); // Inicia o serviço de mensageria

            Console.WriteLine("Pressione 'Enter' para sair do programa.");
            Console.ReadLine();

            await busControl.StopAsync(); // Para o serviço de mensageria
        }
    }

    // Define a classe que será usada para deserializar as mensagens do tópico
    public class YourMessage
    {
        public string Text { get; set; }
    }

    // Define o consumidor que irá processar as mensagens recebidas
    public class MessageConsumer : IConsumer<YourMessage>
    {
        public Task Consume(ConsumeContext<YourMessage> context)
        {
            Console.WriteLine($"Received: {context.Message.Text}");
            return Task.CompletedTask;
        }
    }
}
