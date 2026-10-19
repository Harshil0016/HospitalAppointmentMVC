using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using HospitalAppointmentMVC.Models;
using HospitalAppointmentMVC.Extensions;
using System.Text.Json;

namespace HospitalAppointmentMVC.Controllers
{
    public class HomeController : Controller
    {
        // Provides access to the memory cache.
        private readonly IMemoryCache _cache;

        // Creates an HTTP client factory for API requests.
        private readonly IHttpClientFactory _httpClientFactory;

        // Initializes the controller dependencies.
        public HomeController(
            IMemoryCache cache,
            IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            _httpClientFactory = httpClientFactory;
        }

        // Displays the doctor list.
        public IActionResult Index()
        {
            // Retrieves cached doctors or creates the list.
            var doctors = _cache.GetOrCreate("DoctorList", entry =>
            {
                // Sets the cache expiration to three minutes.
                entry.AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(3);

                // Creates sample doctor records.
                return new List<Doctor>
                {
                    new Doctor
                    {
                        DoctorId = 1,
                        DoctorName = "Dr. Amit Patel",
                        Specialization = "Cardiologist",
                        Experience = 10,
                        ConsultationFee = 500
                    },
                    new Doctor
                    {
                        DoctorId = 2,
                        DoctorName = "Dr. Neha Shah",
                        Specialization = "Dermatologist",
                        Experience = 7,
                        ConsultationFee = 400
                    },
                    new Doctor
                    {
                        DoctorId = 3,
                        DoctorName = "Dr. Raj Mehta",
                        Specialization = "Neurologist",
                        Experience = 12,
                        ConsultationFee = 700
                    }
                };
            });

            // Displays the doctors in the view.
            return View(doctors);
        }

        // Displays details of the selected doctor.
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public IActionResult Details(int id)
        {
            // Gets the doctor list from the cache.
            var doctors = _cache.Get<List<Doctor>>("DoctorList");

            // Handles a missing or expired cache entry.
            if (doctors == null)
            {
                return RedirectToAction("Index");
            }

            // Finds the doctor using the query string ID.
            var doctor = doctors.FirstOrDefault(d => d.DoctorId == id);

            // Returns not found if the doctor does not exist.
            if (doctor == null)
            {
                return NotFound();
            }

            // Calculates the total consultation charge.
            ViewBag.TotalCharge = doctor.CalculateTotalCharge();

            // Displays the selected doctor's details.
            return View(doctor);
        }

        // Displays the appointment form.
        [HttpGet]
        public IActionResult Appointment(int id)
        {
            // Passes the doctor ID to the appointment view.
            ViewBag.DoctorId = id;

            // Displays the appointment form.
            return View();
        }

        // Processes the submitted appointment form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Appointment(
            int doctorId,
            string patientName,
            string specialization)
        {
            // Saves the patient name in a cookie.
            Response.Cookies.Append("PatientName", patientName, new CookieOptions
            {
                // Keeps the cookie for one day.
                Expires = DateTimeOffset.UtcNow.AddDays(1),

                // Prevents JavaScript access to the cookie.
                HttpOnly = true
            });

            // Saves the selected specialization in session.
            HttpContext.Session.SetString(
                "Specialization", specialization);

            // Passes submitted details to the confirmation view.
            ViewBag.PatientName = patientName;
            ViewBag.Specialization = specialization;
            ViewBag.DoctorId = doctorId;

            // Displays the appointment confirmation.
            return View("Appointment");
        }

        // Retrieves posts from an external API asynchronously.
        public async Task<IActionResult> Posts()
        {
            // Creates an HTTP client.
            var client = _httpClientFactory.CreateClient();

            // Sends an asynchronous request to JSONPlaceholder.
            var response = await client.GetAsync(
                "https://jsonplaceholder.typicode.com/posts");

            // Checks whether the API request succeeded.
            response.EnsureSuccessStatusCode();

            // Reads the API response asynchronously.
            var json = await response.Content.ReadAsStringAsync();

            // Converts the JSON response into post objects.
            var posts = JsonSerializer.Deserialize<List<ApiPost>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            posts = posts?.Take(3).ToList();

            // Displays the posts in a table.
            return View(posts ?? new List<ApiPost>());
        }
    }
}