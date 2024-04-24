namespace DA_Web.Models
{
    public class NgheSi
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public String imageFile { get; set; }


        public List<DanhSachPhat> danhSachPhats { get; set; }
    }
}
