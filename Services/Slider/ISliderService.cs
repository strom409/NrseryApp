using MVC_Project.Models.Slider;
using MVC_Project.Services.Variety;

namespace MVC_Project.Services.Slider
{
    public interface ISliderService
    {
        Task<SliderResponse?> GetAllSlidersAsync(CancellationToken ct = default);
        Task<ApiResponse<object>> AddSliderAsync(SliderRequest request, CancellationToken ct = default);
    }
}
