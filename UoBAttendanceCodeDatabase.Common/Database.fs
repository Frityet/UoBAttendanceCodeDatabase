module UoBAttendanceCodeDatabase.Common.Database

open System
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Logging
open UoBAttendanceCodeDatabase.Common

let parseDatabaseUrl (url: string) =
    let uri = Uri url
    let userInfo = uri.UserInfo.Split ':'
    let username = userInfo.[0]
    let password = userInfo.[1]
    let host = uri.Host
    let port = uri.Port
    let database = uri.AbsolutePath.TrimStart '/'

    let queryParams = uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries)
    let sslMode =
        queryParams
        |> Array.tryFind (fun x -> x.StartsWith("sslmode=", StringComparison.OrdinalIgnoreCase))
        |> Option.map (fun kv -> kv.Split( '=').[1])
        |> Option.defaultValue "Require"

    $"Host={host};Port={port};Username={username};Password={password};Database={database};SSL Mode={sslMode};Trust Server Certificate=true"


type AttendanceCodeContext(logger: ILogger<AttendanceCodeContext>) =
    inherit DbContext()
    
    [<DefaultValue>]
    val mutable attendanceCodes: DbSet<Models.AttendanceCodes.AttendanceCode>
    member this.AttendanceCodes
        with get() = this.attendanceCodes
        and set v = this.attendanceCodes <- v

    override this.OnConfiguring(optionsBuilder: DbContextOptionsBuilder) =
        match Environment.GetEnvironmentVariable "DATABASE_URL" with
        | null | "" ->
            logger.LogWarning("DATABASE_URL not set, using in-memory database.")
            optionsBuilder.UseInMemoryDatabase("AttendanceCodeDb") |> ignore
        | rawUrl ->
            try
                let connectionString = parseDatabaseUrl rawUrl
                logger.LogInformation("Using parsed PostgreSQL connection string.")
                optionsBuilder.UseNpgsql(connectionString) |> ignore
            with ex ->
                logger.LogError(ex, "Failed to configure PostgreSQL, falling back to in-memory.")
                optionsBuilder.UseInMemoryDatabase("AttendanceCodeDb") |> ignore


let initializeDatabase logger =
    use context = new AttendanceCodeContext(logger)
    context.Database.EnsureCreated() |> ignore
