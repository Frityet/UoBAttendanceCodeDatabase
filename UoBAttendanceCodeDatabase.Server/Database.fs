module UoBAttendanceCodeDatabase.Server.Database

open System
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Logging
open UoBAttendanceCodeDatabase.Server.Models.AttendanceCodes

type AttendanceCodeContext(logger: ILogger<AttendanceCodeContext>) =
    inherit DbContext()
    
    [<DefaultValue>]
    val mutable attendanceCodes: DbSet<AttendanceCode>
    member this.AttendanceCodes
        with get() = this.attendanceCodes
        and set v = this.attendanceCodes <- v

    override this.OnConfiguring(optionsBuilder: DbContextOptionsBuilder) =
        logger.LogInformation "Configuring in-memory database connection."
        let connectionString = Environment.GetEnvironmentVariable "DATABASE_URL"
        optionsBuilder.UseNpgsql connectionString |> ignore
        // optionsBuilder.UseInMemoryDatabase("AttendanceCodeDb") |> ignore

let initializeDatabase logger =
    use context = new AttendanceCodeContext(logger)
    context.Database.EnsureCreated() |> ignore
