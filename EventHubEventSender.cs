using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace EventHubSingleEventSender
{
    class Program
    {
        private const string connectionString = "<SUA_EVENT_HUB_CONNECTION_STRING>";
        private const string eventHubName = "<SEU_EVENT_HUB_NAME>";

        static async Task Main(string[] args)
        {
            // Criar um produtor de eventos para o Event Hub
            await using (var producer = new EventHubProducerClient(connectionString, eventHubName))
            {
                // Criar um objeto do evento customizado
                var customEvent = new CustomEvent
                {
                    Id = 1,
                    Timestamp = DateTime.UtcNow,
                    Message = "Hello, Event Hub!"
                };

                // Serializar o objeto para JSON
                var eventData = new EventData(JsonSerializer.SerializeToUtf8Bytes(customEvent));

                // Enviar o evento para o Event Hub
                await producer.SendAsync(eventData);
                Console.WriteLine("Evento enviado com sucesso para o Event Hub.");
            }
        }
    }

    // Definir a classe do evento customizado
    public class CustomEvent
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
    }
}
