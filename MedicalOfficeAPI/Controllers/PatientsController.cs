using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalOfficeAPI.Data;
using MedicalOfficeAPI.Models;

namespace MedicalOfficeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly MedicalOfficeContext _context;

        public PatientsController(MedicalOfficeContext context)
        {
            _context = context;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatients()
        {
            return await _context.Patients
                .Include(p=>p.Doctor)
                .Select(p=> new PatientDTO
                {
                    ID = p.ID,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    LastName = p.LastName,
                    OHIP = p.OHIP,
                    DOB = p.DOB,
                    ExpYrVisits = p.ExpYrVisits,
                    RowVersion = p.RowVersion,
                    DoctorID = p.DoctorID,
                    Doctor = new DoctorDTO
                    {
                        ID = p.Doctor.ID,
                        FirstName = p.Doctor.FirstName,
                        MiddleName = p.Doctor.MiddleName,
                        LastName = p.Doctor.LastName
                    }
                })
                .ToListAsync();
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDTO>> GetPatient(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Doctor)
                .Select(p => new PatientDTO
                {
                    ID = p.ID,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    LastName = p.LastName,
                    OHIP = p.OHIP,
                    DOB = p.DOB,
                    ExpYrVisits = p.ExpYrVisits,
                    RowVersion = p.RowVersion,
                    DoctorID = p.DoctorID,
                    Doctor = new DoctorDTO
                    {
                        ID = p.Doctor.ID,
                        FirstName = p.Doctor.FirstName,
                        MiddleName = p.Doctor.MiddleName,
                        LastName = p.Doctor.LastName
                    }
                })
                .FirstOrDefaultAsync(p => p.ID == id);

            if (patient == null)
            {
                return NotFound(new { message = "Error: Patient record not found." });
            }

            return patient;
        }

        // GET: api/Patients/ByDoctor
        [HttpGet("ByDoctor/{id}")]
        public async Task<ActionResult<IEnumerable<PatientDTO>>> GetPatientsByDoctor(int id)
        {
            var patients =  await _context.Patients
                .Include(e => e.Doctor)
                .Where(e => e.DoctorID == id)
                .Select(p => new PatientDTO
                {
                    ID = p.ID,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    LastName = p.LastName,
                    OHIP = p.OHIP,
                    DOB = p.DOB,
                    ExpYrVisits = p.ExpYrVisits,
                    RowVersion = p.RowVersion,
                    DoctorID = p.DoctorID,
                    Doctor = new DoctorDTO
                    {
                        ID = p.Doctor.ID,
                        FirstName = p.Doctor.FirstName,
                        MiddleName = p.Doctor.MiddleName,
                        LastName = p.Doctor.LastName
                    }
                }).ToListAsync();

            if(patients.Count()>0)
            {
                return patients;
            }
            else
            {
                return NotFound(new { message = "Error: No Patient records or that Doctor." });
            }
        }

        // PUT: api/Patients/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, PatientDTO patientDTO)
        {
            if (id != patientDTO.ID)
            {
                return BadRequest(new { message = "Error: Incorrect ID for Patient." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Get the record you want to update
            var patientToUpdate = await _context.Patients.FindAsync(id);

            //Check that you got it
            if (patientToUpdate == null)
            {
                return NotFound(new { message = "Error: Patient record not found." });
            }

            //Wow, we have a chance to check for concurrency even before bothering
            //the database!  Of course, it will get checked again in the database just in case
            //it changes after we pulled the record.  
            //Note using SequenceEqual becuase it is an array after all.
            if (!patientToUpdate.RowVersion.SequenceEqual(patientDTO.RowVersion))
            {
                return Conflict(new { message = "Concurrency Error: Patient has been changed by another user.  Try editing the record again." });
            }

            //patientToUpdate = patientDTO; - Fix with MappingGenerator

            //Update the properties of the entity object from the DTO object
            patientToUpdate.ID = patientDTO.ID;
            patientToUpdate.FirstName = patientDTO.FirstName;
            patientToUpdate.MiddleName = patientDTO.MiddleName;
            patientToUpdate.LastName = patientDTO.LastName;
            patientToUpdate.OHIP = patientDTO.OHIP;
            patientToUpdate.DOB = patientDTO.DOB;
            patientToUpdate.ExpYrVisits = patientDTO.ExpYrVisits;
            patientToUpdate.RowVersion = patientDTO.RowVersion;
            patientToUpdate.DoctorID = patientDTO.DoctorID;

            //Put the original RowVersion value in the OriginalValues collection for the entity
            _context.Entry(patientToUpdate).Property("RowVersion").OriginalValue = patientDTO.RowVersion;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    return Conflict(new { message = "Concurrency Error: Patient has been Removed." });
                }
                else
                {
                    return Conflict(new { message = "Concurrency Error: Patient has been updated by another user.  Back out and try editing the record again." });
                }
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE"))
                {
                    return BadRequest(new { message = "Unable to save: Duplicate OHIP number." });
                }
                else
                {
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
                }
            }
        }

        // POST: api/Patients
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(PatientDTO patientDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Patient patient = new Patient { };    //Right click in { } and Fix with MappingGenerator

            Patient patient = new Patient
            {
                ID = patientDTO.ID,
                FirstName = patientDTO.FirstName,
                MiddleName = patientDTO.MiddleName,
                LastName = patientDTO.LastName,
                OHIP = patientDTO.OHIP,
                DOB = patientDTO.DOB,
                ExpYrVisits = patientDTO.ExpYrVisits,
                RowVersion = patientDTO.RowVersion,
                DoctorID = patientDTO.DoctorID
            };

            try
            {
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                //Assign Database Generated values back into the DTO
                patientDTO.ID = patient.ID;
                patientDTO.RowVersion = patient.RowVersion;

                return CreatedAtAction(nameof(GetPatient), new { id = patient.ID }, patientDTO);
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE"))
                {
                    return BadRequest(new { message = "Unable to save: Duplicate OHIP number." });
                }
                else
                {
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
                }
            }
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Patient>> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound(new { message = "Delete Error: Patient has already been removed." });
            }
            try
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Delete Error: Unable to delete Patient." });
            }
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.ID == id);
        }
    }
}
