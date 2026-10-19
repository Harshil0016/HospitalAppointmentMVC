namespace HospitalAppointmentMVC.Models
{
    // Stores the details of a doctor.
    public class Doctor
    {
        // Stores the unique doctor ID.
        public int DoctorId { get; set; }

        // Stores the doctor's name.
        public string DoctorName { get; set; } = "";

        // Stores the doctor's specialization.
        public string Specialization { get; set; } = "";

        // Stores the doctor's experience in years.
        public int Experience { get; set; }

        // Stores the consultation fee.
        public decimal ConsultationFee { get; set; }
    }
}