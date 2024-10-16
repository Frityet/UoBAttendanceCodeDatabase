module UoBAttendanceCodeDatabase.Server.Views.Layout

open Giraffe.ViewEngine

let layout (content: XmlNode list) =
    html [] [
        head [] [
            title []  [ encodedText "University of Birmingham" ]
            link [
                _rel  "stylesheet"
                _type "text/css"
                _href "/main.css"
            ]
        ]
        body [] content
    ]
