using API.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_MVC.Models
{
    public class Booking 
    {
        public Booking()
        {
            CategoryA = 300;
            CategoryB = 200;
            CategoryC = 100;
            NumberOfPerson = 1;
            Count = 1; // Initialize to 0
            SelectedCategory = "CategoryA"; // Default selection
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public int NationalParkId { get; set; }
        [ForeignKey("NationalParkId")]
        public NationalPark NationalPark { get; set; }

        public DateTime Date { get; set; }

        [Display(Name = "Booking Status")]
        public bool BookingStatus { get; set; }

        [Display(Name = "Total")]
        public int Count { get; set; }

        [NotMapped]
        public int CategoryA { get; set; }
        [NotMapped]
        public int CategoryB { get; set; }
        [NotMapped]
        public int CategoryC { get; set; }

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
                "CategoryA" => NumberOfPerson <= CategoryA,
                "CategoryB" => NumberOfPerson <= CategoryB,
                "CategoryC" => NumberOfPerson <= CategoryC,
                _ => false
            };
        }
    }
}
