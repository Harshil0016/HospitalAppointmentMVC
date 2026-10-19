namespace HospitalAppointmentMVC.Models
{
    // Represents a post received from the API.
    public class ApiPost
    {
        // Stores the post ID.
        public int Id { get; set; }

        // Stores the post title.
        public string Title { get; set; } = "";
    }
}