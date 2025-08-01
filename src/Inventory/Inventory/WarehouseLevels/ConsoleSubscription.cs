using JasperFx.Core.Reflection;
using JasperFx.Events.Daemon;
using JasperFx.Events.Projections;
using Marten;
using Marten.Subscriptions;

namespace Inventory.WarehouseLevels;

public class ConsoleSubscription : ISubscription
{
    public Task<IChangeListener> ProcessEventsAsync(
        EventRange page,
        ISubscriptionController controller,
        IDocumentOperations operations,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Starting to process events from {page.SequenceFloor} to {page.SequenceCeiling}");

        foreach (var e in page.Events)
            Console.WriteLine($"Got event of type {e.Data.GetType().NameInCode()} from stream {e.StreamId}");

        // If you don't care about being signaled for
        return Task.FromResult(NullChangeListener.Instance);
    }

    public ValueTask DisposeAsync()
    {
        return new ValueTask();
    }
}
