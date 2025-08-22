using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class ApiDbContext: DbContext
    {
        public DbSet<Colaborador> Colaboradores { get; set; }
        public DbSet<DefPuesto> DefPuestos { get; set; }


        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Colaborador>().HasNoKey();
            modelBuilder.Entity<DefPuesto>().HasNoKey(); 
        }

    }
}
