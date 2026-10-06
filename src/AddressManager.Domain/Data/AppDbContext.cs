// ============================================================
// AppDbContext — главный контекст Entity Framework Core
//
// Отвечает за:
//   - Предоставление DbSet<T> для каждой сущности (таблицы БД)
//   - Настройку маппинга сущностей на таблицы (OnModelCreating)
//   - Начальное заполнение базы данными (HasData — seed data)
//
// Иерархия данных:
//   Country (страна)
//     └─ Region (регион / область / воеводство)
//          └─ City (город)
//               └─ Address (адрес)
// ============================================================

using AddressManager.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressManager.Domain.Data;

/// <summary>
/// Контекст Entity Framework Core для базы данных AddressDb.
/// Регистрируется в DI как Scoped-сервис через Program.cs.
/// </summary>
public class AppDbContext : DbContext
{
    // Конструктор принимает DbContextOptions — настройки, переданные при регистрации
    // в DI (строка подключения, провайдер SQL Server и т.д.)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSet<T> — представление таблицы в виде LINQ-запросов.
    // Используем свойство-выражение (=> Set<T>()) вместо автосвойства,
    // чтобы EF Core сам управлял кешированием DbSet.
    public DbSet<Country>  Countries  => Set<Country>();
    public DbSet<Region>   Regions    => Set<Region>();
    public DbSet<City>     Cities     => Set<City>();
    public DbSet<Address>  Addresses  => Set<Address>();

    /// <summary>
    /// Fluent API-конфигурация модели.
    /// Вызывается EF Core один раз при первом использовании контекста.
    /// Здесь задаются ключи, ограничения, связи и начальные данные.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Country ─────────────────────────────────────────────────
        modelBuilder.Entity<Country>(e =>
        {
            e.HasKey(x => x.Id);                                // PRIMARY KEY
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Code).IsRequired().HasMaxLength(2);  // ISO 3166-1 alpha-2: "UA", "PL", "DE"
            e.HasIndex(x => x.Code).IsUnique();                 // UNIQUE INDEX — код страны уникален

            // Начальные данные (применяются миграцией)
            e.HasData(
                new Country { Id = 1, Name = "Украина",  Code = "UA" },
                new Country { Id = 2, Name = "Польша",   Code = "PL" },
                new Country { Id = 3, Name = "Германия", Code = "DE" }
            );
        });

        // ── Region ──────────────────────────────────────────────────
        modelBuilder.Entity<Region>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);

            // Связь "многие регионы — одна страна".
            // OnDelete Cascade: при удалении страны удаляются все её регионы.
            e.HasOne(x => x.Country).WithMany(c => c.Regions)
             .HasForeignKey(x => x.CountryId).OnDelete(DeleteBehavior.Cascade);

            e.HasData(
                new Region { Id = 1, Name = "Киевская область",        CountryId = 1 },
                new Region { Id = 2, Name = "Львовская область",       CountryId = 1 },
                new Region { Id = 3, Name = "Одесская область",        CountryId = 1 },
                new Region { Id = 4, Name = "Мазовецкое воеводство",   CountryId = 2 },
                new Region { Id = 5, Name = "Малопольское воеводство", CountryId = 2 },
                new Region { Id = 6, Name = "Бавария",                 CountryId = 3 }
            );
        });

        // ── City ────────────────────────────────────────────────────
        modelBuilder.Entity<City>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);

            // Связь "много городов — один регион"
            e.HasOne(x => x.Region).WithMany(r => r.Cities)
             .HasForeignKey(x => x.RegionId).OnDelete(DeleteBehavior.Cascade);

            e.HasData(
                new City { Id = 1, Name = "Киев",    RegionId = 1 },
                new City { Id = 2, Name = "Бровары", RegionId = 1 },
                new City { Id = 3, Name = "Львов",   RegionId = 2 },
                new City { Id = 4, Name = "Одесса",  RegionId = 3 },
                new City { Id = 5, Name = "Варшава", RegionId = 4 },
                new City { Id = 6, Name = "Краков",  RegionId = 5 },
                new City { Id = 7, Name = "Мюнхен",  RegionId = 6 }
            );
        });

        // ── Address ─────────────────────────────────────────────────
        modelBuilder.Entity<Address>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Street).IsRequired().HasMaxLength(200);
            e.Property(x => x.HouseNumber).IsRequired().HasMaxLength(20);
            e.Property(x => x.ApartmentNumber).HasMaxLength(20); // nullable — не у всех адресов есть квартира
            e.Property(x => x.PostalCode).IsRequired().HasMaxLength(20);

            // Связь "много адресов — один город"
            e.HasOne(x => x.City).WithMany(c => c.Addresses)
             .HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Cascade);

            // Тестовые адреса: Украина (Киев, Бровары, Львов, Одесса),
            //                  Польша (Варшава, Краков), Германия (Мюнхен)
            e.HasData(
                new Address { Id = 1,  Street = "вул. Хрещатик",            HouseNumber = "1",   ApartmentNumber = "10", PostalCode = "01001",  CityId = 1 },
                new Address { Id = 2,  Street = "вул. Велика Васильківська", HouseNumber = "3",   ApartmentNumber = "45", PostalCode = "01004",  CityId = 1 },
                new Address { Id = 3,  Street = "вул. Бориспільська",        HouseNumber = "12",  ApartmentNumber = null, PostalCode = "07400",  CityId = 2 },
                new Address { Id = 4,  Street = "вул. Шевченка",             HouseNumber = "7",   ApartmentNumber = "2",  PostalCode = "79000",  CityId = 3 },
                new Address { Id = 5,  Street = "просп. Свободи",            HouseNumber = "15",  ApartmentNumber = "8",  PostalCode = "79008",  CityId = 3 },
                new Address { Id = 6,  Street = "вул. Дерибасівська",        HouseNumber = "22",  ApartmentNumber = "5",  PostalCode = "65026",  CityId = 4 },
                new Address { Id = 7,  Street = "ul. Marszałkowska",         HouseNumber = "100", ApartmentNumber = "12", PostalCode = "00-026", CityId = 5 },
                new Address { Id = 8,  Street = "ul. Nowy Świat",            HouseNumber = "64",  ApartmentNumber = null, PostalCode = "00-357", CityId = 5 },
                new Address { Id = 9,  Street = "ul. Floriańska",            HouseNumber = "3",   ApartmentNumber = "1",  PostalCode = "31-019", CityId = 6 },
                new Address { Id = 10, Street = "Maximilianstraße",          HouseNumber = "12",  ApartmentNumber = null, PostalCode = "80539",  CityId = 7 }
            );
        });
    }
}
