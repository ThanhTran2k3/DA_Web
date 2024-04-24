namespace DA_Web.Models
{
    public class BaiHat_NgheSi
    {
        public int BaiHatID { get; set; }
        public BaiHat baiHat { get; set; }


        public int NgheSiID { get; set; }
        public NgheSi? NgheSi { get; set; }
    }
}
