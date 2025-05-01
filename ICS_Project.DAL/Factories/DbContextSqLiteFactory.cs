using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICS_Project.DAL.Factories
{
    public class DbContextSqLiteFactory : IDbContextFactory<MusicDbContext>
    {
        private readonly DbContextOptions<MusicDbContext> _options;
        private readonly bool _seedData; // Parameter pre seedovanie

        // Konštruktor prijíma cestu a príznak seedovania
        public DbContextSqLiteFactory(string databasePath, bool seedData = false)
        {
            var contextOptionsBuilder = new DbContextOptionsBuilder<MusicDbContext>();
            contextOptionsBuilder.UseSqlite("Data Source=music.db");
            // contextOptionsBuilder.EnableSensitiveDataLogging(); // Pre ladenie
            _options = contextOptionsBuilder.Options;
            _seedData = seedData; // Uložíme príznak seedovania
        }

        // Vytvorí DbContext s použitím uložených options a príznaku seedovania
        public MusicDbContext CreateDbContext() => new(_options, _seedData);
    }
}
