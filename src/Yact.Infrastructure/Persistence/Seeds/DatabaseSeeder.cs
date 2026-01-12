using Yact.Domain.Entities;
using Yact.Domain.ValueObjects.Cyclist;
using Yact.Infrastructure.Persistence.Data;

namespace Yact.Infrastructure.Persistence.Seeds;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;

    public DatabaseSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Verificar si ya existe data
        if (_context.Cyclists.Count() > 0)
            return;

        // ✅ Crear usando factory methods (genera eventos si quieres)
        var guid = Guid.NewGuid();
        var cyclist = Cyclist.Create(
            CyclistId.From(guid),
            "Dummy",
            "Cyclist",
            DateTime.Now,
            new Size(180, 70)
        );

        // Añadir fitness
        //cyclist.AddFitness(
        //    ftp: 250,
        //    vo2Max: 50,
        //    updateDate: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        //);

        _context.Cyclists.Add(cyclist);
        await _context.SaveChangesAsync();
    }
}
