using MVC_Project.Models.Gallery;
using MVC_Project.Services.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MVC_Project.Services.Gallery
{
    public interface IGalleryService
    {
        Task<ApiResponse<List<GalleryHeading>>> GetGalleryHeadingsAsync();
        Task<ApiResponse<List<GalleryItem>>> GetGalleryItemsAsync(int gid);
        Task<ApiResponse<object>> AddGalleryAsync(GalleryAddRequest request);
        Task<ApiResponse<object>> DeleteGalleryItemAsync(int gdid);
        Task<ApiResponse<object>> AddGalleryHeadingAsync(GalleryHeadingRequest request);
    }
}
