# Orbitális Személyzeti Rendszer

Egy egyszerű C# / Windows Forms asztali alkalmazás egy kitalált űrállomás, a **Helios** fedélzeten dolgozó személyzetének nyilvántartására. A projekt kicsi, de a tipikus rétegzett .NET desktop alkalmazás minden fontos elemét tartalmazza: Entity Framework Core Code-First adatelérést, SQLite adatbázist, Dependency Injectiont és LINQ-alapú lekérdezést.

## Funkciók

- **Személyzet listázása** – indításkor és a *Listázás* gombra az adatbázis összes rekordja megjelenik egy csak olvasható táblázatban, rendezve: szerepkör (ABC), majd műszak (növekvő), majd szektor (ABC).
- **Új személy felvétele** – a *Hozzáadás* gomb egy külön párbeszédablakot nyit, ahol a megadott adatok ellenőrzés után az adatbázisba kerülnek.
- **Beviteli ellenőrzés** – a hibás vagy hiányzó mezőket az űrlap `ErrorProvider`-rel jelzi, és hiba esetén nem ment.

### Az adatmodell

| Mező | Típus | Leírás |
|---|---|---|
| `ID` | int | Egyedi azonosító (automatikusan generált) |
| `Name` | string | A személy neve |
| `Role` | string | `kutató`, `technikus` vagy `egyéb` |
| `Sector` | string | `Labor`, `Irányítás`, `Műszaki` vagy `Orvosi` |
| `Specialty` | string | Szakterület (opcionális) |
| `Shift` | int | Kutatók műszakja (1–3), minden más szerepkörnél 0 |

### Az új személy felvételének szabályai

| Mező | Szabály |
|---|---|
| Név | Kötelező; a szóközökből álló név nem érvényes, a vezető és záró szóközök levágódnak |
| Szerepkör | Kötelező, legördülő listából (`kutató` / `technikus` / `egyéb`) |
| Szektor | Kutatónál kötelező, másoknál elhagyható |
| Szakterület | Opcionális |
| Műszak | Csak kutatónál számít (1–3); más szerepkörnél 0 kerül az adatbázisba |

## Technológiák

- C# és Windows Forms, **.NET 10** (Windows)
- **Entity Framework Core 10** Code-First megközelítéssel, migrációkkal
- **SQLite** adatbázis
- **Dependency Injection** (`Microsoft.Extensions.DependencyInjection`)
- **LINQ** a lekérdezéshez, a rendezéshez és az azonosító elhagyásához (`Select` projekció)
- **xUnit** tesztek (`Evaluation` projekt)

## A projekt felépítése

```text
.
├── Orbital.slnx                 # solution
├── Orbital/                     # az alkalmazás
│   ├── Context/CrewContext.cs   # EF Core DbContext
│   ├── Model/CrewMember.cs      # az adatmodell
│   ├── Migrations/              # EF Core migrációk
│   ├── View/
│   │   ├── StartUp.*            # főablak: lista + Listázás / Hozzáadás gomb
│   │   └── Add.*                # új személy felvétele (párbeszédablak)
│   ├── DB/crew.db               # SQLite adatbázis
│   └── Program.cs               # belépési pont, DI konfiguráció
└── Evaluation/                  # xUnit tesztek
```

### Felépítés röviden

- A `Program.cs` regisztrálja a szolgáltatásokat: a `CrewContext` **singleton**, a `StartUp` és az `Add` form **transient**.
- Mindkét form `IServiceProvider` paramétert kap a konstruktorában, ebből kéri el az adatbázis-contextet, a `StartUp` pedig az `Add` formot.
- A `StartUp` a `Role` → `Shift` → `Sector` rendezést LINQ-val végzi, és `Select` projekcióval csak a megjelenítendő mezőket adja a táblázatnak, így az `ID` oszlop el sem készül.
- Az `Add` a `ShowDialog()`-gal nyílik meg. Sikeres mentésnél `DialogResult.OK`-t ad vissza, ennek hatására a főablak frissíti a listát.

## Követelmények

- Windows
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Visual Studio, amely támogatja a `.slnx` formátumot és a .NET 10-et (ajánlott)

## Futtatás

1. Klónozd a repót, és nyisd meg az `Orbital.slnx` fájlt Visual Studióban.
2. Állítsd az **Orbital** projektet kezdőprojektnek (jobb klikk → *Set as Startup Project*).
3. Indítsd el az alkalmazást (`F5`).

Az adatbázis útvonala (`../../../DB/crew.db`) a program futtatási könyvtárához képest relatív, ezért az alkalmazást Visual Studióból, vagy a build kimeneti mappájából (`Orbital/bin/Debug/net10.0-windows/`) indítsd. A `dotnet run` a projekt mappájából indul, így ott az útvonal rossz helyre mutat.

### Az adatbázis újragenerálása

A repóban egy üres, kész `Orbital/DB/crew.db` van. Ha újra létre akarod hozni, töröld a fájlt, és a Visual Studio **Package Manager Console** ablakában (Tools → NuGet Package Manager), az **Orbital** projektet kiválasztva futtasd:

```powershell
Update-Database
```

Ellenőrizd, hogy a `crew.db` az `Orbital/DB/` mappába került-e. Ha máshol keletkezett, másold át oda.

Új migrációt a modell módosítása után így készíthetsz:

```powershell
Add-Migration MigracioNeve
Update-Database
```

## Tesztek

Az `Evaluation` projekt tesztjei a modellt, az adatbázis-réteget, a DI-konstruktorokat és mindkét form működését ellenőrzik. A tesztek memóriában futó SQLite adatbázist használnak, a `crew.db` fájlhoz nem nyúlnak.

Visual Studióban: *Test → Run All Tests*. Parancssorból a repó gyökeréből:

```powershell
dotnet test .\Evaluation\Evaluation.csproj
```

## Hibaelhárítás

Ha a repót zipként töltöd le, és a build azt írja, hogy egy `.resx` fájlt nem tud feldolgozni a „Mark of the Web" miatt, a repó gyökeréből oldd fel a fájlokat:

```powershell
Get-ChildItem -Path . -Recurse -File | Unblock-File
```

Gitből klónozva ez nem fordul elő.
