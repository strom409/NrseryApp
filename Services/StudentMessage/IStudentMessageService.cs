using MVC_Project.Models.StudentMessage;
using MVC_Project.Services.Helper;
using System.Threading.Tasks;

namespace MVC_Project.Services.StudentMessage
{
    public interface IStudentMessageService
    {
        Task<ApiResponse<object>> AddStudentMessageAsync(StudentMessageAddRequest model);
        Task<ApiResponse<List<StudentDetailDto>>> GetStudentsBySectionAsync(string sectionId);
        Task<ApiResponse<List<StudentMessageResponseDto>>> GetStudentMessagesAsync(string id);
    }
}
