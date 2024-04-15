using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace ServiceBusAzureMensagingConsumer
{
    class Program
    {
        // Parâmetros de conexão com o Service Bus
        private const string serviceBusConnectionString = "SUA_SERVICE_BUS_CONNECTION_STRING";
        private const string topicName = "SEU_TOPIC_NAME";
        private const string subscriptionName = "SUA_SUBSCRIPTION_NAME";

        static async Task Main(string[] args)
        {
            // Cria um cliente para o Service Bus
            ServiceBusClient client = new ServiceBusClient(serviceBusConnectionString);

            // Cria um processador para o tópico e a subscrição especificados
            ServiceBusProcessor processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

            // Registra eventos de processamento e erros
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            // Inicia o processamento
            await processor.StartProcessingAsync();

            Console.WriteLine("Pressione 'Enter' para sair do programa.");
            Console.ReadLine();

            // Para o processamento
            await processor.StopProcessingAsync();
            await client.DisposeAsync();
        }

        // Manipula as mensagens recebidas
        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            // Completa a mensagem para que não seja recebida novamente
            await args.CompleteMessageAsync(args.Message);
        }

        // Manipula erros durante o processamento
        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error: {args.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}
