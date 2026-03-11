using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MVC_Project.Models.Slider;
using MVC_Project.Options;
using MVC_Project.Services.Slider;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;
using System.Net.Http;

namespace MVC_Project.Controllers
{
    public class SliderController : Controller
    {
        private readonly ISliderService _sliderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiOptions _apiOptions;

        public SliderController(
            ISliderService sliderService,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment environment,
            IHttpClientFactory httpClientFactory,
            IOptions<ApiOptions> apiOptions)
        {
            _sliderService = sliderService;
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
            _httpClientFactory = httpClientFactory;
            _apiOptions = apiOptions.Value;
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

        /// <summary>
        /// Proxies slider images from the API server so they are served over HTTPS
        /// and avoid mixed-content blocking in the browser.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Image(string path, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return NotFound();
            }

            var baseUrl = (_apiOptions.BaseUrl ?? string.Empty).TrimEnd('/');
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return NotFound();
            }

            var relativePath = path.TrimStart('/');
            var remoteUrl = $"{baseUrl}/{relativePath}";

            var client = _httpClientFactory.CreateClient();
            using var response = await client.GetAsync(remoteUrl, ct);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
            var bytes = await response.Content.ReadAsByteArrayAsync(ct);

            return File(bytes, contentType);
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
                // Read the file and convert directly to Base64 (no local save)
                using (var ms = new MemoryStream())
                {
                    await model.ImageFile.CopyToAsync(ms, ct);
                    var fileBytes = ms.ToArray();
                    base64Image = Convert.ToBase64String(fileBytes);
                    fileExtension = Path.GetExtension(model.ImageFile.FileName);
                }
            }

            var request = new SliderRequest
            {
                NotificationID = 0,
                Title = model.Title,
                Description = model.Description,
                Filepath = base64Image,
                NotificationDate = DateTime.UtcNow,
                ActionType = 1,
                Username = session.UserName ?? "Admin",
                FileExtension = fileExtension
            };

            var result = await _sliderService.AddSliderAsync(request, ct);

            if (result != null && result.IsSuccess)
            {
                TempData["Success"] = string.IsNullOrWhiteSpace(result.Message)
                    ? "Slider created successfully."
                    : result.Message;
                return RedirectToAction(nameof(Index));
            }

            var errorMessage = result?.Message ?? "Failed to add slider to API.";
            ModelState.AddModelError(string.Empty, errorMessage);
            var slidersResponse = await _sliderService.GetAllSlidersAsync(ct);
            model.Sliders = slidersResponse?.Data ?? new List<SliderData>();
            return View("Index", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int notificationId, string description, string filepath, CancellationToken ct)
        {
            var response = await _sliderService.GetAllSlidersAsync(ct);
            var slider = response?.Data?.FirstOrDefault(s =>
                s.NotificationID == notificationId ||
                (s.Description == description && s.Filepath == filepath));

            if (slider == null)
            {
                TempData["Error"] = "Slider not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new SliderViewModel
            {
                NotificationID = slider.NotificationID,
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
                // New image selected: read and convert to Base64 (no local save)
                using (var ms = new MemoryStream())
                {
                    await model.ImageFile.CopyToAsync(ms, ct);
                    var fileBytes = ms.ToArray();
                    base64Image = Convert.ToBase64String(fileBytes);
                    fileExtension = Path.GetExtension(model.ImageFile.FileName);
                }
            }
            else if (!string.IsNullOrWhiteSpace(model.ExistingFilepath))
            {
                // No new image: fetch existing image from API server and send again as Base64
                var baseUrl = (_apiOptions.BaseUrl ?? string.Empty).TrimEnd('/');
                if (!string.IsNullOrWhiteSpace(baseUrl))
                {
                    var relativePath = model.ExistingFilepath.TrimStart('/');
                    var remoteUrl = $"{baseUrl}/{relativePath}";

                    var client = _httpClientFactory.CreateClient();
                    var fileBytes = await client.GetByteArrayAsync(remoteUrl, ct);

                    base64Image = Convert.ToBase64String(fileBytes);
                    fileExtension = Path.GetExtension(relativePath) ?? string.Empty;
                }
            }

            var request = new SliderRequest
            {
                NotificationID = model.NotificationID,
                Title = model.Title,
                Description = model.Description,
                Filepath = base64Image,
                NotificationDate = DateTime.UtcNow,
                ActionType = 2, // Update Action Type
                Username = session.UserName ?? "Admin",
                FileExtension = fileExtension
            };

            var result = await _sliderService.AddSliderAsync(request, ct);

            if (result != null && result.IsSuccess)
            {
                TempData["Success"] = string.IsNullOrWhiteSpace(result.Message)
                    ? "Slider updated successfully."
                    : result.Message;
                return RedirectToAction(nameof(Index));
            }

            var errorMessage = result?.Message ?? "Failed to update slider.";
            ModelState.AddModelError(string.Empty, errorMessage);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int notificationId, string title, string description, string filepath, CancellationToken ct)
        {
            var session = _httpContextAccessor.HttpContext?.Session.GetObject<UserSession>(SessionKeys.UserSession);
            if (session == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var request = new SliderRequest
            {
                NotificationID = notificationId,
                Title = title,
                Description = description,
                Filepath = string.Empty,
                NotificationDate = DateTime.UtcNow,
                ActionType = 3, // Delete
                Username = session.UserName ?? "Admin",
                FileExtension = string.Empty
            };

            var result = await _sliderService.AddSliderAsync(request, ct);

            if (result != null && result.IsSuccess)
            {
                TempData["Success"] = string.IsNullOrWhiteSpace(result.Message)
                    ? "Slider deleted successfully."
                    : result.Message;
            }
            else
            {
                TempData["Error"] = result?.Message ?? "Failed to delete slider.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
