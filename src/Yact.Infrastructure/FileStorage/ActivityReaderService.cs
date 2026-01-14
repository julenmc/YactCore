using Dynastream.Fit;
using Yact.Application.DTOs.Activity;
using Yact.Application.Interfaces.Files;
using Yact.Domain.ValueObjects.Activity;
using Yact.Domain.ValueObjects.Activity.Records;

namespace Yact.Infrastructure.FileStorage;

public class ActivityReaderService : IActivityReaderService
{
    private const double Semicircles = 2147483648.0;

    public async Task<ActivitySummary> ReadActivitySummaryAsync(Stream fileStream)
    {
        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        ms.Position = 0;

        var decode = new Decode();
        var broadcaster = new MesgBroadcaster();
        var recordData = new List<RecordsRawData>();

        string name = "Unknown";
        string? type = "Unknown";
        System.DateTime start = System.DateTime.Now;
        System.DateTime end = System.DateTime.Now;
        double distance = 0;
        
        broadcaster.SessionMesgEvent += (s, e) =>
        {
            var m = (SessionMesg)e.mesg;

            name = m.Name;
            type = m.GetSport()?.ToString();
            start = m.GetStartTime().GetDateTime();
            end = start.Add(TimeSpan.FromSeconds((double)(m.GetTotalElapsedTime() ?? 0)));
            distance = (double)(m.GetTotalDistance() ?? 0);
        };

        if (!decode.IsFIT(ms) || !decode.CheckIntegrity(ms))
            throw new InvalidDataException("Invalid or corrupt FIT file");

        ms.Position = 0;
        decode.MesgEvent += broadcaster.OnMesg;

        decode.Read(ms);

        return ActivitySummary.Create(
            name: name,
            type: type,
            start: start,
            end: end,
            create: System.DateTime.Now
        );
    }

    public async Task<ActivityReadData> ReadFullActivityAsync(Stream fileStream)
    {
        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        ms.Position = 0;

        var decode = new Decode();
        var broadcaster = new MesgBroadcaster();
        var recordData = new List<RecordsRawData>();

        broadcaster.RecordMesgEvent += (s, e) =>
        {
            var m = (RecordMesg)e.mesg;

            recordData.Add(new RecordsRawData(
                DateTime: m.GetTimestamp().GetDateTime(),
                Coordinates: new Coordinates 
                {
                    Latitude = m.GetPositionLat() * 180.0 / Semicircles ?? 0,
                    Longitude = m.GetPositionLong() * 180.0 / Semicircles ?? 0,
                    Altitude = m.GetAltitude() ?? 0
                },
                Performance: new Performance(
                    SpeedMps: m.GetSpeed(),
                    HeartRate: m.GetHeartRate(),
                    Power: m.GetPower(),
                    Cadence: m.GetCadence()
                )
            ));
        };

        string name = "Unknown";
        string? type = "Unknown";
        broadcaster.SessionMesgEvent += (s, e) =>
        {
            var m = (SessionMesg)e.mesg;

            name = m.Name;
            type = m.GetSport()?.ToString();
        };

        if (!decode.IsFIT(ms) || !decode.CheckIntegrity(ms))
            throw new InvalidDataException("Invalid or corrupt FIT file");

        ms.Position = 0;
        decode.MesgEvent += broadcaster.OnMesg;

        // Decodificación (Fit SDK no es async, pero el I/O sí lo es)
        decode.Read(ms);

        return new ActivityReadData(
            Name: name,
            Type: type,
            Records: recordData
        );
    }
}
