# Documentatie - WPF Helpdesk Applicatie

## Projectinformatie
- **Student**: Semih Yildirim
- **Opleiding**: OOAD, Odisee Hogeschool
- **Academiejaar**: 2025-2026

## Projectbeschrijving
Een WPF-helpdesktoepassing voor het beheren van IT-supporttickets.
Tickets worden opgeslagen in een CSV-bestand en kunnen worden aangemaakt en afgesloten.

## Klassendiagram

### CLHelpdesk (Class Library .NET 8)

```
Ticket (abstract)
├── Id : int
├── Beschrijving : string
├── Indiener : string
├── Status : string
├── Datum : DateTime
├── GeefType() : string  (abstract)
├── GeefExtraInfo() : string  (abstract)
└── ToString() : string

HardwareTicket : Ticket
└── Toestel : string

SoftwareTicket : Ticket
└── Applicatie : string

HelpdeskData
├── GeefAlleTickets() : List<Ticket>
├── VoegTicketToe(Ticket) : void
└── SluitTicketAf(int id) : void
```

### WpfHelpdesk (WPF Application .NET 8-windows)

```
MainWindow : Window
├── _helpdeskData : HelpdeskData
├── LaadTickets() : void
├── BtnToevoegen_Click(...)
├── BtnAfsluiten_Click(...)
└── CbxType_SelectionChanged(...)
```

## UI-overzicht

| Control        | Type     | Functie                              |
|----------------|----------|--------------------------------------|
| LstTickets     | ListBox  | Toont alle tickets                   |
| TxtBeschrijving| TextBox  | Invoer beschrijving nieuw ticket     |
| TxtIndiener    | TextBox  | Invoer naam indiener                 |
| CbxType        | ComboBox | Keuze Hardware / Software            |
| TxtExtraInfo   | TextBox  | Toestel (HW) of Applicatie (SW)      |
| BtnToevoegen   | Button   | Ticket aanmaken en opslaan           |
| BtnAfsluiten   | Button   | Geselecteerd ticket status "Afgesloten" |

## Dataopslag
CSV-bestand `helpdesk_tickets.csv` in de uitvoermap.
Formaat: `Id;Type;Beschrijving;Indiener;Status;Datum;ExtraInfo`

## Gebruiksaanwijzing
1. Start de applicatie — bestaande tickets verschijnen in de ListBox
2. Vul het formulier in (beschrijving, indiener, type, extra info)
3. Klik **Ticket Toevoegen** — ticket wordt opgeslagen en de lijst ververst
4. Selecteer een ticket in de ListBox
5. Klik **Geselecteerd Ticket Afsluiten** — bevestig met Ja

## Ontwerpbeslissingen
- Geen LINQ: leesbaarder voor beginners en voldoet aan examenvereisten
- Geen data binding: handmatige populatie van ListBox via `Items.Add()`
- `HelpdeskData` bevat alle CSV-logica (scheiding van lagen)
- Abstracte klasse `Ticket` met polymorfisme via `GeefType()` / `GeefExtraInfo()`
