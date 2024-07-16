using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_MVC.Models
{
    public class Bookings
    {
        // Price per category
        private const int CategoryAPrice = 300;
        private const int CategoryBPrice = 200;
        private const int CategoryCPrice = 100;

        public Bookings()
        {
            NumberOfPerson = 1;
            SelectedCategory = "CategoryA";
            UpdateCount(); // Initialize Count based on default values
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public DateTime Date { get; set; }

        [Display(Name = "Booking Status")]
        public bool BookingStatus { get; set; }

        [Display(Name = "Total")]
        public int Count { get; set; }

        public int NationalParkId { get; set; }
        [ForeignKey("NationalParkId")]
        public NationalPark NationalPark { get; set; }

        [Display(Name = "Number Of Person")]
        public int NumberOfPerson { get; set; }

        // New property for selected category
        [Display(Name = "Select Category")]
        public string SelectedCategory { get; set; }

        // Method to validate booking
        public bool ValidateBooking(string category)
        {
            return category switch
            {
                "CategoryA" => NumberOfPerson <= 3, 
                
                // Assume maximum for demonstration
                "CategoryB" => NumberOfPerson <= 3,
                "CategoryC" => NumberOfPerson <= 3,
                _ => false
            };
    


        }

        // Method to update the Count based on the selected category and number of persons
        public void UpdateCount()
        {
            Count = NumberOfPerson * (SelectedCategory switch
            {
                "CategoryA" => CategoryAPrice,
                "CategoryB" => CategoryBPrice,
                "CategoryC" => CategoryCPrice,
                _ => 0,
            });
        }
    }
}
