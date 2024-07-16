using API_MVC.Models;
using API_MVC.Models.ViewModel;
using API_MVC.Repository;
using API_MVC.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace API_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INationalParkRepo _nationalParkRepo;
        private readonly ITrailParkRepository _trailParkRepository;
        private readonly IBookingRepo _bookingRepo;

        public HomeController(ILogger<HomeController> logger, 
            INationalParkRepo nationalParkRepo, ITrailParkRepository trailParkRepository, IBookingRepo bookingRepo)
        {
            _logger = logger;
            _nationalParkRepo= nationalParkRepo;
            _trailParkRepository= trailParkRepository;
            _bookingRepo= bookingRepo;

        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexViewModel = new IndexViewModel()
            {
                NationalParkList = await _nationalParkRepo.GetAllAsync(SD.NationalParkAPIPath),
                TrailList=await _trailParkRepository.GetAllAsync(SD.TrailAPIPath),
              //  Bookinglist=await _bookingRepo.GetAllAsync(SD.BookingAPIPath)
            };
            return View(indexViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
