using DatingApp.Application.DTO.Hubs;
using DatingApp.Domain.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DatingApp.Infrastructer.Data
{
    public class AppDbContext(DbContextOptions options) :IdentityDbContext<AppUser>(options)
    {

      public  DbSet<AppUser> Users { get; set; }

        public DbSet<Member> Members { get; set; }


        public DbSet<Photo> Photos { get; set; }


        public DbSet<MemberLike> Likes { get; set; }

        public DbSet<Message> Message { get; set; } 


        public DbSet<Group> Groups { get; set; }

        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
             base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityRole>().HasData(
     new IdentityRole { Id = "member-id", Name = "Member", NormalizedName = "MEMBER", ConcurrencyStamp = "fdfd7b62-eaa1-4d30-b87a-928951c5e98f" },
     new IdentityRole { Id = "moderator-id", Name = "Moderator", NormalizedName = "MODERATOR", ConcurrencyStamp = "463985e1-9416-4f53-bce2-c19ae92cde09" },
     new IdentityRole { Id = "admin-id", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "e16f5d19-f3d1-4377-80c5-03498cafc2c3" }
 );

            modelBuilder.Entity<MemberLike>().HasKey(x=>new {x.SourceMemberId, x.TargetMemberId});

            modelBuilder.Entity<MemberLike>().HasOne(x => x.SourceMember).WithMany(t => t.LikedMembers)
                .HasForeignKey(x => x.SourceMemberId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberLike>().HasOne(x=>x.TargetMember).WithMany(s => s.LikedByMembers)
                .HasForeignKey(x => x.TargetMemberId).OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Message>().HasOne(x => x.Recipient).WithMany(x => x.MessagesRecived)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>().HasOne(x => x.Sender).WithMany(x => x.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);



            var datTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullabledatTimeConverter = new ValueConverter<DateTime?, DateTime?>(
               v => v.HasValue? v.Value.ToUniversalTime():null,
               v => v.HasValue? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc):null
               );

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(datTimeConverter);
                    }else if(property.ClrType == typeof(DateTime?)){
                        property.SetValueConverter(nullabledatTimeConverter);
                    }
                    
                }
            }
        }
    }
}
