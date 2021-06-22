using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EfCore.JsonColumn.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var db = new SampleContext();

            var insert = new SampleEntity
            {
                Name = "Name",
                JsonColumn = new SampleJsonColumn
                {
                    Date = DateTime.Now,
                    Description = "Test"
                }
            };
            await db.SampleEntities.AddAsync(insert);
            await db.SaveChangesAsync();

            var list = await db.SampleEntities.ToListAsync();
           
            Console.ReadLine();
        }
    }

    public class SampleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [ToJsonColumn]
        public SampleJsonColumn JsonColumn { get; set; }
    }

    public class SampleJsonColumn
    {
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
    public class SampleContext : DbContext
    {
        public DbSet<SampleEntity> SampleEntities { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-BNNDM11;Database=SampleDb;Trusted_Connection=True");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyJsonColumns();

            base.OnModelCreating(modelBuilder);
        }
    }
}
