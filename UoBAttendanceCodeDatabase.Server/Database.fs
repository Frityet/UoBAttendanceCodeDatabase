module UoBAttendanceCodeDatabase.Server.Database

open Microsoft.EntityFrameworkCore
open UoBAttendanceCodeDatabase.Server.Models.AttendanceCodes

type AttendanceCodeContext() =
    inherit DbContext()
    
    [<DefaultValue>]
    val mutable attendanceCodes: DbSet<AttendanceCode>
    member this.AttendanceCodes
        with get() = this.attendanceCodes
        and set v = this.attendanceCodes <- v

    override this.OnConfiguring(optionsBuilder: DbContextOptionsBuilder) =
        optionsBuilder.UseSqlite("Data Source=attendance-codes.db") |> ignore

let initializeDatabase() =
    use context = new AttendanceCodeContext()
    context.Database.EnsureCreated() |> ignore
