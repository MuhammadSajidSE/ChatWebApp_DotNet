namespace ChatWebApp.DTOLayer
{
    public class CombinedContactViewModel
    {
        public int UserId { get; set; }
        public int ContactId { get; set; }
        public DateTime LastMessage { get; set; }
        public string? Name { get; set; } // From Contacts only
        public string? ContactNumber { get; set; } // From UsavedContact only
    }
}
