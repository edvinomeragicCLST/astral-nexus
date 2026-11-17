# Mapiranje novih zahtjeva - Astral Nexus Mobile

| Zahtjev | Implementacija |
|---|---|
| Mobile Android/iOS | `AstralNexus.Mobile` cilja `net10.0-android` i `net10.0-ios` |
| .NET MAUI / .NET 10 | `UseMaui=true`, .NET 10 target framework |
| Bosanski UI | svi glavni ekrani i poruke su na bosanskom jeziku |
| XAML | 12 XAML stranica + `App.xaml` |
| C# | code-behind, servisi, SQLite data layer, validacija pravila |
| SQLite | `sqlite-net-pcl`, lokalna baza za korisnike, kolekcije, špilove, pakete i mečeve |
| REST | opcionalna sinhronizacija kataloga preko postojećeg ASP.NET Core `GET /api/cards` |
| Nivo 1 | app icon, splash, Welcome, CarouselView, About |
| Nivo 2 | registracija, prijava, odjava i PBKDF2 hash lozinke |
| Nivo 3 | korisnički profil |
| Nivo 4 | kolekcija, špilovi, coin stanje i historija aktivnosti |
| Nivo 5 | svijetla/tamna/sistemska tema |
| Nivo 6 | početni dashboard sa statistikom i navigacijom |
| Nivo 7 | kolekcija/filter, CRUD špilova, pack opening, simulator meča, historija/statistika, REST sync |

Za tim od jednog člana projekat ima više od minimalne 4 funkcionalnosti nivoa 7 i koristi i SQLite i REST.

Implementation and validation status: this academic mapping is not a test report. iOS is declared but untested. Catalog synchronization updates matching existing cards only; see the [README](../README.md) for limits and setup. No fresh Android/iOS runtime checks were completed during publication preparation.
