using Microsoft.AspNetCore.Http;
using System;

namespace MVC_Project.Models.Notification
{
    public class DownloadUploadRequest
    {
        public string? NotificationID { get; set; }
        public string Title { get; set; }
        public IFormFile? File { get; set; }
        public string? FilePath { get; set; }
        public int ClassIdFk { get; set; }
        public int SubjectId { get; set; }
        public int UploadType { get; set; }
        public int ActionType { get; set; } = 1; // 1 for Add, 2 for Update
    }
}
