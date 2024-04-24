using System.ComponentModel.DataAnnotations;

namespace DA_Web.Models
{
    public class BaiHat
    {
        public int Id { get; set; }

        [Required,StringLength(100)]
        public string Name { get; set; }

        public TimeSpan Time { get; set; }

        public String imageFile { get; set; }
        public String Lyrics { get; set; }
        public String FilePath { get; set; }

        List<BinhLuan> binhLuans { get; set; }

        public int TL_NhacID {  get; set; }
        public TL_Nhac Tl_Nhac { get; set; }
    }
}
