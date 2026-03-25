using System;
using Microsoft.EntityFrameworkCore;
using Shipments.Domain.Models;

namespace Shipments.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core DbContext for Shipments application.
/// </summary>
public class ShipmentsDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the ShipmentsDbContext class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    public ShipmentsDbContext(DbContextOptions<ShipmentsDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Shipments DbSet.
    /// </summary>
    public DbSet<Shipment> Shipments { get; set; } = null!;

    /// <summary>
    /// Configures the model for the database.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Shipment entity
        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnType("uuid")
                .HasDefaultValueSql("gen_random_uuid()")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.PackageName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Weight)
                .HasPrecision(10, 3)
                .IsRequired();

            entity.Property(e => e.ShippingCost)
                .HasPrecision(12, 2)
                .IsRequired();

            entity.Property(e => e.OriginZipCode)
                .HasMaxLength(20);

            entity.Property(e => e.DestinationZipCode)
                .HasMaxLength(20);

            entity.Property(e => e.DestinationAddress)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.DateCreated)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(e => e.DateLastUpdated)
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(e => e.Creator)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("pending");

            // Configure Dimensions as owned type
            entity.OwnsOne(e => e.Dimensions, dimensions =>
            {
                dimensions.Property(d => d.Length)
                    .HasColumnName("DimensionsLength")
                    .HasPrecision(10, 2);

                dimensions.Property(d => d.Width)
                    .HasColumnName("DimensionsWidth")
                    .HasPrecision(10, 2);

                dimensions.Property(d => d.Height)
                    .HasColumnName("DimensionsHeight")
                    .HasPrecision(10, 2);
            });

            // Create index on Status for filtering queries
            entity.HasIndex(e => e.Status);

            // Create index on DateCreated for sorting
            entity.HasIndex(e => e.DateCreated);

            // Create composite index for common queries
            entity.HasIndex(e => new { e.Status, e.DateCreated });

            // Set table name
            entity.ToTable("shipments");
        });
    }
}
