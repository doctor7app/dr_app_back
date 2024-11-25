using Contracts.Messages.Patients;
using MassTransit;

namespace Dme.Infrastructure.Consumers;

public class PatientCreatedConsumer : IConsumer<PatientCreatedEvent>
{
    public Task Consume(ConsumeContext<PatientCreatedEvent> context)
    {
        var item = context.Message;
        Console.WriteLine($"Consuming Message of Patient Update {item.FirstName } {item.LastName}");
        return Task.CompletedTask;
    }
}