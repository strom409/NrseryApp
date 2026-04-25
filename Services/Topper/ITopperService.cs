using MVC_Project.Models.Topper;
using MVC_Project.Services.Helper;
using System.Threading;
using System.Threading.Tasks;

namespace MVC_Project.Services.Topper
{
    public interface ITopperService
    {
        Task<TopperResponse?> GetToppersAsync(CancellationToken ct = default);
        Task<ApiResponse<object>> SaveTopperAsync(TopperAddRequest request, CancellationToken ct = default);
        Task<ApiResponse<object>> DeleteTopperAsync(long tid, CancellationToken ct = default);
    }
}
