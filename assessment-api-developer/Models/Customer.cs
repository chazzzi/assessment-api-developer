using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace assessment_platform_developer.Models
{
    public class Country
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<State> States { get; set; }
    }

    public class State
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [ForeignKey("Country")]
        public int CountryID { get; set; }

        public virtual Country Country { get; set; }
    }

    [Serializable]
    public class Customer
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(100)] // Adjust length as needed
        public string Name { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string City { get; set; }

        // Foreign key to State
        [Required]
        [ForeignKey("State")]
        public int StateID { get; set; }

        [JsonIgnore]
        public virtual State State { get; set; }

        [Required]
        public string Zip { get; set; }

        [Required]
        [ForeignKey("Country")]
        public int CountryID { get; set; }

        [JsonIgnore]
        public virtual Country Country { get; set; }

        public string Notes { get; set; }

        public string ContactName { get; set; }

        [Phone]
        public string ContactPhone { get; set; }

        [EmailAddress]
        public string ContactEmail { get; set; }

        public string ContactTitle { get; set; }
        public string ContactNotes { get; set; }
    }
}
