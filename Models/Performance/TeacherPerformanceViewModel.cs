using System.Collections.Generic;

namespace MVC_Project.Models.Performance
{
    public class TeacherPerformanceViewModel
    {
        public List<EmployeePerformanceItemViewModel> Employees { get; set; } = new List<EmployeePerformanceItemViewModel>();
        public string SessionYear { get; set; } = string.Empty;
    }

    public class EmployeePerformanceItemViewModel
    {
        public long EmployeeID { get; set; }
        public long EmployeeCode { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string EmployeePerformance { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
