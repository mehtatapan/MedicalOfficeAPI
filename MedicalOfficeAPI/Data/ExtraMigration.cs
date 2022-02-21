using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOfficeAPI.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            //Patient Table Triggers for Concurrency
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetPatientTimestampOnUpdate
                    AFTER UPDATE ON Patients
                    BEGIN
                        UPDATE Patients
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetPatientTimestampOnInsert
                    AFTER INSERT ON Patients
                    BEGIN
                        UPDATE Patients
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

            //Doctor Table Triggers for Concurrency
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetDoctorTimestampOnUpdate
                    AFTER UPDATE ON Doctors
                    BEGIN
                        UPDATE Doctors
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetDoctorTimestampOnInsert
                    AFTER INSERT ON Doctors
                    BEGIN
                        UPDATE Doctors
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        }
    }
}
