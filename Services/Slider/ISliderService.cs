using MVC_Project.Models.Slider;

namespace MVC_Project.Services.Slider
{
    public interface ISliderService
    {
        Task<SliderResponse?> GetAllSlidersAsync(CancellationToken ct = default);
        Task<bool> AddSliderAsync(SliderRequest request, CancellationToken ct = default);
    }
}
