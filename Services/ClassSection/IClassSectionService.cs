using System.Collections.Generic;
using System.Threading.Tasks;
using MVC_Project.Models;
using MVC_Project.Services.Helper;



namespace MVC_Project.Services.ClassSection
{
    public interface IClassSectionService
    {
        Task<ApiResponse<List<ClassSectionDto>>> GetClassSectionBySessionAsync(string session);
        Task<ApiResponse<List<SectionDto>>> GetSectionsByClassIdAsync(int classId);
        Task<ApiResponse<List<SubjectDto>>> GetSubjectsByClassIdAsync(int classId);
    }
}
