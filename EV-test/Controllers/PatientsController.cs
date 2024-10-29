using EV_test.Models.ViewModel;
using EV_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EV_test.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IWebHostEnvironment _he;
        private readonly PatientDbContext _context;
        public PatientsController(IWebHostEnvironment _he, PatientDbContext _context)
        {
            this._context = _context;
            this._he = _he;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _context.Patients.Include(x => x.PatientReports).ThenInclude(y => y.Report).ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult AddNewSkills(int? id)
        {
            ViewBag.skill = new SelectList(_context.Reports, "ReportId", "ReportName", id.ToString() ?? "");
            return PartialView("_AddNewSkills");
        }
        [HttpPost]
        public async Task<IActionResult> Create(PatientVM VM, int[] ReportId)
        {
            if (ModelState.IsValid)
            {
                Patient patient = new Patient()
                {
                    PatientName = VM.PatientName,
                    DateOfBirth = VM.DateOfBirth,
                    Phone = VM.Phone,
                    Emergency = VM.Emergency
                };
                //image
                var file = VM.ImagePath;
                string webroot = _he.WebRootPath;
                string folder = "Images";
                string imgFileName = DateTime.Now.Ticks.ToString() + "_" + Path.GetFileName(VM.ImagePath.FileName);
                string fileToSave = Path.Combine(webroot, folder, imgFileName);

                if (file != null)
                {
                    using (var stream = new FileStream(fileToSave, FileMode.Create))
                    {
                        VM.ImagePath.CopyTo(stream);
                        patient.Image = "/" + folder + "/" + imgFileName;
                    }
                }
                foreach (var item in ReportId)
                {
                    PatientReport patientReport = new PatientReport()
                    {
                        Patient = patient,
                        PatientId = patient.PatientId,
                        ReportId = item
                    };
                    _context.PatientReports.Add(patientReport);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.PatientId == id);

            PatientVM VM = new PatientVM()
            {
                PatientId = patient.PatientId,
                PatientName = patient.PatientName,
                DateOfBirth = patient.DateOfBirth,
                Phone = patient.Phone,
                Image = patient.Image,
                Emergency = patient.Emergency
            };
            var existReport = _context.PatientReports.Where(x => x.PatientId == id).ToList();
            foreach (var item in existReport)
            {
                VM.ReportList.Add(item.ReportId);
            }
            return View(VM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PatientVM VM, int[] ReportId)
        {
            if (ModelState.IsValid)
            {
                Patient patient = new Patient()
                {
                    PatientId = VM.PatientId,
                    PatientName = VM.PatientName,
                    DateOfBirth = VM.DateOfBirth,
                    Phone = VM.Phone,
                    Emergency = VM.Emergency,
                    Image = VM.Image
                };
                var file = VM.ImagePath;
                string existImg = VM.Image;

                if (file != null)
                {
                    string webroot = _he.WebRootPath;
                    string folder = "Images";
                    string imgFileName = DateTime.Now.Ticks.ToString() + "_" + Path.GetFileName(VM.ImagePath.FileName);
                    string fileToSave = Path.Combine(webroot, folder, imgFileName);
                    using (var stream = new FileStream(fileToSave, FileMode.Create))
                    {
                        VM.ImagePath.CopyTo(stream);
                        patient.Image = "/" + folder + "/" + imgFileName;
                    }

                }
                else
                {
                    patient.Image = existImg;
                }

                var existReport = _context.PatientReports.Where(x => x.PatientId == patient.PatientId).ToList();
                //Remove
                foreach (var item in existReport)
                {
                    _context.PatientReports.Remove(item);
                }
                //Add
                foreach (var item in ReportId)
                {
                    PatientReport patientReport = new PatientReport()
                    {
                        PatientId = patient.PatientId,
                        ReportId = item
                    };
                    _context.PatientReports.Add(patientReport);
                }
                _context.Update(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public async Task<IActionResult> Delete(int? id)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.PatientId == id);
            var existReport = _context.PatientReports.Where(x => x.PatientId == id).ToList();
            foreach (var item in existReport)
            {
                _context.PatientReports.Remove(item);
            }

            _context.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
