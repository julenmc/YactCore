using Dynastream.Fit;
using Yact.Application.Interfaces;
using YactActivity = Yact.Domain.Entities.Activity;

namespace Yact.Infrastructure.Services.ActivityReader;

public class ActivityReaderService : IActivityReaderService
{
    private const double Semicircles = 2147483648.0;

    public async Task<YactActivity.Activity> ReadActivityAsync(Stream fileStream)
    {
        using var ms = new MemoryStream();
        await fileStream.CopyToAsync(ms);
        ms.Position = 0;

        var decode = new Decode();
        var broadcaster = new MesgBroadcaster();
        var result = new YactActivity.Activity()
        {
            Info = new YactActivity.ActivityInfo()
            {
                Name = "Name",
                Path = "Error", // change in application
            },
            RecordData = new List<YactActivity.RecordData>()
        };
        float? lastAltitude = null;

        broadcaster.RecordMesgEvent += (s, e) =>
        {
            var m = (RecordMesg)e.mesg;
            var currentAltitude = m.GetAltitude();
            if (lastAltitude != null && currentAltitude != null)
            {
                float altDiff = (float)(currentAltitude - lastAltitude);
                result.Info.ElevationMeters += (altDiff > 0) ? altDiff : 0;
            }
            lastAltitude = currentAltitude;

            result.RecordData.Add(new YactActivity.RecordData
            {
                Timestamp = m.GetTimestamp().GetDateTime(),
                Coordinates = new YactActivity.CoordinatesData
                {
                    Latitude = m.GetPositionLat(),
                    Longitude = m.GetPositionLong(),
                    Altitude = m.GetAltitude(),
                },
                SpeedMps = m.GetSpeed(),
                HeartRate = m.GetHeartRate(),
                Power = m.GetPower(),
                Cadence = m.GetCadence()
            });
        };

        broadcaster.SessionMesgEvent += (s, e) =>
        {
            var m = (SessionMesg)e.mesg;

            result.Info.Type = m.GetSport()?.ToString();
            result.Info.DistanceMeters = Math.Round((double)(m.GetTotalDistance() ?? 0.0f));
            result.Info.StartDate = m.GetStartTime().GetDateTime();
            result.Info.EndDate = result.Info.StartDate.AddSeconds(m.GetTotalElapsedTime() ?? 0.0f);
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
        result.Info.ElevationMeters = Math.Round(result.Info.ElevationMeters);

        return result;
    }
}
