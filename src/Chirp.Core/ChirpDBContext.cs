using Chirp.Web.Storage;
using Microsoft.EntityFrameworkCore;
using System;

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    { 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Cheep>().Property(x => x.CheepId);
    }
}

public class Author
{
    public int AuthorId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; }
}

public class Cheep
{
    public int CheepId { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    public required string Text { get; set; }
    public required DateTime TimeStamp { get; set; }

    public string GetSerializedTimeStamp()
    {
        return $"{TimeStamp:MM/dd/yy H:mm:ss}";
    }

    public override string ToString()
    {
        return Author.Name + " " + Text + " " + TimeStamp;
    }
}