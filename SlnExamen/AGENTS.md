# AGENTS.md - WPF Helpdesk Applicatie

## Project
WPF helpdesktoepassing voor OOAD examen aan Odisee Hogeschool.

## Architectuur
- **CLHelpdesk** (class library .NET 8): domeinlogica
  - `Ticket` (abstract basisklasse): Id, Beschrijving, Indiener, Status, Datum
  - `HardwareTicket : Ticket`: eigenschap Toestel
  - `SoftwareTicket : Ticket`: eigenschap Applicatie
  - `HelpdeskData`: leest/schrijft CSV, beheert tickets
- **WpfHelpdesk** (WPF .NET 8-windows): presentatielaag
  - `MainWindow`: ListBox + formulier + knoppen

## CSV-formaat
`Id;Type;Beschrijving;Indiener;Status;Datum;ExtraInfo`

Voorbeeld: `1;Hardware;Computer start niet op;Jan Janssen;Open;2024-01-15;Dell Laptop XPS 15`

## Verplichte coderingsregels
1. Geen `var` — altijd expliciete types
2. Geen LINQ — altijd foreach-lussen
3. Geen `{Binding}` in XAML
4. Geen DataGrid/ListView/GridView — gebruik ListBox
5. Geen async/await
6. Geen out-parameters
7. Geen struct — gebruik class
8. Alle CSV-logica uitsluitend in HelpdeskData.cs
9. Geen WPF-controls in CLHelpdesk

## Build
```
dotnet build CLHelpdesk\CLHelpdesk.csproj
dotnet build WpfHelpdesk\WpfHelpdesk.csproj
```
