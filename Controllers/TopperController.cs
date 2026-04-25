using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models.Topper;
using MVC_Project.Services.Topper;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MVC_Project.Controllers
{
    public class TopperController : Controller
    {
        private readonly ITopperService _topperService;
        private readonly IWebHostEnvironment _environment;

        public TopperController(ITopperService topperService, IWebHostEnvironment environment)
        {
            _topperService = topperService;
            _environment = environment;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var response = await _topperService.GetToppersAsync(ct);
            var viewModel = new TopperViewModel
            {
                Toppers = response?.Data ?? new List<TopperDto>()
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new TopperViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TopperViewModel model, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(model.TopperName) || string.IsNullOrWhiteSpace(model.Marks))
            {
                ModelState.AddModelError("", "Name and Marks are required.");
                return View(model);
            }

            string fileName = "";
            if (model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "toppers");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.PhotoFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PhotoFile.CopyToAsync(fileStream, ct);
                }
            }
            else if (!string.IsNullOrWhiteSpace(model.ExistingPhoto))
            {
                fileName = model.ExistingPhoto;
            }
            else
            {
                ModelState.AddModelError("PhotoFile", "Photo is required.");
                 return View(model);
            }

            var request = new TopperAddRequest
            {
                TopperName = model.TopperName,
                Marks = model.Marks,
                Photo = fileName
            };

            var result = await _topperService.SaveTopperAsync(request, ct);

            if (result != null && result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message ?? "Topper saved successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result?.Message ?? "Failed to save topper.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id, CancellationToken ct)
        {
            var result = await _topperService.DeleteTopperAsync(id, ct);
            if (result != null && result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message ?? "Topper deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result?.Message ?? "Failed to delete topper.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
