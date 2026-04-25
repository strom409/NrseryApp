using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models.Performance;
using MVC_Project.Services.Performance;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace MVC_Project.Controllers
{
    public class TeacherPerformanceController : Controller
    {
        private readonly ITeacherPerformanceService _performanceService;
        private readonly IConfiguration _configuration;

        public TeacherPerformanceController(ITeacherPerformanceService performanceService, IConfiguration configuration)
        {
            _performanceService = performanceService;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            string year = System.DateTime.Now.Year.ToString();
            
            // 1. Fetch main employee list
            string typeList = $"2_{year}";
            var responseList = await _performanceService.GetEmployeesPerformanceAsync(typeList, ct);
            
            // 2. Fetch existing performance data
            string typePerf = "1_0";
            var responsePerf = await _performanceService.GetEmployeesPerformanceAsync(typePerf, ct);
            
            // 3. Create a lookup for existing performance by EmployeeCode
            var performanceLookup = responsePerf?.Data?
                .Where(p => p.EmployeeCode.ValueKind != JsonValueKind.Null && p.EmployeeCode.ValueKind != JsonValueKind.Undefined)
                .ToDictionary(
                    p => p.EmployeeCode.ToString(), 
                    p => p.Performance,
                    StringComparer.OrdinalIgnoreCase) ?? new Dictionary<string, string>();

            var viewModel = new TeacherPerformanceViewModel
            {
                SessionYear = year,
                Employees = responseList?.Data?.Select(e => 
                {
                    string code = e.EmployeeCode.ToString();
                    return new EmployeePerformanceItemViewModel
                    {
                        EmployeeID = e.EmployeeID,
                        EmployeeCode = long.TryParse(code, out long c) ? c : 0,
                        EmployeeName = e.EmployeeName,
                        FatherName = e.FatherName,
                        // Priority: API 2 performance > API 1 FieldValue (if applicable)
                        EmployeePerformance = performanceLookup.ContainsKey(code) 
                            ? performanceLookup[code] 
                            : (e.FieldValue == "NA" ? "" : e.FieldValue)
                    };
                }).ToList() ?? new List<EmployeePerformanceItemViewModel>()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(ValueCountLimit = 10000)]
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> SavePerformance(TeacherPerformanceViewModel model, CancellationToken ct)
        {
            var selectedEmployees = model.Employees.Where(e => e.IsSelected).ToList();
            
            if (!selectedEmployees.Any())
            {
                TempData["ErrorMessage"] = "Please select at least one employee to update.";
                return RedirectToAction(nameof(Index));
            }

            int successCount = 0;
            int failCount = 0;

            foreach (var employee in selectedEmployees)
            {
                var request = new PerformanceUpdateRequest
                {
                    Edi = employee.EmployeeID,
                    EmployeeCode = employee.EmployeeCode.ToString(),
                    Performance = string.IsNullOrWhiteSpace(employee.EmployeePerformance) ? "Good" : employee.EmployeePerformance
                };

                var result = await _performanceService.UpdatePerformanceAsync(request, ct);
                if (result != null && result.IsSuccess)
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                }
            }

            if (failCount == 0)
            {
                TempData["SuccessMessage"] = $"Successfully updated performance for {successCount} employees.";
            }
            else
            {
                TempData["WarningMessage"] = $"Updated {successCount} employees, but {failCount} failed.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
