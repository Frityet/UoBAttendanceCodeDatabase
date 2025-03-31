module UoBAttendanceCodeDatabase.Server.Views.Index

open UoBAttendanceCodeDatabase.Common
open Giraffe.ViewEngine



/// Helper function to create a Bootstrap form group (label + input)
let formGroup labelText inputType name id placeholder isRequired =
    div [ _class "mb-3" ] [ 
        label [ _for id; _class "form-label" ] [ encodedText labelText ]
        input [
            _type inputType
            _name name
            _id id
            _class "form-control"
            _placeholder placeholder
            _spellcheck "false"
            if isRequired then _required else _spellcheck "false"
        ]
    ]

/// Helper function to create a Bootstrap primary button
let primaryButton text =
    button [ _type "submit"; _class "btn btn-primary" ] [ encodedText text ]

/// Helper function to create a Bootstrap secondary button
let secondaryButton text =
    button [ _type "reset"; _class "btn btn-secondary" ] [ encodedText text ]

/// Helper function to create a Bootstrap danger button (e.g., for delete)
let dangerButton text href onclick =
    a [ _href href; _class "btn btn-danger"; _onclick onclick ] [ encodedText text ]

/// Helper function to create a Bootstrap info button (e.g., for edit)
let infoButton text href =
    a [ _href href; _class "btn btn-info" ] [ encodedText text ]

/// Helper function for navbar links (active state can be controlled if needed)
let navLink text href isActive =
    li [ _class "nav-item" ] [
        a [ _class (sprintf "nav-link %s" (if isActive then "active" else "")); _href href ] [ encodedText text ]
    ]



let navbar =
    nav [ _class "navbar navbar-expand-lg navbar-dark bg-primary" ] [ 
        div [ _class "container-fluid" ] [
            a [ _class "navbar-brand"; _href "/" ] [ encodedText "UoB Attendance" ]
        ]
    ]

let footer =
    footer [ _class "bg-light text-center py-3 mt-4" ] [ 
        div [ _class "container" ] [
            p [ _class "text-muted mb-0" ] [ encodedText "© 2025 Frityet. All rights reserved." ]
        ]
    ]

let inputForm () =
    form [ _method "post"; _action "/add-code"; _class "row g-3" ] [ 
        formGroup "Code:" "number" "code" "code" "Enter code" true
        formGroup "Date Added:" "datetime-local" "dateAdded" "dateAdded" "" true 
        formGroup "Subject:" "text" "subject" "subject" "Enter subject" true

        div [ _class "col-12" ] [ 
            div [ _class "d-flex justify-content-start gap-2" ] [ 
                primaryButton "Add Attendance Code"
                secondaryButton "Reset"
            ]
        ]
    ]

let tableView (codes: Models.AttendanceCodes.AttendanceCode list) =
    div [ _class "table-responsive" ] [ 
        table [ _class "table table-striped table-hover" ] [ 
            thead [ _class "table-primary" ] [ 
                tr [] [
                    th [] [ encodedText "Code" ]
                    th [] [ encodedText "Date Added" ]
                    th [] [ encodedText "Subject" ]
                    // th [ _class "text-center" ] [ encodedText "Actions" ] 
                ]
            ]
            tbody [] [
                for code in codes do
                    tr [] [
                        td [] [ encodedText (string code.Code) ]
                        td [] [ encodedText (code.DateAdded.ToString "dd/MM/yyyy") ]
                        td [] [ encodedText code.Subject ]
                        // td [ _class "text-center" ] [ 
                        //     infoButton "Edit" (sprintf "/edit/%d" code.Code)
                        //     dangerButton "Delete" (sprintf "/delete/%d" code.Code) "return confirm('Are you sure you want to delete this code?');"
                        // ]
                    ]
            ]
        ]
    ]

let index (model: Models.AttendanceCodes.AttendanceCodes) =
    [
        
        navbar
        div [ _class "container mt-4" ] [ 
            h1 [ _class "page-title text-center mb-4" ] [ encodedText "University of Birmingham Attendance Code Database" ] 
            section [ _class "content-section p-4 bg-light border rounded shadow-sm" ] [ 
                h2 [ _class "mb-3" ] [ encodedText "Attendance Codes" ]
                div [ _class "mb-3" ] [ 
                    input [ _type "text"; _id "searchInput"; _class "form-control"; _placeholder "Filter by subject..." ] 
                ]
                tableView model.Codes

                h2 [ _class "mt-4 mb-3" ] [ encodedText "Add Attendance Code" ] 
                inputForm()
            ]
        ]
        footer
        script [] [ rawText """
            
            document.getElementById('searchInput').addEventListener('keyup', function() {
                var filter = this.value.toUpperCase();
                var rows = document.querySelectorAll('table tbody tr');
                rows.forEach(function(row) {
                    var subject = row.cells[2].textContent.toUpperCase();
                    row.style.display = subject.indexOf(filter) > -1 ? '' : 'none';
                });
            });
        """ ]
    ] |> Layout.layout 
