using MedicalOfficeAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOfficeAPI.Data
{
    public static class MOSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {

            using (var context = new MedicalOfficeContext(
                serviceProvider.GetRequiredService<DbContextOptions<MedicalOfficeContext>>()))
            {
                // Look for any Doctors.  Since we can't have patients without Doctors.
                if (!context.Doctors.Any())
                {
                    context.Doctors.AddRange(
                     new Doctor
                     {
                         FirstName = "Gregory",
                         MiddleName = "A",
                         LastName = "House"
                     },

                     new Doctor
                     {
                         FirstName = "Doogie",
                         MiddleName = "R",
                         LastName = "Houser"
                     },
                     new Doctor
                     {
                         FirstName = "Charles",
                         LastName = "Xavier"
                     }
                );
                    context.SaveChanges();
                }
                if (!context.Patients.Any())
                {
                    context.Patients.AddRange(
                    new Patient
                    {
                        FirstName = "Fred",
                        MiddleName = "Reginald",
                        LastName = "Flintstone",
                        OHIP = "1231231234",
                        DOB = DateTime.Parse("1955-09-01"),
                        ExpYrVisits = 6,
                        DoctorID = context.Doctors.FirstOrDefault(d => d.FirstName == "Gregory" && d.LastName == "House").ID
                    },
                    new Patient
                    {
                        FirstName = "Wilma",
                        MiddleName = "Jane",
                        LastName = "Flintstone",
                        OHIP = "1321321324",
                        DOB = DateTime.Parse("1964-04-23"),
                        ExpYrVisits = 2,
                        DoctorID = context.Doctors.FirstOrDefault(d => d.FirstName == "Gregory" && d.LastName == "House").ID
                    },
                    new Patient
                    {
                        FirstName = "Barney",
                        LastName = "Rubble",
                        OHIP = "3213213214",
                        DOB = DateTime.Parse("1964-02-22"),
                        ExpYrVisits = 2,
                        DoctorID = context.Doctors.FirstOrDefault(d => d.FirstName == "Doogie" && d.LastName == "Houser").ID
                    },
                    new Patient
                    {
                        FirstName = "Jane",
                        MiddleName = "Samantha",
                        LastName = "Doe",
                        OHIP = "4124124123",
                        ExpYrVisits = 2,
                        DoctorID = context.Doctors.FirstOrDefault(d => d.FirstName == "Charles" && d.LastName == "Xavier").ID
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
