using System;
using System.Collections.Generic;
using BetaCycle_Padova.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace BetaCycle_Padova.Controllers.Context;

public partial class BetacycleUsersContext : DbContext
{
    public BetacycleUsersContext()
    {
    }

    public BetacycleUsersContext(DbContextOptions<BetacycleUsersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Credential> Credentials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=BetacycleUsers;Integrated Security=True;Encrypt=False;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Credential>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Credenti__3213E83F1091C71C");

            entity.HasIndex(e => e.IdUser, "UQ__Credenti__D2D1463678CB300D").IsUnique();

            entity.Property(e => e.Id)               
                .HasColumnName("id");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Salt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("salt");

            entity.HasOne(d => d.IdUserNavigation).WithOne(p => p.Credential)
                .HasForeignKey<Credential>(d => d.IdUser)
                .HasConstraintName("FK__Credentia__id_us__286302EC");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83F45457F7F");

            entity.ToTable("User");

            entity.HasIndex(e => e.Mail, "UQ__User__7A2129049DFFBED6").IsUnique();

            entity.Property(e => e.Id)                
                .HasColumnName("id");
            entity.Property(e => e.Mail)
                .HasMaxLength(50)
                .HasColumnName("mail");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(25)
                .HasColumnName("phone");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
