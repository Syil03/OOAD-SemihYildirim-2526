# CLAUDE.md – Agent Instruction File
# Project: DokterspraktijkDB – OOAD C# WPF Applicatie

## Projectoverzicht
Dit is een C# WPF-applicatie (.NET 10) voor een dokterspraktijk, bestaande uit drie projecten in één solution:
- **WPFDokter** – applicatie voor artsen
- **WPFPatient** – applicatie voor patiënten
- **DokterspraktijkLib** – class library (alle databaselogica)

De databank heet `DokterspraktijkDB` en draait op SQL Server Express (LocalDB of SQLEXPRESS).

---

## Databasestructuur

### Tabel: Afspraak
| Kolom | Type |
|-------|------|
| id | int IDENTITY PK |
| moment | datetime |
| klacht | text |
| patient_id | int (FK → Patient) |
| dokter_id | int (FK → Dokter) |

### Tabel: Dokter
| Kolom | Type |
|-------|------|
| id | int IDENTITY PK |
| voornaam | nvarchar(50) |
| achternaam | nvarchar(50) |
| gsm | nchar(10) |
| email | nvarchar(100) |
| paswoord | nvarchar(100) – SHA256 hash |
| profielfotodata | image |
| rizivnummer | int |
| isgeconventioneerd | tinyint |

### Tabel: Patient
| Kolom | Type |
|-------|------|
| id | int IDENTITY PK |
| voornaam | nvarchar(50) |
| achternaam | nvarchar(50) |
| geslacht | int |
| gsm | nchar(10) |
| email | nvarchar(100) |
| paswoord | nvarchar(100) – SHA256 hash |
| geboortedatum | datetime |
| profielfotodata | image |
| notificaties | int (enum: 0=Geen, 1=Mail, 2=Sms, 3=Beide) |

---

## Architectuurregels (VERPLICHT volgen)

### Class Library
- Alle SQL-queries zitten **uitsluitend** in de class library. Geen SQL in WPF-projecten.
- CRUD-methodes zitten **in de klassen zelf** (geen aparte DataLayer/DataContext/Repository).
- Klassen `Patient` en `Dokter` erven van een superklasse `Persoon` of `Gebruiker`.
- Gebruik een `enum Notificaties` met waarden: `Geen`, `Mail`, `Sms`, `Beide`.

### WPF Applicaties
- Gebruik **Frame + Page** navigatie (geen TabControl, geen UserControls).
- Patientenoverzicht = **dynamisch Grid** (zie SlnDemoItemsGrid patroon).
- Foutmeldingen tonen in een **TextBlock**, nooit in een MessageBox.
- Exception handling via **try-catch** in de code-behind (niet in de library).
- Rijkelijk voorzien van **commentaar** in de code.

---

## VERBODEN technieken (nooit gebruiken!)
De docent geeft 0 punten op onderdelen waar deze technieken gebruikt worden:

- `var` keyword
- `dynamic`
- `async` / `await`
- LINQ
- DataBinding (geen `{Binding ...}` in XAML)
- `DataGrid`, `GridView`, `ListView`
- Tuples
- Case guards (`when` in switch)
- `out` parameters
- `struct`
- Type switches
- User Controls
- `Invoke` / `Dispatcher.Invoke`
- Expando objecten

---

## Toegestane technieken
- Gewone C# klassen, properties, constructors, methodes
- `SqlConnection`, `SqlCommand`, `SqlDataReader` (ADO.NET)
- WPF controls: `TextBox`, `PasswordBox`, `Button`, `Label`, `TextBlock`, `Image`, `Calendar`, `ComboBox`, `ListBox`, `Grid`, `StackPanel`, `WrapPanel`, `Frame`, `Page`
- SHA256 hashing via `System.Security.Cryptography`
- `System.IO` voor afbeeldingen lezen/schrijven
- Gewone `if/else`, `for`, `foreach`, `while`, `switch`

---

## Wat je mag en niet mag aanpassen

### Mag aanpassen
- Alle .cs en .xaml bestanden in WPFDokter en WPFPatient
- Alle bestanden in de class library
- NuGet packages toevoegen indien nodig

### Mag NIET aanpassen
- De databasestructuur (geen schema-wijzigingen)
- De SQL-verbindingsstring (gebruik bestaande configuratie)

---

## Code conventies
- Nederlandstalige variabele- en methodenamen (consistent met de database)
- Commentaar in het **Nederlands**
- Elke methode voorzien van een korte samenvatting in commentaar
- Foutmeldingen weergeven in een `TextBlock` genaamd `txtFout` of `lblFout`
- Profielfoto's worden opgeslagen als `byte[]` in de database (kolom `profielfotodata`)

---

## Werkwijze voor de agent
- Werk **feature per feature**, niet alles tegelijk
- Vraag bevestiging voor je grote blokken code aanpast
- Controleer altijd of gegenereerde code geen verboden technieken bevat
- Gebruik **ask mode** voor vragen, **agent mode** voor wijzigingen doorvoeren
