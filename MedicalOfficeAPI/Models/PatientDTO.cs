using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MedicalOfficeAPI.Models
{
    [ModelMetadataType(typeof(PatientMetaData))]
    public class PatientDTO : IValidatableObject
    {
        public int ID { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string OHIP { get; set; }

        public DateTime DOB { get; set; }

        public byte ExpYrVisits { get; set; }

        public Byte[] RowVersion { get; set; }

        public int DoctorID { get; set; }
        public DoctorDTO Doctor { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DOB > DateTime.Today.AddDays(1))
            {
                yield return new ValidationResult("Date of Birth cannot be in the future.", new[] { "DOB" });
            }
        }
    }
}
