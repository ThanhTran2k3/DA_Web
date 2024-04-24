using DA_Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NAudio.Wave;
using System.Security.Claims;

namespace DA_Web.Controllers
{
    public class BaiHatsController : Controller
    {
        private readonly Web_Context _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public BaiHatsController(Web_Context context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BaiHats
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var baiHats = await _context.BaiHat.ToListAsync();

            int pageSize = 8;
            int totalPages = (int)Math.Ceiling((double)baiHats.Count() / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

            var paginatedProducts = baiHats
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = pageNumber;
            return View(paginatedProducts);
        }




        // GET: BaiHats/Details/5
        public async Task<IActionResult> Details(int id)
        {
            string userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var baiHat = await _context.BaiHat.FindAsync(id);
            var danhSachPhat = await _context.DanhSachPhat
                .FirstOrDefaultAsync(dsp => dsp.UserID == userId && dsp.TL_DanhSachID == 3);
            if (baiHat == null)
            {
                return NotFound();
            }
            bool isFavorite = false;
            if (danhSachPhat != null)
            {
                isFavorite = await _context.DanhSach_BaiHat
                    .AnyAsync(dsb => dsb.DanhSachID == danhSachPhat.ID && dsb.BaiHatID == baiHat.Id);
            }
            var listdanhSachPhat = await _context.DanhSachPhat
                .Where(dsp => dsp.UserID == userId && dsp.TL_DanhSachID == 2).ToListAsync();

            string lrcFilePath = "wwwroot/" + baiHat.Lyrics;
            string lrcData = await ReadLyricsFileAsync(lrcFilePath);
            ViewData["IsFavorite"] = isFavorite;
            ViewData["LRCData"] = lrcData;
            ViewData["listDanhSach"] = listdanhSachPhat;
            return View(baiHat);
        }

        private bool BaiHatExists(int id)
        {
            return _context.BaiHat.Any(e => e.Id == id);
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            if (file.ContentType == "audio/mpeg" || file.ContentType == "audio/mp3")
            {
                var savePath = Path.Combine("wwwroot/audio", file.FileName);
                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return "/audio/" + file.FileName;
            }
            else if (file.ContentType == "image/jpeg" || file.ContentType == "image/png")
            {
                // Lưu tệp hình ảnh vào thư mục "images"
                var savePath = Path.Combine("wwwroot/images/songs", file.FileName);
                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return "/images/songs" + file.FileName;
            }
            else
            {
                var savePath = Path.Combine("wwwroot/lyrics", file.FileName);
                using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                }
                return "/lyrics/" + file.FileName;
            }
        }

        public TimeSpan GetMp3Time(IFormFile filePath)
        {
            using (var stream = filePath.OpenReadStream())
            {
                // Tạo Mp3FileReader với Stream
                using (var reader = new Mp3FileReader(stream))
                {
                    TimeSpan duration = reader.TotalTime;

                    double roundedSeconds = Math.Floor(duration.TotalSeconds);
                    TimeSpan roundedDuration = TimeSpan.FromSeconds(roundedSeconds);

                    return roundedDuration;

                }
            }
        }



        public async Task<string> ReadLyricsFileAsync(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<IActionResult> AddYeuThich(int id)
        {
            string userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }
            var baiHat = await _context.BaiHat.FindAsync(id);
            var danhSachPhat = await _context.DanhSachPhat
                        .Where(dsp => dsp.UserID == userId && dsp.TL_DanhSachID == 3)
                        .FirstOrDefaultAsync();
            var find = await _context.DanhSach_BaiHat.Where(a => a.DanhSachID == danhSachPhat.ID && a.BaiHatID == baiHat.Id).FirstOrDefaultAsync();
            if (find != null)
            {
                _context.DanhSach_BaiHat.Remove(find);
                await _context.SaveChangesAsync();
            }
            else
            {
                DanhSach_BaiHat danhSach_BaiHat = new DanhSach_BaiHat();
                danhSach_BaiHat.DanhSach = danhSachPhat;
                danhSach_BaiHat.BaiHat = baiHat;
                _context.DanhSach_BaiHat.Add(danhSach_BaiHat);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = id });

        }


        public async Task<IActionResult> YeuThich(int pageNumber = 1)
        {
            List<BaiHat> listBaiHat = new List<BaiHat>();
            string userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }
            var danhSachPhat = await _context.DanhSachPhat
                        .Where(dsp => dsp.UserID == userId && dsp.TL_DanhSachID == 3)
                        .FirstOrDefaultAsync();

            if (danhSachPhat == null)
            {
                return NotFound();
            }

            var CT_DanhSach = await _context.DanhSach_BaiHat
                            .Where(dsp => dsp.DanhSachID == danhSachPhat.ID)
                            .Include(dsp => dsp.BaiHat)
                            .ToListAsync();

            if (CT_DanhSach != null)
            {
                foreach (var danhSachBaiHat in CT_DanhSach)
                {
                    var baiHat = danhSachBaiHat.BaiHat;
                    listBaiHat.Add(baiHat);
                }
            }
            int pageSize = 8;
            int totalPages = (int)Math.Ceiling((double)listBaiHat.Count() / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

            var paginatedBH = listBaiHat
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = pageNumber;
            return View(paginatedBH);

            return View(listBaiHat);
        }


        public async Task<IActionResult> DanhGia(int idBH, int pageNumber = 1)
        {
            ViewBag.BaiHat = await _context.BaiHat.FindAsync(idBH);
            List<BinhLuan> binhLuans = await _context.BinhLuan.Where(p => p.BaiHatID == idBH).ToListAsync();
            int pageSize = 8;
            int totalPages = (int)Math.Ceiling((double)binhLuans.Count() / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

            var paginatedBL = binhLuans
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = pageNumber;
            return View(paginatedBL);
        }

        [HttpPost]
        public async Task<IActionResult> AddDanhGia(string djjd, int idbh)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            string userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }

            // Tạo đối tượng bình luận mới
            var binhLuan = new BinhLuan
            {
                NoiDung = djjd, // Gán giá trị từ ô textarea
                BaiHatID = idbh,
                UserID = userId,
                NgayTao = DateTime.Now
            };

            // Thêm bình luận vào cơ sở dữ liệu và lưu thay đổi
            _context.BinhLuan.Add(binhLuan);
            await _context.SaveChangesAsync();

            // Chuyển hướng đến trang danh sách bình luận
            return RedirectToAction("DanhGia", new { idBH  = idbh });
        }

        public async Task<IActionResult> ViewTheLoai()
        {
            var tlNhac = await _context.TL_Nhac.ToListAsync();
            
            return View(tlNhac);
        }


        public async Task<IActionResult> DetailsTheLoai(int id, int pageNumber = 1)
        {
            List<BaiHat> listBaiHat = await _context.BaiHat.Where(p => p.TL_NhacID == id).ToListAsync();

            ViewBag.TLNhac = await _context.TL_Nhac.FindAsync(id);

            int pageSize = 8;
            int totalPages = (int)Math.Ceiling((double)listBaiHat.Count() / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

            var paginated = listBaiHat
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = pageNumber;
            return View(paginated);
        }


        public async Task<IActionResult> SearchBaiHats(string query, int pageNumber = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                    return BadRequest("Search query is required");
                var result = _context.BaiHat.Where(p => p.Name.Contains(query)).ToList();
                int pageSize = 8;
                int totalPages ;
                var paginatedBaiHats = result;
                if (result.Count() !=0) {
                    totalPages = (int)Math.Ceiling((double)result.Count() / pageSize);
                    pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));
                    paginatedBaiHats = result
                                            .Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToList();
                }
                else
                {
                    var listBaiHat = new List<BaiHat>();
                    var ngheSi = await _context.NgheSi.Where(p => p.Name.Contains(query)).ToListAsync();
                    foreach (var si in ngheSi)
                    {
                        var ctNgheSiList = await _context.BaiHat_NgheSi
                            .Where(bns => bns.NgheSiID == si.Id)
                            .Include(bns => bns.baiHat) 
                            .ToListAsync();
                        foreach (var ctNgheSi in ctNgheSiList)
                        {
                             listBaiHat.Add(ctNgheSi.baiHat);
                        }
                    }
                    totalPages = (int)Math.Ceiling((double)listBaiHat.Count() / pageSize);
                    pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));
                    paginatedBaiHats = listBaiHat
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList();

                }
                ViewBag.Query = query;
                ViewBag.TotalPages = totalPages;
                ViewBag.PageNumber = pageNumber;
                return View("Index", paginatedBaiHats);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public List<string> SearchSuggestions(string query)
        {

            return _context.BaiHat

            .Where(p => p.Name.StartsWith(query))
            .Select(p => p.Name)
            .ToList();

        }
    }
}
