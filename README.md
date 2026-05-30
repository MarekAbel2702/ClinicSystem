# ClinicSystem

ClinicSystem to aplikacja webowa napisana w technologii **ASP.NET Core MVC**, służąca do obsługi podstawowych procesów w przychodni lekarskiej. System umożliwia zarządzanie pacjentami, lekarzami oraz wizytami.

## Opis projektu

**ClinicSystem** to aplikacja typu MVC przeznaczona do zarządzania pracą przychodni. Aplikacja jest projektem studenckim.

Aplikacja pozwala na przechowywanie informacji o pacjentach, lekarzach, specjalizacjach oraz wizytach. System może obsługiwać różne role użytkowników, na przykład administratora, recepcjonistę, lekarza oraz pacjenta.

## Funkcjonalności

Przykładowe funkcjonalności aplikacji:
- rejestracja i logowanie użytkowników,
- zarządzanie pacjentami,
- zarządzanie lekarzami,
- zarządzanie specjalizacjami lekarskimi,
- tworzenie, edytowanie i anulowanie wizyt,
- przypisywanie lekarzy do specjalizacji,
- wyszukiwanie pacjentów i lekarzy,
- panel administratora,
- walidacja formularzy,
- obsługa ról użytkowników,
- komunikaty sukcesu i błędów dla użytkownika.

### 1. Rejestracja i logowanie użytkowników
<img width="738" height="918" alt="obraz" src="https://github.com/user-attachments/assets/4602a9ce-a9a2-42f1-b9c7-16dafdd80e6d" />

### 2. Zarządzanie pacjentami
<img width="2065" height="917" alt="obraz" src="https://github.com/user-attachments/assets/bb609153-dd3a-489f-bb9b-3a13637369a5" />

### 3. Zarządzanie lekarzami
<img width="2060" height="1041" alt="obraz" src="https://github.com/user-attachments/assets/609dc5b5-3cd9-4baf-9d65-3aaaf9e90156" />

### 4. Zarządzanie specjalizacjami lekarskimi
<img width="2020" height="1257" alt="obraz" src="https://github.com/user-attachments/assets/e1d88287-5120-4674-ac33-b720748a4044" />

### 5. Tworzenie, edytowanie i anulowanie wizyt
<img width="2020" height="1209" alt="obraz" src="https://github.com/user-attachments/assets/bc23525c-279c-4cb6-9636-4e39747d0f96" />

### 6. Przypisywanie lekarzy do specjalizacji
<img width="2057" height="676" alt="obraz" src="https://github.com/user-attachments/assets/4068833e-f132-45ea-9cc1-6985dad9f09d" />

### 7. Wyszukiwanie pacjentów i lekarzy
<img width="2071" height="678" alt="obraz" src="https://github.com/user-attachments/assets/7e08c6e8-ac5c-40a2-87a1-4de659bc5c40" />

### 8. Panel administratora
<img width="2076" height="727" alt="obraz" src="https://github.com/user-attachments/assets/20a384fe-923d-43b6-8745-0444ac56f410" />

### 9. Walidacja formularzy
<img width="2021" height="702" alt="obraz" src="https://github.com/user-attachments/assets/417474ba-b870-41a4-80f6-2ddfb7885b1c" />

### 10. Obsługa ról użytkowników
<img width="1250" height="307" alt="obraz" src="https://github.com/user-attachments/assets/0f856ff2-3f44-4bca-bd13-383f9658dd81" />

### 11. Komunikaty sukcesu i błędów dla użytkownika
<img width="2081" height="458" alt="obraz" src="https://github.com/user-attachments/assets/d3c0ef63-969a-4dfa-b863-a54b49fbaea3" />

### 12. Dashboard aplikacji webowej
<img width="2068" height="1372" alt="obraz" src="https://github.com/user-attachments/assets/a7ed5cdf-84f3-4258-b405-2273ce6cf171" />

## Technologie
- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Razor Views
- HTML5
- CSS3
- Bootstrap

## Wymagania

Do uruchomienia projektu wymagane są:

- .NET SDK, na przykład .NET 8 lub nowszy
- SQL Server lub SQL Server Express
- Visual Studio 2022 / Visual Studio Code / Rider
- Git
- przeglądarka internetowa

## Jak uruchomić projekt lokalnie?

Aby uruchomić aplikację **ClinicSystem** na swoim komputerze, wykonaj poniższe kroki.

### 1. Sklonuj repozytorium

```bash
git clone https://github.com/twoj-login/ClinicSystem.git
```

### 2. Przejdź do katalogu projektu

```bash
cd ClinicSystem
```

### 3. Przywróć zależności

```bash
dotnet restore
```

### 4. Skonfiguruj bazę danych

W pliku `appsettings.json` ustaw poprawny connection string do swojej lokalnej bazy danych.

Przykład dla lokalnego SQL Servera:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ClinicSystemDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Przykład dla SQL Server Express:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ClinicSystemDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 5. Utwórz lub zaktualizuj bazę danych

Jeżeli projekt korzysta z Entity Framework Core, uruchom migracje:

```bash
dotnet ef database update
```

Jeżeli polecenie `dotnet ef` nie działa, zainstaluj narzędzie Entity Framework Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

Następnie ponownie uruchom:

```bash
dotnet ef database update
```

### 6. Uruchom aplikację

```bash
dotnet run
```

### 7. Otwórz aplikację w przeglądarce

Po uruchomieniu aplikacji w konsoli pojawi się adres, pod którym jest dostępna, na przykład:

```text
https://localhost:5001
```

Otwórz ten adres w przeglądarce.

### Alternatywnie: uruchomienie w Visual Studio

Projekt można uruchomić również bezpośrednio z Visual Studio:

1. Otwórz plik `.sln`.
2. Ustaw projekt `ClinicSystem` jako projekt startowy.
3. Sprawdź connection string w pliku `appsettings.json`.
4. Wykonaj migracje bazy danych.
5. Kliknij **Start** albo naciśnij `F5`.
