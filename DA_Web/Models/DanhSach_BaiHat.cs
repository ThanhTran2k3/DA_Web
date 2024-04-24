namespace DA_Web.Models
{
    public class DanhSach_BaiHat
    {
        public int BaiHatID { get; set; }
        public BaiHat BaiHat { get; set; }

        public int DanhSachID { get; set; }
        public DanhSachPhat DanhSach { get; set; }
    }
}
