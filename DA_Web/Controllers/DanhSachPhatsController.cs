using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DA_Web.Models;
using System.Security.Claims;

namespace DA_Web.Controllers
{
    public class DanhSachPhatsController : Controller
    {
        private readonly Web_Context _context;

        public DanhSachPhatsController(Web_Context context)
        {
            _context = context;
        }

        // GET: DanhSachPhats
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            string userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }
            var listPlayList = await _context.DanhSachPhat.Where(p => p.UserID == userId && p.TL_DanhSachID == 2).Include(d => d.TL_DanhSach).Include(d => d.User).ToListAsync();
            int pageSize = 8;
            int totalPages = (int)Math.Ceiling((double)listPlayList.Count/ pageSize);
            pageNumber = Math.Max(1, Math.Min(pageNumber, totalPages));

            var paginated = listPlayList
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();
            ViewBag.TotalPages = totalPages;
            ViewBag.PageNumber = pageNumber;
            return View(paginated);
        }

        // GET: DanhSachPhats/Details/5
        public async Task<IActionResult> Details(int? id, int pageNumber = 1)
        {
            List<BaiHat> listBaiHat = new List<BaiHat>();
            string userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }
            var danhSachPhat = await _context.DanhSachPhat.FindAsync(id);


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
            ViewBag.NamePlaylist = danhSachPhat;
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

        // GET: DanhSachPhats/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: DanhSachPhats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,SoLuong,imageFile,NgheSiId,UserID,TL_DanhSachID")] DanhSachPhat danhSachPhat)
        {
            ModelState.Remove("TL_DanhSach");
            ModelState.Remove("NgheSiId");
            ModelState.Remove("UserID");
            ModelState.Remove("SoLuong");
            ModelState.Remove("imageFile");
            if (ModelState.IsValid)
            {
                string userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Redirect("/Identity/Account/Login");
                }
                danhSachPhat.UserID = userId;
                danhSachPhat.TL_DanhSachID = 2;
                danhSachPhat.SoLuong = 0;
                _context.Add(danhSachPhat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(danhSachPhat);
        }

        // GET: DanhSachPhats/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhSachPhat = await _context.DanhSachPhat.FindAsync(id);
            if (danhSachPhat == null)
            {
                return NotFound();
            }
            return View(danhSachPhat);
        }

        // POST: DanhSachPhats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,SoLuong,imageFile,NgheSiId,UserID,TL_DanhSachID")] DanhSachPhat danhSachPhat)
        {
            string userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }
            ModelState.Remove("id");
            ModelState.Remove("Name");
            ModelState.Remove("TL_DanhSach");
            if (ModelState.IsValid)
            {
                try { 
                    danhSachPhat.UserID = userId;
                    danhSachPhat.TL_DanhSachID = 2;
                    _context.Update(danhSachPhat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhSachPhatExists(danhSachPhat.ID))
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

            return View(danhSachPhat);
        }

        // GET: DanhSachPhats/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhSachPhat = await _context.DanhSachPhat
                .Include(d => d.NgheSi)
                .Include(d => d.TL_DanhSach)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (danhSachPhat == null)
            {
                return NotFound();
            }

            return View(danhSachPhat);
        }

        // POST: DanhSachPhats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var danhSachPhat = await _context.DanhSachPhat.FindAsync(id);
            if (danhSachPhat != null)
            {
                _context.DanhSachPhat.Remove(danhSachPhat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanhSachPhatExists(int id)
        {
            return _context.DanhSachPhat.Any(e => e.ID == id);
        }


        public async Task<IActionResult> AddPlaylist(int DSid, int BHid)
        {
           
            string userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Redirect("/Identity/Account/Login");
            }
            var baiHat = await _context.BaiHat.FindAsync(BHid);
            var danhSachPhat = await _context.DanhSachPhat.FindAsync(DSid);
            var find = await _context.DanhSach_BaiHat.Where(a => a.DanhSachID == danhSachPhat.ID && a.BaiHatID == baiHat.Id).FirstOrDefaultAsync();
            if (find == null)
            {
                DanhSach_BaiHat danhSach_BaiHat = new DanhSach_BaiHat();
                danhSach_BaiHat.DanhSach = danhSachPhat;
                danhSach_BaiHat.BaiHat = baiHat;
                _context.DanhSach_BaiHat.Add(danhSach_BaiHat);
                danhSachPhat.SoLuong++;
            }
            await _context.SaveChangesAsync();
            

            return RedirectToAction("Details", "BaiHats", new { id = BHid });
        }

        public async Task<IActionResult> XoaRaPlayList(int bhid, int dsid)
        {
            var danhSach_BaiHat = await _context.DanhSach_BaiHat.Where(p => p.BaiHatID == bhid && p.DanhSachID == dsid).FirstOrDefaultAsync();
            var danhSachPhat = await _context.DanhSachPhat.FindAsync(dsid);
            if (danhSach_BaiHat != null)
            {
                danhSachPhat.SoLuong--;
                _context.DanhSachPhat.Update(danhSachPhat);
                _context.DanhSach_BaiHat.Remove(danhSach_BaiHat);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = dsid });

        }
    }
}
