using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LangLang.Models;

public class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    //public DbSet<CourseGrade> CourseGrades { get; set; }
    public DbSet<Exam> Exams { get; set; }
    //public DbSet<ExamGrade> ExamGrades { get; set; }
    public DbSet<Language> Languages { get; set; }
    //public DbSet<Message> Messages { get; set; }
    //public DbSet<PenaltyPoint> PenaltyPoints { get; set; }
    public DbSet<ScheduleItem> ScheduleItems { get; set; }
    public DbSet<User> Users { get; set; } 
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Director>Director { get; set; }
    public DbSet<Student> Students { get; set; }
    
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        if (optionsBuilder.IsConfigured) return;
        
        DotNetEnv.Env.Load("../.env"); // Works if the current directory is the LangLang project
        string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new InvalidInputException("Connection string not found in .env file.");
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Language>()
            .Property(l => l.Id)
            .ValueGeneratedOnAdd();
        
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
            dateTime => DateOnly.FromDateTime(dateTime)
        );

        var timeOnlyConverter = new ValueConverter<TimeOnly, TimeSpan>(
            timeOnly => timeOnly.ToTimeSpan(),
            timeSpan => TimeOnly.FromTimeSpan(timeSpan)
        );

        modelBuilder.Entity<Course>()
            .Property(e => e.Date)
            .HasConversion(dateOnlyConverter);

        modelBuilder.Entity<Course>()
            .Property(e => e.StartDate)
            .HasConversion(dateOnlyConverter);

        modelBuilder.Entity<Course>()
            .Property(e => e.ScheduledTime)
            .HasConversion(timeOnlyConverter);

        modelBuilder.Entity<List<Weekday>>().HasNoKey();
        
        modelBuilder.Entity<Course>()
            .Property(c => c.Held)
            .HasPostgresArrayConversion(
                v => (int)v,
                v => (Weekday)v
            );
        modelBuilder.Entity<Student>()
            .Property(s=>s.LanguagePassFail)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v =>JsonConvert.DeserializeObject<Dictionary<int,bool>>(v)!
                );

        modelBuilder.Entity<Student>()
            .Property(s => s.CourseGradeIds)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<int, int>>(v)!
            );

        modelBuilder.Entity<Student>()
            .Property(s => s.ExamGradeIds)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<int, int>>(v)!
            );

        modelBuilder.Entity<Dictionary<int, bool>>().HasNoKey();
        modelBuilder.Entity<Dictionary<int, int>>().HasNoKey();
        modelBuilder.Entity<Dictionary<int, ApplicationStatus>>().HasNoKey();
        modelBuilder.Entity<Dictionary<int, string>>().HasNoKey();
        
        modelBuilder.Entity<Schedule>()
            .Property(s => s.ScheduleItems)
            .HasConversion(
                v => JsonConvert.SerializeObject(v,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }),
                v => JsonConvert.DeserializeObject<List<ScheduleItem>>(v,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })!
            );
        
        modelBuilder.Entity<Course>()
            .Property(s => s.Students)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<int, ApplicationStatus>>(v)!
            );
        
        modelBuilder.Entity<Course>()
            .Property(s => s.DropOutRequests)
            .HasConversion(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<Dictionary<int, string>>(v)!
            );
    }
}