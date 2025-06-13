namespace MessengerClone.API.Hubs.Chathub.DTOs
{
    class ChatDto
    {
        public bool IsGroup { get; set; }
        public string Title { get; set; } 
        public string GroupPhotoUrl { get; set; }
        public string? Description { get; set; }
        public int CreatedBy { get; set; }
    }
}
