namespace MVC_Project.Models.Notification
{
    public class ContactUpdateRequest
    {
        public int Cid { get; set; }
        public string PersonalName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Filepath { get; set; } = string.Empty;
        public int ActionType { get; set; }
    }
}