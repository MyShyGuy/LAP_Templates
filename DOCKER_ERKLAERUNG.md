# Docker – Einführung & Projektdokumentation

---

## Schnellstart – wichtigste Befehle

| Zweck | Befehl |
|---|---|
| Projekt bauen und starten | `docker compose up --build -d` |
| Status der Container prüfen | `docker compose ps` |
| Live-Logs der Web-App ansehen | `docker compose logs -f web` |
| Alle Services stoppen und entfernen | `docker compose down` |
| DB und Volumes komplett zurücksetzen | `docker compose down -v` |
| Nur den Web-Service neu bauen | `docker compose up --build -d web` |
| Tabellen in SQL Server prüfen | `docker exec mssql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "<PASSWORT>" -Q "SELECT TABLE_SCHEMA, TABLE_NAME FROM MyDb.INFORMATION_SCHEMA.TABLES;" -C` |

> **Hinweis:** Wenn nach einem Neuaufbau Login- oder Register-Probleme mit Antiforgery auftreten, Browser-Cookies für `localhost:5000` löschen oder ein Inkognito-Fenster verwenden.

## Inhaltsverzeichnis

1. [Was ist Docker?](#1-was-ist-docker)
2. [Kernkonzepte – das musst du wissen](#2-kernkonzepte--das-musst-du-wissen)
3. [Das Dockerfile dieses Projekts](#3-das-dockerfile-dieses-projekts)
4. [Die docker-compose.yml dieses Projekts](#4-die-docker-composeyml-dieses-projekts)
5. [Die wichtigsten Docker-Befehle](#5-die-wichtigsten-docker-befehle)
6. [Typischer Workflow](#6-typischer-workflow)
7. [Mit SQL Server Management Studio verbinden](#7-mit-sql-server-management-studio-verbinden)

---

## 1. Was ist Docker?

Docker ist ein Tool, das Anwendungen in sogenannte **Container** verpackt. Ein Container enthält alles, was eine Anwendung zum Laufen braucht: Code, Laufzeitumgebung, Bibliotheken und Konfiguration.

### Container vs. Virtuelle Maschine (VM)

| | Container | VM |
|---|---|---|
| Startet in | Sekunden | Minuten |
| Größe | MB | GB |
| Teilt | Kernel des Hosts | Eigener Kernel |
| Isolation | Prozess-Ebene | Hardware-Ebene |

**Kurzversion:** Container sind leichtgewichtig und starten schnell. Sie sind keine vollständige VM, aber isoliert genug, um „es läuft auf meinem Rechner"-Probleme zu eliminieren.

### Warum Docker?

- **Einheitliche Umgebung** – Entwicklung, Test und Produktion laufen identisch.
- **Einfaches Deployment** – App einmal bauen, überall starten.
- **Isolation** – mehrere Apps laufen nebeneinander, ohne sich zu stören.
- **Reproduzierbarkeit** – neue Teammitglieder müssen nichts installieren, nur Docker.

---

## 2. Kernkonzepte – das musst du wissen

### Image
Ein **Image** ist ein unveränderlicher Bauplan für einen Container – wie eine ZIP-Datei mit allem, was die App braucht. Images werden aus einem `Dockerfile` gebaut oder von einer Registry (z. B. Docker Hub) heruntergeladen.

### Container
Ein **Container** ist eine laufende Instanz eines Images. Du kannst beliebig viele Container aus demselben Image starten.

```
Image  →  docker run  →  Container (läuft)
```

### Dockerfile
Eine Textdatei mit Schritt-für-Schritt-Anweisungen, wie ein Image gebaut wird. Jede Zeile erzeugt einen **Layer** (Schicht), der gecacht wird – das macht Rebuilds schnell.

### Docker Compose
Ein Tool, das mehrere Container gleichzeitig definiert und startet. Konfiguration erfolgt in einer `docker-compose.yml`-Datei. Perfekt für Apps mit mehreren Services (z. B. Web + Datenbank).

### Registry
Ein Speicherort für Images. Die bekannteste ist **Docker Hub** (`hub.docker.com`). Microsoft hostet seine Images auf `mcr.microsoft.com`.

### Volumes
Persistenter Speicher außerhalb des Containers. Daten in einem Volume überleben das Stoppen oder Löschen eines Containers – wichtig für Datenbanken.

### Ports
Container haben ein eigenes Netzwerk. Um von außen erreichbar zu sein, müssen Ports **gemappt** werden:
```
HOST_PORT:CONTAINER_PORT
z. B. "5000:8080" → dein Browser öffnet localhost:5000, Container hört auf 8080
```

---

## 3. Das Dockerfile dieses Projekts

```dockerfile
# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish ./BlazorTemplate/BlazorTemplate.csproj -c Release -o /app/out

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "BlazorTemplate.dll"]
```

### Erklärung Zeile für Zeile

#### Stage 1 – Build-Stage (`AS build`)

| Zeile | Bedeutung |
|---|---|
| `FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build` | Verwendet das offizielle .NET 9 SDK-Image als Basis. Das SDK wird nur zum Kompilieren gebraucht. Mit `AS build` bekommt diese Stage einen Namen. |
| `WORKDIR /src` | Setzt das Arbeitsverzeichnis im Container auf `/src`. Alle folgenden Befehle laufen dort. |
| `COPY . .` | Kopiert den gesamten Quellcode vom Host in den Container (von `.` nach `/src`). |
| `RUN dotnet publish ... -c Release -o /app/out` | Kompiliert und veröffentlicht das Blazor-Projekt im Release-Modus. Das Ergebnis landet in `/app/out`. |

#### Stage 2 – Runtime-Stage

| Zeile | Bedeutung |
|---|---|
| `FROM mcr.microsoft.com/dotnet/aspnet:9.0` | Startet ein **neues, kleineres** Image – nur die ASP.NET Runtime, kein SDK (→ kleineres finales Image). |
| `WORKDIR /app` | Arbeitsverzeichnis im Runtime-Container. |
| `COPY --from=build /app/out .` | Kopiert nur die kompilierten Ausgabedateien aus der Build-Stage. Der gesamte Quellcode landet **nicht** im finalen Image. |
| `ENTRYPOINT ["dotnet", "BlazorTemplate.dll"]` | Definiert den Startbefehl. Wird ausgeführt, wenn der Container startet. |

### Multi-Stage Build – Warum?

Das Muster `Build-Stage → Runtime-Stage` (Multi-Stage Build) hat einen wichtigen Vorteil:

- Das **finale Image** enthält nur die Runtime und die kompilierten Dateien (~300 MB).
- Ohne Multi-Stage würde das SDK (~700 MB) im finalen Image landen.
- Kein Quellcode im Produktions-Image → bessere Sicherheit.

---

## 4. Die docker-compose.yml dieses Projekts

```yaml
services:
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: mssql
    environment:
      SA_PASSWORD: "TestingDockeronWindows2022!"
      ACCEPT_EULA: "Y"
    ports:
      - "1433:1433"
    volumes:
      - mssql_data:/var/opt/mssql
    healthcheck:
      test: ["CMD-SHELL", "/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P \"$SA_PASSWORD\" -Q \"SELECT 1\" -C || exit 1"]
      interval: 10s
      timeout: 5s
      retries: 12
      start_period: 20s

  web:
    build: ./LAP_Templates
    container_name: blazor
    ports:
      - "5000:8080"
    depends_on:
      db:
        condition: service_healthy
    environment:
      ConnectionStrings__DefaultConnection: "Server=db;Database=MyDb;User Id=sa;Password=TestingDockeronWindows2022!;TrustServerCertificate=True"
    volumes:
      - dp_keys:/app/keys

volumes:
  mssql_data:
  dp_keys:
```

### Service: `db` (SQL Server)

| Schlüssel | Bedeutung |
|---|---|
| `image: mcr.microsoft.com/mssql/server:2022-latest` | Verwendet das fertige Microsoft SQL Server 2022 Image. Kein eigenes Dockerfile nötig. |
| `container_name: mssql` | Gibt dem Container den fixen Namen `mssql`. |
| `SA_PASSWORD` | Passwort für den SQL-Server-Admin-Account (`sa`). |
| `ACCEPT_EULA: "Y"` | Pflichtfeld – akzeptiert die Lizenzvereinbarung von Microsoft SQL Server. |
| `ports: "1433:1433"` | SQL Server läuft auf Port 1433 und kann von außen erreicht werden. |
| `volumes: mssql_data:/var/opt/mssql` | Persistiert die eigentlichen Datenbankdaten in einem Named Volume. |
| `healthcheck` | Prüft aktiv, ob SQL Server wirklich bereit ist. Erst dann soll die Web-App starten. |

### Service: `web` (Blazor App)

| Schlüssel | Bedeutung |
|---|---|
| `build: ./LAP_Templates` | Baut das Image anhand des Dockerfiles in `./LAP_Templates`. |
| `container_name: blazor` | Fixer Container-Name. |
| `ports: "5000:8080"` | Die App ist im Browser unter `http://localhost:5000` erreichbar. |
| `depends_on: db: condition: service_healthy` | Die App wartet, bis SQL Server nicht nur gestartet, sondern auch **gesund** ist. |
| `ConnectionStrings__DefaultConnection` | Übergibt den Connection String als Umgebungsvariable. `Server=db` funktioniert, weil `db` der Hostname im Compose-Netzwerk ist. |
| `volumes: dp_keys:/app/keys` | Persistiert ASP.NET-Core-DataProtection-Keys. Das verhindert viele Antiforgery-/Cookie-Probleme nach Neustarts. |

### Volumes-Sektion

```yaml
volumes:
  mssql_data:
  dp_keys:
```

- `mssql_data` speichert die SQL-Server-Daten dauerhaft.
- `dp_keys` speichert die ASP.NET-Core-Schlüssel für Cookies, Login und Antiforgery.

### Warum sind `healthcheck`, `dp_keys` und Migrationen wichtig?

- **Healthcheck:** `depends_on` allein regelt nur die Startreihenfolge. Mit `service_healthy` wartet die Web-App wirklich auf eine bereite DB.
- **`dp_keys`:** Ohne persistente DataProtection-Keys können alte Browser-Cookies oder Antiforgery-Tokens nach einem Rebuild nicht mehr entschlüsselt werden.
- **Migrationen:** Die App führt beim Start automatisch `Database.MigrateAsync()` aus. Das klappt aber nur, wenn vorher EF-Migrationsdateien im Projekt vorhanden sind.

### Wie kommunizieren die Container miteinander?

Docker Compose erstellt automatisch ein **internes Netzwerk** für alle definierten Services. Jeder Service ist über seinen **Service-Namen** als Hostname erreichbar.

```
web-Container  →  "Server=db"  →  db-Container (Port 1433)
```

Von außen (Host) kannst du per `localhost:5000` (Web) und `localhost:1433` (DB) zugreifen.

---

## 5. Die wichtigsten Docker-Befehle

### Images

| Befehl | Beschreibung |
|---|---|
| `docker build -t mein-image .` | Baut ein Image aus dem Dockerfile im aktuellen Verzeichnis und taggt es mit `mein-image`. |
| `docker images` | Listet alle lokal vorhandenen Images. |
| `docker pull nginx` | Lädt ein Image von Docker Hub herunter. |
| `docker rmi mein-image` | Löscht ein lokales Image. |

### Container

| Befehl | Beschreibung |
|---|---|
| `docker run -p 5000:8080 mein-image` | Startet einen Container aus einem Image und mappt Ports. |
| `docker run -d mein-image` | Startet den Container im Hintergrund (`-d` = detached). |
| `docker ps` | Zeigt alle **laufenden** Container. |
| `docker ps -a` | Zeigt **alle** Container (auch gestoppte). |
| `docker stop <container>` | Stoppt einen laufenden Container (graceful). |
| `docker rm <container>` | Löscht einen gestoppten Container. |
| `docker logs <container>` | Zeigt die Logs eines Containers. |
| `docker logs -f <container>` | Zeigt Logs live (follow). |
| `docker exec -it <container> bash` | Öffnet eine interaktive Shell im laufenden Container. |

### Docker Compose

| Befehl | Beschreibung |
|---|---|
| `docker compose up` | Startet alle Services. Baut Images falls nötig. |
| `docker compose up -d` | Startet alle Services im Hintergrund. |
| `docker compose up --build` | Baut alle Images neu und startet dann. |
| `docker compose down` | Stoppt und entfernt alle Container und Netzwerke. |
| `docker compose down -v` | Wie `down`, löscht zusätzlich alle Volumes (⚠️ Datenverlust!). |
| `docker compose logs -f` | Zeigt Live-Logs aller Services. |
| `docker compose ps` | Zeigt Status aller Compose-Services. |
| `docker compose restart web` | Startet einzelnen Service neu. |

### Volumes

| Befehl | Beschreibung |
|---|---|
| `docker volume ls` | Listet alle vorhandenen Volumes auf. |
| `docker volume inspect <volume-name>` | Zeigt Details eines Volumes, inkl. `Mountpoint` (realer Speicherort auf dem Host). |
| `docker compose config --volumes` | Zeigt die in Compose definierten Volume-Namen. |

**Wichtig bei Docker Compose:**
Compose setzt standardmaessig einen Projekt-Praefix vor den Volume-Namen.

Beispiel in diesem Projekt:
- In der Compose-Datei steht: `mssql_data`
- Tatsaechlicher Docker-Name kann sein: `dockerlearning_mssql_data`

Darum funktioniert oft nicht:
```bash
docker volume inspect mssql_data
```

Sondern stattdessen:
```bash
docker volume inspect dockerlearning_mssql_data
```

### Aufräumen

| Befehl | Beschreibung |
|---|---|
| `docker system prune` | Löscht alle ungenutzten Container, Netzwerke und Images. |
| `docker volume prune` | Löscht alle ungenutzten Volumes. |
| `docker image prune` | Löscht alle „dangling" (ungetaggte) Images. |

---

## 6. Typischer Workflow

### Erststart des Projekts

```bash
# Im Wurzelverzeichnis (wo die docker-compose.yml liegt)
docker compose up --build -d
```

- Images werden gebaut (Blazor aus Dockerfile, SQL Server wird gezogen).
- `db` wird zuerst gestartet und per Healthcheck geprüft.
- Danach startet `web`.
- Beim App-Start werden vorhandene EF-Migrationen automatisch angewendet.
- App erreichbar unter: **http://localhost:5000**
- DB erreichbar unter: **localhost:1433** (z. B. mit SSMS oder Azure Data Studio)

### Änderungen am Datenmodell

Wenn du neue Entities, Properties oder Relations ergänzt, musst du **zuerst eine Migration erzeugen**:

```bash
dotnet ef migrations add NeueMigration --project .\LAP_Templates\BLDAL --startup-project .\LAP_Templates\BlazorTemplate
```

Danach reicht wieder:

```bash
docker compose up --build -d
```

### Änderungen deployen

```bash
# Container stoppen
docker compose down

# Neu bauen und starten
docker compose up --build -d
```

### Logs anschauen

```bash
# Logs aller Services
docker compose logs -f

# Nur Logs der Web-App
docker compose logs -f web
```

### In den Container schauen (Debugging)

```bash
# Shell im laufenden Blazor-Container
docker exec -it blazor bash
```

### Datenbank zurücksetzen (⚠️ löscht alle Daten!)

```bash
docker compose down -v
docker compose up -d
```

---

## 7. Mit SQL Server Management Studio verbinden

Wenn dein `db`-Container laeuft und Port `1433:1433` gemappt ist, kannst du dich direkt mit SSMS verbinden.

### 1) Container und Port pruefen

```bash
docker compose ps
```

Der `db`-Service sollte `Up` sein und `0.0.0.0:1433->1433/tcp` anzeigen.

### 2) In SSMS folgende Daten eintragen

- Server type: `Database Engine`
- Server name: `localhost,1433`
- Authentication: `SQL Server Authentication`
- Login: `sa`
- Password: `TestingDockeronWindows2022!`

### 3) Falls Zertifikatsfehler kommt

In SSMS auf `Options` gehen und unter `Connection Properties` die Option `Trust server certificate` aktivieren.

### 4) Optional: Schnelltest in SSMS

```sql
SELECT @@VERSION;
SELECT name FROM sys.databases;
```

Wenn Ergebnisse kommen, steht die Verbindung.

---

> **Tipp:** Passwörter und Secrets niemals direkt in die `docker-compose.yml` schreiben, die in Git eingecheckt wird. Nutze stattdessen eine `.env`-Datei und trage diese in `.gitignore` ein.
