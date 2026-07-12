module Format

open System

/// Formatting helpers shared across every Money Pilot view.

/// "$1,234.56"
let currency (value: float) : string =
    let sign = if value < 0.0 then "-" else ""
    let v = abs value
    let whole = Math.Floor(v)
    let cents = Math.Round((v - whole) * 100.0) |> int
    // Group the whole part with thousands separators.
    let digits = (string (int64 whole)).ToCharArray() |> Array.rev
    let grouped =
        digits
        |> Array.mapi (fun i c -> if i > 0 && i % 3 = 0 then sprintf "%c," c else string c)
        |> Array.rev
        |> String.concat ""
    sprintf "%s$%s.%02d" sign grouped cents

/// "$1,235" — no decimals, rounded.
let currency0 (value: float) : string =
    let sign = if value < 0.0 then "-" else ""
    let v = abs value |> Math.Round |> int64
    let digits = (string v).ToCharArray() |> Array.rev
    let grouped =
        digits
        |> Array.mapi (fun i c -> if i > 0 && i % 3 = 0 then sprintf "%c," c else string c)
        |> Array.rev
        |> String.concat ""
    sprintf "%s$%s" sign grouped

/// Compact currency for large numbers: "$1.2M", "$12.4k".
let currencyCompact (value: float) : string =
    let sign = if value < 0.0 then "-" else ""
    let v = abs value
    if v >= 1_000_000.0 then sprintf "%s$%.1fM" sign (v / 1_000_000.0)
    elif v >= 1_000.0 then sprintf "%s$%.1fk" sign (v / 1_000.0)
    else sprintf "%s$%.0f" sign v

/// "+3.4%" / "-1.2%"
let percentSigned (value: float) : string =
    let s = if value >= 0.0 then "+" else ""
    sprintf "%s%.1f%%" s value

let percent (value: float) : string = sprintf "%.0f%%" value

let private months =
    [| "Jan"; "Feb"; "Mar"; "Apr"; "May"; "Jun"
       "Jul"; "Aug"; "Sep"; "Oct"; "Nov"; "Dec" |]

/// "Jul 12" from an ISO yyyy-MM-dd string (no timezone maths, purely lexical).
let shortDate (iso: string) : string =
    match iso.Split('-') with
    | [| _; m; d |] ->
        let mi = (int m) - 1
        let day = int d
        if mi >= 0 && mi < 12 then sprintf "%s %d" months.[mi] day else iso
    | _ -> iso

/// "Jul 12, 2026"
let longDate (iso: string) : string =
    match iso.Split('-') with
    | [| y; m; d |] ->
        let mi = (int m) - 1
        if mi >= 0 && mi < 12 then sprintf "%s %d, %s" months.[mi] (int d) y else iso
    | _ -> iso

/// Two-letter monogram for an account or merchant.
let initials (name: string) : string =
    let parts =
        name.Split([| ' '; '-'; '_' |], StringSplitOptions.RemoveEmptyEntries)
    match parts with
    | [||] -> "?"
    | [| single |] -> single.Substring(0, min 2 single.Length).ToUpper()
    | _ ->
        let a = parts.[0].Substring(0, 1)
        let b = parts.[1].Substring(0, 1)
        (a + b).ToUpper()
