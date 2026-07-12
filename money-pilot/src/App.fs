module App

open Feliz
open Elmish
open Feliz.UseElmish
open Types
open State

/// Root component — wires Elmish state to the current page and app chrome.
[<ReactComponent>]
let Root () =
    let model, dispatch = React.useElmish (init, update, [||])

    let page =
        match model.Page with
        | Dashboard -> Views.Dashboard.view model dispatch
        | Accounts -> Views.Accounts.view model dispatch
        | Transactions -> Views.Transactions.view model dispatch
        | Budget -> Views.Budget.view model dispatch
        | CashFlow -> Views.CashFlow.view model dispatch
        | Investments -> Views.Investments.view model dispatch
        | Recurring -> Views.Recurring.view model dispatch
        | Goals -> Views.Goals.view model dispatch

    Html.div [
        prop.className (if model.Theme = Dark then "app theme-dark" else "app theme-light")
        prop.custom ("data-sidebar", if model.SidebarOpen then "open" else "closed")
        prop.children [
            Views.Shared.sidebar model dispatch
            Html.div [
                prop.className "main"
                prop.children [
                    Views.Shared.topbar model dispatch
                    Html.main [ prop.className "content"; prop.children [ page ] ]
                ]
            ]
            Views.Shared.modal model dispatch
            Views.Shared.toast model
        ]
    ]

open Browser.Dom

let root = ReactDOM.createRoot (document.getElementById "app")
root.render (Root())
