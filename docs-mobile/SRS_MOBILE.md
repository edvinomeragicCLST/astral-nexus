# SRS - Astral Nexus Mobile

## Svrha
Mobilna aplikacija omogućava korisniku lokalno upravljanje računom, kolekcijom karata, špilovima, paketima, simulacijom meča i historijom.

## Funkcionalni zahtjevi
- FR-01: korisnik može kreirati lokalni račun.
- FR-02: korisnik se može prijaviti i odjaviti.
- FR-03: korisnik može pregledati i izmijeniti profil.
- FR-04: korisnik može promijeniti temu aplikacije.
- FR-05: korisnik može pretraživati i filtrirati kolekciju.
- FR-06: korisnik može kreirati, učitati, izmijeniti i obrisati špil.
- FR-07: aplikacija validira pravila špila prije spremanja.
- FR-08: korisnik može otvoriti paket karata uz coin ekonomiju i detekciju duplikata.
- FR-09: korisnik može pokrenuti simulator meča sa validnim špilom, izvlačiti/odbacivati karte, mijenjati HP, baciti kocku i novčić te završiti meč.
- FR-10: aplikacija trajno čuva rezultate i historiju u SQLite bazi.
- FR-11: dashboard prikazuje coine, broj karata, špilova i W/L statistiku.
- FR-12: aplikacija može sinhronizovati centralni katalog karata preko REST API-ja.

## Nefunkcionalni zahtjevi
- NF-01: UI je na bosanskom jeziku.
- NF-02: aplikacija radi bez mreže za sve SQLite funkcije.
- NF-03: osjetljive lozinke ne čuvaju se kao čist tekst; koristi se PBKDF2 + salt.
- NF-04: glavni ekrani su prilagodljivi standardnim Android/iOS dimenzijama.
- NF-05: trajni podaci ostaju nakon zatvaranja aplikacije.
