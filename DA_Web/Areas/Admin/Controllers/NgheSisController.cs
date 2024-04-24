using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA_Web.Models;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;

namespace DA_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NgheSisController : Controller
    {
        private readonly Web_Context _context;

        public NgheSisController(Web_Context context)
        {
            _context = context;
        }

        // GET: NgheSis
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var ngheSi = await _context.NgheSi.ToListAsync();

            int pageSize = 8;
            int totalPages = (int)Math.Ceiling((double)ngheSi.Count() / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

            var paginatedNS = ngheSi
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = pageNumber;
            return View(paginatedNS);
        }

        // GET: NgheSis/Details/5
        public async Task<IActionResult> Details(int? id, int pageNumber = 1)
        {
            List<BaiHat> listBaiHat = new List<BaiHat>();
            var CT_NgheSi = await _context.BaiHat_NgheSi
                            .Where(dsp => dsp.NgheSiID == id)
                            .Include(dsp => dsp.baiHat)
                            .ToListAsync();
            if (CT_NgheSi != null)
            {
                foreach (var danhSachBaiHat in CT_NgheSi)
                {
                    var baiHat = danhSachBaiHat.baiHat;
                    listBaiHat.Add(baiHat);
                }
            }
            ViewBag.NgheSi = await _context.NgheSi.FindAsync(id);
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

        // GET: NgheSis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NgheSis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Date,imageFile,QuocGia")] NgheSi ngheSi, IFormFile imageFile)
        {
            ModelState.Remove("imageFile");
            ModelState.Remove("danhSachPhats");
            if (ngheSi.Date >= DateTime.Today)
            {
                ModelState.AddModelError("Date", "Ngày sinh phải nhỏ hơn ngày hiện tại.");
                return View(ngheSi);
            }
            if (ModelState.IsValid)
            {
                ngheSi.imageFile = await SaveFile(imageFile);
                _context.Add(ngheSi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ngheSi);
        }

        // GET: NgheSis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngheSi = await _context.NgheSi.FindAsync(id);
            if (ngheSi == null)
            {
                return NotFound();
            }
            return View(ngheSi);
        }

        // POST: NgheSis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Date,imageFile,QuocGia")] NgheSi ngheSi, IFormFile imageFile)
        {

            ModelState.Remove("imageFile");
            ModelState.Remove("danhSachPhats");
            if (ModelState.IsValid)
            {
                try
                {
                    ngheSi.imageFile = await SaveFile(imageFile);
                    _context.Update(ngheSi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NgheSiExists(ngheSi.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ngheSi);
        }

        // GET: NgheSis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ngheSi = await _context.NgheSi
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ngheSi == null)
            {
                return NotFound();
            }

            return View(ngheSi);
        }

        // POST: NgheSis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ngheSi = await _context.NgheSi.FindAsync(id);
            if (ngheSi != null)
            {
                _context.NgheSi.Remove(ngheSi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NgheSiExists(int id)
        {
            return _context.NgheSi.Any(e => e.Id == id);
        }


        private async Task<string> SaveFile(IFormFile file)
        {
            var savePath = Path.Combine("wwwroot/images/artists", file.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return "/images/artists/" + file.FileName;

        }

        public async Task<IActionResult> ViewAlbum(int id, int pageNumber = 1)
        {
            var listDanhSach = await _context.DanhSachPhat.Where(p => p.TL_DanhSachID == 1 && p.NgheSiId == id).ToListAsync();
            int pageSize = 8;
            int totalPages = (int)Math.Ceiling((double)listDanhSach.Count() / pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

            var paginated = listDanhSach
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = pageNumber;
            return View(paginated);
        }
    }
}
