using HospitalAppointmentMVC.Models;

namespace HospitalAppointmentMVC.Extensions
{
    // Defines extension methods for Doctor.
    public static class DoctorExtensions
    {
        // Calculates the total consultation charge.
        public static decimal CalculateTotalCharge(this Doctor doctor)
        {
            // Adds the service charge to the consultation fee.
            return doctor.ConsultationFee + 100;
        }
    }
}