using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models.Gallery;
using MVC_Project.Services.Gallery;
using MVC_Project.Constants;
using MVC_Project.Extensions;
using MVC_Project.Models.Auth;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MVC_Project.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IGalleryService _galleryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly MVC_Project.Options.ApiOptions _apiOptions;

        public GalleryController(IGalleryService galleryService, IHttpContextAccessor httpContextAccessor, HttpClient httpClient, Microsoft.Extensions.Options.IOptions<MVC_Project.Options.ApiOptions> apiOptions)
        {
            _galleryService = galleryService;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _apiOptions = apiOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Image(string path, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(path)) return NotFound();

            var baseUrl = (_apiOptions.BaseUrl ?? string.Empty).TrimEnd('/');
            if (string.IsNullOrWhiteSpace(baseUrl)) return NotFound();

            var remoteUrl = path.StartsWith("http") ? path : $"{baseUrl}/{path.TrimStart('/')}";

            try
            {
                using var response = await _httpClient.GetAsync(remoteUrl, ct);
                if (!response.IsSuccessStatusCode) return NotFound();

                var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
                var bytes = await response.Content.ReadAsByteArrayAsync(ct);

                return File(bytes, contentType);
            }
            catch
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Index()
        {
            var response = await _galleryService.GetGalleryHeadingsAsync();
            var viewModel = new GalleryViewModel
            {
                GalleryHeadings = response.IsSuccess ? response.ResponseData : new List<GalleryHeading>()
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetGalleryItems(int gid)
        {
            var response = await _galleryService.GetGalleryItemsAsync(gid);
            return Json(new { isSuccess = response.IsSuccess, data = response.ResponseData ?? new List<GalleryItem>(), message = response.Message });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int gdid)
        {
            var result = await _galleryService.DeleteGalleryItemAsync(gdid);
            return Json(new { isSuccess = result.IsSuccess, message = result.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(GalleryViewModel model)
        {
            if (model.Photo == null || model.SelectedGid == 0)
            {
                TempData["Error"] = "Please select a gallery and a photo.";
                return RedirectToAction(nameof(Index));
            }

            var session = _httpContextAccessor.HttpContext?.Session.GetObject<UserSession>(SessionKeys.UserSession);
            if (session == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            string base64Image = "";
            if (model.Photo != null && model.Photo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await model.Photo.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    base64Image = Convert.ToBase64String(fileBytes);
                }
            }

            var request = new GalleryAddRequest
            {
                Gdid = 0,
                Gidfk = model.SelectedGid,
                PhotoPath = base64Image,
                AddedBy = session.UserName ?? "Admin"
            };

            var result = await _galleryService.AddGalleryAsync(request);

            if (result.IsSuccess)
            {
                TempData["Success"] = result.Message ?? "Photo uploaded successfully!";
            }
            else
            {
                TempData["Error"] = result.Message ?? "Failed to upload photo.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddGalleryHeading(string headingName)
        {
            if (string.IsNullOrWhiteSpace(headingName))
            {
                return Json(new { isSuccess = false, message = "Heading name cannot be empty." });
            }

            var request = new GalleryHeadingRequest
            {
                GalleryHeadingName = headingName
            };

            var result = await _galleryService.AddGalleryHeadingAsync(request);
            return Json(new { isSuccess = result.IsSuccess, message = result.Message });
        }
    }
}
