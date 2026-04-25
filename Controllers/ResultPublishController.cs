using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MVC_Project.Models.Result;
using MVC_Project.Services.ClassSection;
using MVC_Project.Services.Result;

namespace MVC_Project.Controllers
{
    public class ResultPublishController : Controller
    {
        private readonly IClassSectionService _classSectionService;
        private readonly IResultPublishService _resultPublishService;
        private readonly IConfiguration _configuration;

        public ResultPublishController(
            IClassSectionService classSectionService,
            IResultPublishService resultPublishService,
            IConfiguration configuration)
        {
            _classSectionService = classSectionService;
            _resultPublishService = resultPublishService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            string session = _configuration["Session"] ?? "2025-26";
            var classesResponse = await _classSectionService.GetClassSectionBySessionAsync(session);
            
            var viewModel = new ResultPublishViewModel
            {
                Classes = classesResponse.ResponseData ?? new List<Models.ClassSectionDto>()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetStatus(int classId, CancellationToken ct)
        {
            var response = await _resultPublishService.GetResultStatusAsync(classId, ct);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveStatus([FromBody] ResultStatusRequest request, CancellationToken ct)
        {
            if (request == null) return Json(new { isSuccess = false, message = "Invalid request." });
            
            var response = await _resultPublishService.SaveResultStatusAsync(request, ct);
            return Json(response);
        }
    }
}
