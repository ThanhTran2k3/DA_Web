namespace DA_Web.Models
{
    public class TL_Nhac
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string imageFile { get; set; }

        List<BaiHat> BaiHat { get; set; }
    }
}
