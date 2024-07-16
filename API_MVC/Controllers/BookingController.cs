using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using API_MVC.Models;
using API_MVC.Repository.IRepository;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Stripe;
using API_MVC;
using static API_MVC.Twilio.Types;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace NP_APP.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingRepo _bookingRepository;
        private readonly INationalParkRepo _nationalParkRepository;

        public BookingController(IBookingRepo bookingRepository, INationalParkRepo nationalParkRepo)
        {
            _bookingRepository = bookingRepository;
            _nationalParkRepository = nationalParkRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region APIs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingRepository.GetAllAsync(SD.BookingAPIPath);
            return Json(new { data = bookings });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _bookingRepository.DeleteAsync(SD.BookingAPIPath, id);
            return Json(new { success = status, message = status ? "Deleted successfully" : "Deletion failed" });
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> UpdateCount(int? bookingId, int numberOfPersons)
        {
            if (!bookingId.HasValue) return NotFound();

            var booking = await _bookingRepository.GetAsync(SD.BookingAPIPath, bookingId.Value);
            if (booking == null) return NotFound();

            // Update the number of persons
            booking.NumberOfPerson = numberOfPersons;
            booking.UpdateCount(); // Update Count based on the new NumberOfPerson

            // Save changes to the database
            await _bookingRepository.UpdateAsync(SD.BookingAPIPath, booking);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Upsert(int? id, int? nationalParkId)
        {
            if (id == null)
            {
                if (!nationalParkId.HasValue)
                {
                    return BadRequest("National Park ID is required.");
                }

                var newBooking = new Bookings { Date = DateTime.Today, NationalParkId = nationalParkId.Value };
                return View(newBooking);
            }

            var existingBooking = await _bookingRepository.GetAsync(SD.BookingAPIPath, id.Value);
            if (existingBooking == null) return NotFound();

            return View(existingBooking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Bookings booking)
        {
       
            if (booking == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(booking);
            }

            var allBookings = await _bookingRepository.GetAllAsync(SD.BookingAPIPath);
            var bookingsGroupedByDate = allBookings.GroupBy(b => b.Date.Date);

            // Validate booking limits
            var categoryBookings = allBookings.Count(b => b.SelectedCategory == booking.SelectedCategory && b.Date.Date == booking.Date.Date);
            if (categoryBookings >= 2)
            {
                ModelState.AddModelError("", $"Cannot make more than 2 bookings for {booking.SelectedCategory} on this date.");
                return View(booking);
            }

            if (bookingsGroupedByDate.Any(group => group.Count() >= 2 && group.Key == booking.Date.Date))
            {
                ModelState.AddModelError("", "Two bookings are already made for this date. Please select another date.");
                return View(booking);
            }
            // Fetch the national park's capacity
            var nationalPark = await _nationalParkRepository.GetAsync(SD.NationalParkAPIPath, booking.NationalParkId);
            if (nationalPark == null)
            {
                ModelState.AddModelError("", "Selected National Park does not exist.");
                return View(booking);
            }

            var nationalParkBookings = allBookings.Count(b => b.NationalParkId == booking.NationalParkId && b.Date.Date == booking.Date.Date);
            if (nationalParkBookings >= nationalPark.Capacity)
            {
                ModelState.AddModelError("", $"Cannot make more than {nationalPark.Capacity} bookings for {nationalPark.Name} on this date.");
                return View(booking);
            }

            booking.UpdateCount();

            // Create or update the booking
            if (booking.Id == 0)
            {
                await _bookingRepository.CreateAsync(SD.BookingAPIPath, booking);
            }
            else
            {
                await _bookingRepository.UpdateAsync(SD.BookingAPIPath, booking);
            }

            // Redirect to payment successful page
            return RedirectToAction(nameof(PaymentSuccessful));
        }

        public IActionResult PaymentSuccessful()
        {
            if (!ModelState.IsValid) {
                string accountSid = "your-twilio-account-sid";
                string authToken = "your-twilio-auth-token";

                TwilioClient.Init(accountSid, authToken);
                string fromPhoneNumber = "+12183167899";
                string toPhoneNumber = "+917018324640";

                try
                {
                    var message = MessageResource.Create(
                        body: "Your payment was successful! Thank you for your booking.",
                        from: new PhoneNumber(fromPhoneNumber),
                        to: new PhoneNumber(toPhoneNumber)
                    );

                    Console.WriteLine("Message SID: " + message.Sid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            return View();
        }
        public async Task<bool> ProcessPaymentAsync(Bookings booking, string stripeToken)
        {
            StripeConfiguration.ApiKey = "your-stripe-secret-key";

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(booking.Count * 100), // Amount in cents
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                Description = "Booking payment",
                ReceiptEmail = booking.Email,
                PaymentMethod = stripeToken
            };

            var service = new PaymentIntentService();
            try
            {
                var paymentIntent = await service.CreateAsync(options);
                return paymentIntent.Status == "succeeded";
            }
            catch (StripeException ex)
            {
                Console.WriteLine("Stripe Exception: " + ex.Message);
                return false;
            }
        }

            public async Task <IActionResult> Details()
            {
                var bookings = await _bookingRepository.GetAllAsync(SD.BookingAPIPath);
                return View(bookings);

            }
        }
    }

