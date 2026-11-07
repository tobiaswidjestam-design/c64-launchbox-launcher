# C64 LaunchBox Launcher

Liten Windows-launcher som gör att **LaunchBox kan starta Commodore 64-spel i VICE**
när spelen ligger som zip-arkiv med flera filer i samma mapp.

## Problemet

LaunchBox packar upp spelets zip till sin tillfälliga 7-Zip-mapp och skickar *en*
fil vidare till emulatorn — men väljer inte diskettbilden, utan sista filen i
arkivet:

```
x64sc.exe -autostart "...\7-Zip\Temp\<...>\2\VERSION.NFO"
```

VICE skriver då skräptecken i `LOAD"…",8,1` och landar i `READY.` utan att spelet
startar. Samma sak händer för alla arkiv som är uppbyggda som
`NNNxx.D64` (A–D) + `.T64` + `VERSION.NFO`.

## Vad launchern gör

1. Tar emot sökvägen som LaunchBox skickar.
2. Letar i samma mapp efter första `.d64`, annars `.t64`, `.crt`, `.prg` eller `.tap`
   (ordningen går att ändra).
3. Startar VICE med den valda filen.
4. Väntar tills VICE stängs, så att LaunchBox inte städar bort temp-mappen i förtid.
5. Skriver varje steg till `c64-launcher.log`.

Det är **ingen plugin**: LaunchBox behandlar programmet som "emulatorn" eftersom
emulatorpostens `ApplicationPath` pekar på launchern i stället för på `x64sc.exe`.
Inga zip- eller ROM-filer ändras.

## Installation

1. Kopiera mappen till exempelvis
   `C:\Users\<du>\LaunchBox\ThirdParty\C64Launcher\`.
2. Kopiera `c64-launcher.ini.example` till `c64-launcher.ini` och ange sökvägen
   till din `x64sc.exe`.
3. I LaunchBox: **Tools → Manage → Emulators**, välj din VICE-post och sätt
   **Application Path** till `...\C64Launcher\c64-launcher.exe`.
   (Samma sak kan göras genom att redigera `Data\Emulators.xml`.)
4. Behåll `AutoExtract` påslaget för plattformen — launchern behöver den
   uppackade mappen.

Ta backup på `Emulators.xml` innan du ändrar den. Vill du backa allt räcker det
att peka `ApplicationPath` tillbaka mot `x64sc.exe`.

## Inställningsfil

| Nyckel | Exempel | Betydelse |
|---|---|---|
| `Emulator` | `J:\...\GTK3VICE-3.10-win64\bin\x64sc.exe` | Sökväg till VICE |
| `Arguments` | `-autostart-warp -autostart-handle-tde -autostart "{rom}"` | Argument; `{rom}` ersätts med vald fil |
| `PreferExtensions` | `d64,t64,crt,prg,tap` | Prioritetsordning för filändelser |

## Bygga själv

Kräver bara .NET Framework 4 som ingår i Windows:

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -target:winexe -r:System.Core.dll c64-launcher.cs
```

`-target:winexe` gör att det inte blir något konsolfönster när LaunchBox startar
programmet.

## Felsökning

Kör launchern direkt i en kommandoprompt för att se vad den gör:

```
c64-launcher.exe "C:\nagon\mapp\VERSION.NFO"
```

Resultatet hamnar i `c64-launcher.log` bredvid exe-filen.

## Testat med

- LaunchBox (2026-build) på Windows 11
- VICE 3.10 GTK3 (`x64sc.exe`), Commodore 64 (C64SC)

## English summary

A small Windows launcher (C#, .NET Framework 4) that lets LaunchBox start
Commodore 64 games in VICE when each game is a zip containing several files.
LaunchBox normally hands the last file of the archive (`VERSION.NFO`) to the
emulator, which makes VICE print garbage and drop to `READY.`. The launcher
picks the first `.d64` (falling back to `.t64`, `.crt`, `.prg`, `.tap`), starts
VICE with it and waits until VICE exits. No LaunchBox plugin, no ROMs modified.

## Licens

MIT – se [LICENSE](LICENSE).

