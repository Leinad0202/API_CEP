using Microsoft.EntityFrameworkCore;
using API_CEP.Models;

namespace API_CEP.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<CepEndereco> CepEnderecos { get; set; }
    }
}
