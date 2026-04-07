using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Api.Ventas.Services.EventBus
{
    /// <summary>
    /// Interfaz del Event Bus para publicación de eventos.
    /// </summary>
    public interface IEventBus
    {
        Task PublicarVentaRealizadaAsync(VentaRealizadaEvent evento);
    }

    /// <summary>
    /// Implementación del Event Bus usando RabbitMQ.
    /// Boilerplate preparado para producción — requiere un servidor RabbitMQ activo.
    /// </summary>
    public class RabbitMQEventBus : IEventBus
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<RabbitMQEventBus> _logger;

        private const string EXCHANGE_NAME = "sistema_facturacion";
        private const string QUEUE_NAME = "venta_realizada";
        private const string ROUTING_KEY = "venta.realizada";

        public RabbitMQEventBus(IConfiguration configuration, ILogger<RabbitMQEventBus> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task PublicarVentaRealizadaAsync(VentaRealizadaEvent evento)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                    Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                    UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                    Password = _configuration["RabbitMQ:Password"] ?? "guest"
                };

                await using var connection = await factory.CreateConnectionAsync();
                await using var channel = await connection.CreateChannelAsync();

                // Declarar el Exchange tipo Topic
                await channel.ExchangeDeclareAsync(
                    exchange: EXCHANGE_NAME,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false);

                // Declarar la cola
                await channel.QueueDeclareAsync(
                    queue: QUEUE_NAME,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                // Vincular cola al exchange
                await channel.QueueBindAsync(
                    queue: QUEUE_NAME,
                    exchange: EXCHANGE_NAME,
                    routingKey: ROUTING_KEY);

                // Serializar y publicar el mensaje
                var mensajeJson = JsonSerializer.Serialize(evento);
                var body = Encoding.UTF8.GetBytes(mensajeJson);

                var props = new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent
                };

                await channel.BasicPublishAsync(
                    exchange: EXCHANGE_NAME,
                    routingKey: ROUTING_KEY,
                    mandatory: false,
                    basicProperties: props,
                    body: body);

                _logger.LogInformation(
                    "✅ Evento VentaRealizada publicado — Comprobante: {Comprobante}, Total: {Total}",
                    evento.NumeroComprobante, evento.Total);
            }
            catch (Exception ex)
            {
                // En desarrollo, se registra el error pero no se detiene el flujo
                _logger.LogWarning(
                    "⚠️ No se pudo publicar el evento VentaRealizada en RabbitMQ: {Error}. " +
                    "Esto es esperado si RabbitMQ no está activo.", ex.Message);
            }
        }
    }
}
