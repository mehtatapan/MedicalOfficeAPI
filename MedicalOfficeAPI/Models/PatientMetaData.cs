using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MedicalOfficeAPI.Models
{
    public class PatientMetaData : Auditable, IValidatableObject
    {

        [Display(Name = "Patient ")]
        public string FullName
        {
            get
            {
                return FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? " " :
                        (" " + (char?)MiddleName[0] + ". ").ToUpper())
                    + LastName;
            }
        }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "The first name cannot be left blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(50, ErrorMessage = "Middle name cannot be more than 50 characters long.")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "The last name cannot be left blank.")]
        [StringLength(100, ErrorMessage = "Lasst name cannot be more than 100 characters long.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "OHIP Number is required.")]
        [StringLength(10)]//DS Note: we only include this to limit the size of the database field to 10
        [RegularExpression("^\\d{10}$", ErrorMessage = "The OHIP number must be exactly 10 numeric digits.")]
        public string OHIP { get; set; }

        [Required(ErrorMessage = "You must enter the Date of Birth.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DOB { get; set; }

        [Display(Name = "Visits/Yr")]
        [Required(ErrorMessage = "You must enter the number of expected yearly visits.")]
        [Range(2, 12, ErrorMessage = "The expected number of yearly visits must be between 2 and 12.")]
        public byte ExpYrVisits { get; set; }

        [Timestamp]
        public Byte[] RowVersion { get; set; }

        [Display(Name = "Doctor")]
        [Required(ErrorMessage = "You must select the Primary Care Physician.")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Doctor as the Primary Care Physician.")]
        public int DoctorID { get; set; }
        public Doctor Doctor { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DOB > DateTime.Today.AddDays(1))
            {
                yield return new ValidationResult("Date of Birth cannot be in the future.", new[] { "DOB" });
            }
        }
    }
}
