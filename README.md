# EfCore.JsonColumn

Nuget =>   https://www.nuget.org/packages/EfCore.JsonColumn/
## Usage Example

```csharp
 
 
 /* Your Entity */

 public class SampleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        // Add attribute
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the  extension method 
            modelBuilder.ApplyJsonColumns();

            base.OnModelCreating(modelBuilder);
        }
    }

```
## Add-Migration

```csharp

migrationBuilder.CreateTable(
                name: "SampleEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    // Type is string 
                    JsonColumn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleEntities", x => x.Id);
                });

```


![Screenshot_9](https://user-images.githubusercontent.com/17519791/122889438-78586900-d34b-11eb-8a5b-2423817abd73.png)

