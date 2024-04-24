namespace DA_Web.Models
{
    public class TL_DanhSachPhat
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public List<DanhSachPhat> DanhSachPhats { get; set; }
    }
}
