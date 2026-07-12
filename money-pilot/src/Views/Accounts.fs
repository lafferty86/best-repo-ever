module Views.Accounts

open Feliz
open Types
open State
open Views.Shared

let private accountRow (dispatch: Msg -> unit) (a: Account) =
    Html.button [
        prop.className "acct-row"
        prop.onClick (fun _ ->
            dispatch (SetAccountFilter (Some a.Id))
            dispatch (Navigate Transactions))
        prop.children [
            Html.div [
                prop.className "acct-mark"
                prop.style [ style.custom ("background", a.Color + "22"); style.color a.Color ]
                prop.text (Format.initials a.Institution)
            ]
            Html.div [
                prop.className "acct-main"
                prop.children [
                    Html.span [ prop.className "acct-name"; prop.text a.Name ]
                    Html.span [ prop.className "acct-sub"; prop.text (sprintf "%s · %s ••%s" a.Institution a.Kind.Label a.Mask) ]
                ]
            ]
            Html.div [
                prop.className "acct-right"
                prop.children [
                    Html.span [
                        prop.className (if a.Balance >= 0.0 then "acct-bal" else "acct-bal neg")
                        prop.text (Format.currency a.Balance)
                    ]
                    trendChip a.Change
                ]
            ]
        ]
    ]

let private group (dispatch: Msg -> unit) (title: string) (accts: Account list) =
    if List.isEmpty accts then Html.none
    else
        let sum = accts |> List.sumBy (fun a -> a.Balance)
        card "" [
            cardHead title (Html.span [ prop.className "muted-sm"; prop.text (Format.currency sum) ])
            Html.div [ prop.className "acct-list"; prop.children [ for a in accts -> accountRow dispatch a ] ]
        ]

let view (model: Model) (dispatch: Msg -> unit) =
    let nw = netWorth model.Accounts
    let assets = assetsTotal model.Accounts
    let liabilities = liabilitiesTotal model.Accounts
    let ofKind ks = model.Accounts |> List.filter (fun a -> List.contains a.Kind ks)

    Html.div [
        prop.className "page"
        prop.children [
            Html.div [
                prop.className "networth-band"
                prop.children [
                    Html.div [
                        prop.className "nw-cell"
                        prop.children [
                            Html.span [ prop.className "nw-label"; prop.text "Net worth" ]
                            Html.span [ prop.className "nw-value"; prop.text (Format.currency nw) ]
                        ]
                    ]
                    Html.div [
                        prop.className "nw-cell"
                        prop.children [
                            Html.span [ prop.className "nw-label"; prop.text "Assets" ]
                            Html.span [ prop.className "nw-value pos"; prop.text (Format.currency assets) ]
                        ]
                    ]
                    Html.div [
                        prop.className "nw-cell"
                        prop.children [
                            Html.span [ prop.className "nw-label"; prop.text "Liabilities" ]
                            Html.span [ prop.className "nw-value neg"; prop.text (Format.currency liabilities) ]
                        ]
                    ]
                    Html.div [
                        prop.className "nw-bar"
                        prop.children [
                            Html.div [
                                prop.className "nw-bar-fill"
                                prop.style [ style.width (length.percent (if assets <= 0.0 then 0.0 else (assets - liabilities) / assets * 100.0)) ]
                            ]
                        ]
                    ]
                ]
            ]
            Html.div [
                prop.className "grid-2"
                prop.children [
                    Html.div [ prop.className "stack"; prop.children [
                        group dispatch "Cash" (ofKind [ Checking; Savings; Cash ])
                        group dispatch "Investments" (ofKind [ Investment ])
                    ]]
                    Html.div [ prop.className "stack"; prop.children [
                        group dispatch "Credit cards" (ofKind [ CreditCard ])
                        group dispatch "Loans" (ofKind [ Loan ])
                    ]]
                ]
            ]
        ]
    ]
