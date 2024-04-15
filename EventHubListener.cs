using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

namespace EventHubListener
{
    class Program
    {
        // Parâmetros de conexão com o Event Hub
        private const string eventHubConnectionString = "SEU_EVENT_HUB_CONNECTION_STRING";
        private const string eventHubName = "SEU_EVENT_HUB_NAME";
        private const string blobStorageConnectionString = "SEU_BLOB_STORAGE_CONNECTION_STRING";
        private const string blobContainerName = "SEU_BLOB_CONTAINER_NAME";

        static async Task Main(string[] args)
        {
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            // Configura o cliente do blob para o armazenamento de checkpoints
            BlobContainerClient storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

            // Configura o processador de eventos
            EventProcessorClient processor = new EventProcessorClient(storageClient, consumerGroup, eventHubConnectionString, eventHubName);

            // Registra eventos de processamento e erros
            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;

            // Inicia o processamento
            await processor.StartProcessingAsync();

            Console.WriteLine("Pressione ENTER para parar o processador...");
            Console.ReadLine();

            // Para o processamento
            await processor.StopProcessingAsync();
        }

        // Manipula os eventos recebidos
        static async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            Console.WriteLine($"Received event: {eventArgs.Data.EventBody.ToString()}");
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
        }

        // Manipula erros durante o processamento
        static Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            Console.WriteLine($"Error: {eventArgs.Exception.Message}");
            return Task.CompletedTask;
        }
    }
}
