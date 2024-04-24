using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace DA_Web.Models
{
    public class Web_Context : IdentityDbContext<ApplicationUser>
    {
        public Web_Context(DbContextOptions<Web_Context> options) : base(options)
        {
        }
        public DbSet<BaiHat> BaiHat { get; set; }
        public DbSet<BaiHat_NgheSi> BaiHat_NgheSi { get; set; }
        public DbSet<BinhLuan> BinhLuan { get; set; }
        public DbSet<DanhSach_BaiHat> DanhSach_BaiHat { get; set; }
        public DbSet<DanhSachPhat> DanhSachPhat { get; set; }
        public DbSet<NgheSi> NgheSi { get; set; }
        public DbSet<TL_Nhac> TL_Nhac { get; set; }
        public DbSet<TL_DanhSachPhat> TL_DanhSach { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BaiHat_NgheSi>()
                .HasKey(bn => new { bn.BaiHatID, bn.NgheSiID });

            modelBuilder.Entity<DanhSach_BaiHat>()
                .HasKey(bn => new { bn.BaiHatID, bn.DanhSachID });

        
        }


    }
}
