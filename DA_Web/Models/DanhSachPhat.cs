namespace DA_Web.Models
{
    public class DanhSachPhat
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int SoLuong { get; set; }

        public String? imageFile { get; set; }
        public int? NgheSiId { get; set; }
        public NgheSi? NgheSi { get; set; }

        public string? UserID { get; set; }
        public ApplicationUser? User { get; set; }

        public int TL_DanhSachID { get; set; }
        public TL_DanhSachPhat TL_DanhSach { get; set; }
    }
}
