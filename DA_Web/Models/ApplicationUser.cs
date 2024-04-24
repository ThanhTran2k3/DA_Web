using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DA_Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public List<DanhSachPhat> danhSachPhats { get; set;}


        public List<BinhLuan> binhLuans { get; set;}
    }
}
