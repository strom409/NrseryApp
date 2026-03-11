using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models.Slider;
using MVC_Project.Services.Slider;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;

namespace MVC_Project.Controllers
{
    public class SliderController : Controller
    {
        private readonly ISliderService _sliderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;

        public SliderController(ISliderService sliderService, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment)
        {
            _sliderService = sliderService;
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var response = await _sliderService.GetAllSlidersAsync(ct);
            var viewModel = new SliderViewModel
            {
                Sliders = response?.Data ?? new List<SliderData>()
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new SliderViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SliderViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid || model.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Image is required for a new slider.");
                var response = await _sliderService.GetAllSlidersAsync(ct);
                model.Sliders = response?.Data ?? new List<SliderData>();
                return View("Index", model);
            }

            var session = _httpContextAccessor.HttpContext?.Session.GetObject<UserSession>(SessionKeys.UserSession);
            if (session == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            string base64Image = "";
            string fileExtension = "";

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                // We'll read the file into a MemoryStream first
                using (var ms = new MemoryStream())
                {
                    await model.ImageFile.CopyToAsync(ms, ct);
                    var fileBytes = ms.ToArray();
                    
                    // 1. Save locally
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "sliders");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await System.IO.File.WriteAllBytesAsync(filePath, fileBytes, ct);

                    // 2. Convert to Base64 for API
                    base64Image = Convert.ToBase64String(fileBytes);
                    fileExtension = Path.GetExtension(model.ImageFile.FileName);
                }
            }

            var request = new SliderRequest
            {
                Title = model.Title,
                Description = model.Description,
                Filepath = base64Image,
                NotificationDate = DateTime.UtcNow,
                ActionType = 1,
                Username = session.UserName ?? "Admin",
                FileExtension = fileExtension
            };

            var result = await _sliderService.AddSliderAsync(request, ct);

            if (result)
            {
                TempData["Success"] = "Slider added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to add slider to API.");
            var slidersResponse = await _sliderService.GetAllSlidersAsync(ct);
            model.Sliders = slidersResponse?.Data ?? new List<SliderData>();
            return View("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string description, string filepath, CancellationToken ct)
        {
            var response = await _sliderService.GetAllSlidersAsync(ct);
            var slider = response?.Data?.FirstOrDefault(s => s.Description == description && s.Filepath == filepath);

            if (slider == null)
            {
                TempData["Error"] = "Slider not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new SliderViewModel
            {
                Title = slider.Title,
                Description = slider.Description,
                ExistingFilepath = slider.Filepath,
                OldDescription = slider.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SliderViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var session = _httpContextAccessor.HttpContext?.Session.GetObject<UserSession>(SessionKeys.UserSession);
            if (session == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            string base64Image = "";
            string fileExtension = "";

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await model.ImageFile.CopyToAsync(ms, ct);
                    var fileBytes = ms.ToArray();
                    
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "sliders");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePathLocal = Path.Combine(uploadsFolder, uniqueFileName);

                    await System.IO.File.WriteAllBytesAsync(filePathLocal, fileBytes, ct);

                    base64Image = Convert.ToBase64String(fileBytes);
                    fileExtension = Path.GetExtension(model.ImageFile.FileName);
                }
            }

            var request = new SliderRequest
            {
                Title = model.Title,
                Description = model.Description, // Use new or existing description
                Filepath = string.IsNullOrEmpty(base64Image) ? (model.ExistingFilepath ?? "") : base64Image,
                NotificationDate = DateTime.UtcNow,
                ActionType = 2, // Update Action Type
                Username = session.UserName ?? "Admin",
                FileExtension = fileExtension
            };

            var result = await _sliderService.AddSliderAsync(request, ct);

            if (result)
            {
                TempData["Success"] = "Slider updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Failed to update slider.");
            return View(model);
        }

    }
}
