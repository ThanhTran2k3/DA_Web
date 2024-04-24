using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA_Web.Models;
using Microsoft.AspNetCore.Identity;
using NAudio.Wave;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace DA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
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

            var paginatedBH = baiHats
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = pageNumber;
            return View(paginatedBH);
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
                .Where(dsp => dsp.TL_DanhSachID == 1).ToListAsync();

            string lrcFilePath = "wwwroot/" + baiHat.Lyrics;
            string lrcData = await ReadLyricsFileAsync(lrcFilePath);
            ViewData["IsFavorite"] = isFavorite;
            ViewData["LRCData"] = lrcData;
            ViewData["listDanhSach"] = listdanhSachPhat;
            return View(baiHat);
        }

        // GET: BaiHats/Create
        public IActionResult Create()
        {
            ViewData["NgheSyID"] = new SelectList(_context.NgheSi, "Id", "Name");
            ViewData["TL_NhacID"] = new SelectList(_context.TL_Nhac, "ID", "Name");
            return View();
        }

        // POST: BaiHats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Time,imageFile,NgayPhatHanh,Lyrics,FilePath,binhLuans,TL_NhacID")] BaiHat baiHat, IFormFile imageFile, IFormFile lrcFile, IFormFile mp3File, string selectedNgheSiList)
        {
            List<int> selectedNgheSiIds = selectedNgheSiList.Split(',').Select(int.Parse).ToList();
            ModelState.Remove("selectedNgheSiList");
            ModelState.Remove("Name");
            ModelState.Remove("lrcFile");
            ModelState.Remove("mp3File");
            ModelState.Remove("NgayPhatHanh");
            ModelState.Remove("TL_Nhac");
            ModelState.Remove("Lyrics");
            ModelState.Remove("FilePath");
            ModelState.Remove("imageFile");
            if (ModelState.IsValid)
            {
                if (mp3File != null && mp3File.Length > 0)
                {
                    baiHat.imageFile = await SaveFile(imageFile);
                    baiHat.FilePath = await SaveFile(mp3File);
                    baiHat.Lyrics = await SaveFile(lrcFile);
                    baiHat.Time = GetMp3Time(mp3File);
                    _context.Add(baiHat);
                    if (selectedNgheSiIds != null && selectedNgheSiIds.Count > 0)
                    {
                        foreach (int ngheSiId in selectedNgheSiIds)
                        {
                            BaiHat_NgheSi baiHatNgheSi = new BaiHat_NgheSi();
                            baiHatNgheSi.baiHat = baiHat;
                            var ngheSi = await _context.NgheSi.FindAsync(ngheSiId);
                            baiHatNgheSi.NgheSi = ngheSi;
                            _context.BaiHat_NgheSi.Add(baiHatNgheSi);
                        }
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["NgheSyID"] = new SelectList(_context.NgheSi, "Id", "Name");
            ViewData["TL_NhacID"] = new SelectList(_context.TL_Nhac, "ID", "ID", baiHat.TL_NhacID);
            return View(baiHat);
        }


        // GET: BaiHats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["TL_NhacID"] = new SelectList(_context.TL_Nhac, "ID", "Name");
            ViewData["NgheSyID"] = new SelectList(_context.NgheSi, "Id", "Name");
            if (id == null)
            {
                return NotFound();
            }

            var baiHat = await _context.BaiHat.FindAsync(id);
            if (baiHat == null)
            {
                return NotFound();
            }
            return View(baiHat);
        }

        // POST: BaiHats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Time,imageFile,NgayPhatHanh,Lyrics,FilePath,binhLuans,TL_NhacID")] BaiHat baiHat, IFormFile imageFile, IFormFile lrcFile, IFormFile mp3File, string selectedNgheSiList)
        {
            List<int> selectedNgheSiIds = selectedNgheSiList.Split(',').Select(int.Parse).ToList();
            ModelState.Remove("FilePath");
            ModelState.Remove("Time");
            ModelState.Remove("Lyrics");
            ModelState.Remove("binhLuans");
            ModelState.Remove("imageFile");
            ModelState.Remove("TL_Nhac");
            if (ModelState.IsValid)
            {
                if (mp3File != null && mp3File.Length > 0)
                {
                    baiHat.imageFile = await SaveFile(imageFile);
                    baiHat.FilePath = await SaveFile(mp3File);
                    baiHat.Lyrics = await SaveFile(lrcFile);
                    baiHat.Time = GetMp3Time(mp3File);
                    _context.Update(baiHat);
                    if (selectedNgheSiIds != null && selectedNgheSiIds.Count > 0)
                    {
                        var CTbaihat = await _context.BaiHat_NgheSi.Where(p => p.BaiHatID == baiHat.Id).ToListAsync();
                        foreach (BaiHat_NgheSi baiHat_NgheSi in CTbaihat)
                        {
                            _context.BaiHat_NgheSi.Remove(baiHat_NgheSi);
                        }
                        foreach (int ngheSiId in selectedNgheSiIds)
                        {
                            BaiHat_NgheSi baiHatNgheSi = new BaiHat_NgheSi();
                            baiHatNgheSi.baiHat = baiHat;
                            var ngheSi = await _context.NgheSi.FindAsync(ngheSiId);
                            baiHatNgheSi.NgheSi = ngheSi;
                            _context.BaiHat_NgheSi.Add(baiHatNgheSi);
                        }
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(baiHat);
        }

        // GET: BaiHats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var baiHat = await _context.BaiHat
                .FirstOrDefaultAsync(m => m.Id == id);
            if (baiHat == null)
            {
                return NotFound();
            }

            return View(baiHat);
        }

        // POST: BaiHats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var baiHat = await _context.BaiHat.FindAsync(id);
            if (baiHat != null)
            {
                _context.BaiHat.Remove(baiHat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
                return "/images/songs/" + file.FileName;
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

        public async Task<IActionResult> DanhGia(int idBH, int pageNumber = 1)
        {
            ViewBag.BaiHat = await _context.BaiHat.FindAsync(idBH);
            List<BinhLuan> binhLuans = await _context.BinhLuan.Where(p => p.BaiHatID == idBH).ToListAsync();
            int pageSize = 10;
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
        public async Task<IActionResult> AddDanhGia(string noiDung, int idbh)
        {
            string userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }
            var binhLuan = new BinhLuan
            {
                NoiDung = noiDung,
                BaiHatID = idbh,
                UserID = userId,
                NgayTao = DateTime.Now
            };

            _context.BinhLuan.Add(binhLuan);
            await _context.SaveChangesAsync();

            return RedirectToAction("DanhGia", new { idBH = idbh });
        }


        public async Task<IActionResult> XoaDanhGia(int idBL, int idBH)
        {

            var binhLuan = await _context.BinhLuan.FindAsync(idBL);
            _context.BinhLuan.Remove(binhLuan);
            await _context.SaveChangesAsync();
            return RedirectToAction("DanhGia", new { idBH = idBH });
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
                int totalPages;
                var paginatedBaiHats = result;
                if (result.Count() != 0)
                {
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
