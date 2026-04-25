using MVC_Project.Models.Performance;
using MVC_Project.Services.Helper;
using System.Threading;
using System.Threading.Tasks;

namespace MVC_Project.Services.Performance
{
    public interface ITeacherPerformanceService
    {
        Task<EmployeePerformanceResponse?> GetEmployeesPerformanceAsync(string type, CancellationToken ct = default);
        Task<ApiResponse<object>> UpdatePerformanceAsync(PerformanceUpdateRequest request, CancellationToken ct = default);
    }
}
