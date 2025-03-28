module UoBAttendanceCodeDatabase.Server.Views.Layout

open Giraffe.ViewEngine

let layout content =
    html [] [
        head [] [
            meta [ _charset "utf-8" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1" ]
            title [] [ encodedText "UoB Attendance Code Database" ]
            link [ _rel "stylesheet"; _href "https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css"; _crossorigin "anonymous" ]
        ]
        body [] content
    ]