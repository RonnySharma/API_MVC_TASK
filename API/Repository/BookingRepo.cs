using API.Data;
using API.Models;
using API.Repository.IRepository;
using API_MVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace API.Repository
{
    public class BookingRepo : IBookingRepos
    {
        private readonly ApplicationDbContext _context;
        public BookingRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool BookingExists(int bookingId)
        {
            return _context.Bookings.Any(b => b.Id == bookingId);
        }

        public bool BookingExists(string bookingName)
        {
            return _context.Bookings.Any(b => b.Name == bookingName); // Use 'Name' instead of 'name'
        }

        public bool CreateBooking(Booking booking)
        {
            _context.Bookings.Add(booking);
            return Save();
        }

        public bool DeleteBooking(Booking booking)
        {
            _context.Bookings.Remove(booking);
            return Save();
        }

        public Booking GetBooking(int bookingId)
        {
            return _context.Bookings.Find(bookingId);
        }

        public ICollection<Booking> GetBookings()
        {
            return _context.Bookings.ToList();
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0; // Return true if changes were saved
        }

        public bool UpdateeBooking(Booking booking)
        {
            _context.Bookings.Update(booking);
            return Save();
        }
    }
}
