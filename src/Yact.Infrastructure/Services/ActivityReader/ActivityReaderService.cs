using Dynastream.Fit;
using Yact.Application.Interfaces;
using YactActivity = Yact.Domain.Entities.Activity;

namespace Yact.Infrastructure.Services.ActivityReader;

public class ActivityReaderService : IActivityReaderService
{
    private const double Semicircles = 2147483648.0;

    public async Task<YactActivity.Activity> ReadActivityAsync(Stream fileStream, string fileName, string description = "")
    {
        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        ms.Position = 0;

        var decode = new Decode();
        var broadcaster = new MesgBroadcaster();
        var result = new YactActivity.Activity()
        {
            Name = fileName,
            Path = "Error", // change in application
        };
        float? lastAltitude = null;

        broadcaster.RecordMesgEvent += (s, e) =>
        {
            var m = (RecordMesg)e.mesg;
            var currentAltitude = m.GetAltitude();
            if (lastAltitude != null && currentAltitude != null)
            {
                float altDiff = (float)(currentAltitude - lastAltitude);
                result.ElevationMeters += (altDiff > 0) ? altDiff : 0;
            }
            lastAltitude = currentAltitude;

            //result.Records.Add(new RecordData
            //{
            //    Timestamp = m.GetTimestamp().GetDateTime(),
            //    Latitude = ConvertSemicircles(m.GetPositionLat()),
            //    Longitude = ConvertSemicircles(m.GetPositionLong()),
            //    Altitude = m.GetAltitude(),
            //    Speed = m.GetSpeed(),
            //    HeartRate = m.GetHeartRate(),
            //    Power = m.GetPower(),
            //    Cadence = m.GetCadence()
            //});
        };

        broadcaster.SessionMesgEvent += (s, e) =>
        {
            var m = (SessionMesg)e.mesg;

            result.Type = m.GetSport()?.ToString();
            result.DistanceMeters = Math.Round((double)(m.GetTotalDistance() ?? 0.0f));
            result.StartDate = m.GetStartTime().GetDateTime();
            result.EndDate = result.StartDate.AddSeconds(m.GetTotalElapsedTime() ?? 0.0f);
        };

        //broadcaster.LapMesgEvent += (s, e) =>
        //{
        //    var m = (LapMesg)e.mesg;
        //    result.Laps.Add(new LapData
        //    {
        //        TotalDistanceKm = m.GetTotalDistance() / 1000.0,
        //        TotalTimeSeconds = m.GetTotalTimerTime() / 1000.0
        //    });
        //};

        if (!decode.IsFIT(ms) || !decode.CheckIntegrity(ms))
            throw new InvalidDataException("Invalid or corrupt FIT file");

        ms.Position = 0;
        decode.MesgEvent += broadcaster.OnMesg;

        // Decodificación (Fit SDK no es async, pero el I/O sí lo es)
        decode.Read(ms);
        result.ElevationMeters = Math.Round(result.ElevationMeters);

        return result;
    }
}
