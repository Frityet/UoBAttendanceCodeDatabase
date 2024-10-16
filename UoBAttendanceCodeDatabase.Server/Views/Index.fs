module UoBAttendanceCodeDatabase.Server.Views.Index

open UoBAttendanceCodeDatabase.Server
open Giraffe.ViewEngine

let inputForm () =
    form [ _method "post"; _action "/add-code" ] [
        div [] [
            label [ _for "code" ] [ encodedText "Code: " ]
            input [ _type "number"; _name "code"; _id "code"; _required ]
        ]
        div [] [
            label [ _for "dateAdded" ] [ encodedText "Date Added: " ]
            input [ _type "datetime-local"; _name "dateAdded"; _id "dateAdded"; _required ]
        ]
        div [] [
            label [ _for "subject" ] [ encodedText "Subject: " ]
            input [ _type "text"; _name "subject"; _id "subject"; _required ]
        ]
        div [] [
            button [ _type "submit" ] [ encodedText "Add Attendance Code" ]
        ]
    ]

let index (model: Models.AttendanceCodes.AttendanceCodes) =
    [
        h1 [] [ encodedText "University of Birmingham Attendance Code Database" ]
        section [] [
            h2 [] [ encodedText "Attendance Codes" ]
            table [] [
                thead [] [
                    tr [] [
                        th [] [ encodedText "Code" ]
                        th [] [ encodedText "Date Added" ]
                        th [] [ encodedText "Subject" ]
                    ]
                ]
                tbody [] [
                    for code in model.Codes do
                        tr [] [
                            td [] [ encodedText (string code.Code) ]
                            td [] [ encodedText (code.DateAdded.ToString("dd/MM/yyyy")) ]
                            td [] [ encodedText code.Subject ]
                        ]
                ]
            ]

            h2 [] [ encodedText "Add Attendance Code" ]
            inputForm()
        ]
    ] |> Layout.layout

