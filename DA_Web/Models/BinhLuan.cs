namespace DA_Web.Models
{
    public class BinhLuan
    {
        public int Id { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayTao { get; set; }

        public int BaiHatID {  get; set; }
        public BaiHat baiHat {  get; set; }

        public string UserID { get; set; }
        public ApplicationUser User { get; set; }
    }
}
