module UoBAttendanceCodeDatabase.Common.Models.AttendanceCodes

open System

type AC =
    {
        id: int
        code: int
        dateAdded: int
        subject: int
    }

// type AttendanceCode() =
//     new(id: int, code: int, dateAdded: DateTime, subject: string) as this =
//         AttendanceCode()
//         then
//             this.Id <- id
//             this.Code <- code
//             this.DateAdded <- dateAdded
//             this.Subject <- subject

//     member val Id = 0 with get, set 
//     member val Code = 0 with get, set
//     member val DateAdded = DateTime.MinValue with get, set
//     member val Subject = "" with get, set

type AttendanceCode =
    {
        mutable Id: int
        mutable Code: int
        mutable DateAdded: DateTime
        mutable Subject: string
    }

type AttendanceCodes =
    {
        Codes: AttendanceCode list
    }
