using System.Threading;
using System.Threading.Tasks;
using MVC_Project.Models.Result;
using MVC_Project.Services.Helper;

namespace MVC_Project.Services.Result
{
    public interface IResultPublishService
    {
        Task<ResultStatusResponse?> GetResultStatusAsync(int classId, CancellationToken ct = default);
        Task<ApiResponse<object>> SaveResultStatusAsync(ResultStatusRequest request, CancellationToken ct = default);
    }
}
