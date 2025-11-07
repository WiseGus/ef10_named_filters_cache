using System.Diagnostics.Metrics;

namespace EF10_QueryCache;

public class MeterListenerHelper
{
    private MeterListener _listener;

    public MeterListenerHelper()
    {
        _listener = new MeterListener();

        _listener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Meter.Name == "Microsoft.EntityFrameworkCore")
            {
                //Console.WriteLine($"Subscribed to {instrument.Meter.Name}.{instrument.Name}");
                listener.EnableMeasurementEvents(instrument);
            }
        };

        _listener.SetMeasurementEventCallback<long>(OnMeasurement);
        _listener.SetMeasurementEventCallback<int>(OnMeasurement);
        _listener.SetMeasurementEventCallback<double>(OnMeasurement);

        _listener.Start();
    }

    public void CollectMetrics()
    {
        _listener.RecordObservableInstruments();
    }

    private void OnMeasurement<T>(Instrument instrument, T value, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        Console.WriteLine($"[{instrument.Meter.Name}] {instrument.Name} = {value}");
    }
}