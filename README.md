# C64 LaunchBox Launcher

A small Windows launcher that lets **LaunchBox start Commodore 64 games in VICE**
when each game is a zip archive containing several files in the same folder.

## The problem

LaunchBox extracts the game's zip into its temporary 7-Zip folder and passes *one*
file on to the emulator — but it does not pick the disk image, it picks the last
file in the archive:

```
x64sc.exe -autostart "...\7-Zip\Temp\<...>\2\VERSION.NFO"
```

VICE then prints garbage characters in `LOAD"…",8,1` and ends up at `READY.`
without starting the game. The same thing happens for every archive built as
`NNNxx.D64` (A–D) + `.T64` + `VERSION.NFO`.

## What the launcher does

1. Receives the path that LaunchBox passes to it.
2. Looks in the same folder for the first `.d64`, otherwise `.t64`, `.crt`, `.prg`
   or `.tap` (the order is configurable).
3. Starts VICE with the chosen file.
4. Waits until VICE exits, so that LaunchBox does not clean up the temp folder
   too early.
5. Writes every step to `c64-launcher.log`.

It is **not a plugin**: LaunchBox treats the program as "the emulator" because the
emulator entry's `ApplicationPath` points at the launcher instead of `x64sc.exe`.
No zip or ROM files are modified.

## Installation

1. Copy the folder to, for example,
   `C:\Users\<you>\LaunchBox\ThirdParty\C64Launcher\`.
2. Copy `c64-launcher.ini.example` to `c64-launcher.ini` and set the path to your
   `x64sc.exe`.
3. In LaunchBox: **Tools → Manage → Emulators**, select your VICE entry and set
   **Application Path** to `...\C64Launcher\c64-launcher.exe`.
   (The same change can be made by editing `Data\Emulators.xml`.)
4. Keep `AutoExtract` enabled for the platform — the launcher needs the extracted
   folder.

Back up `Emulators.xml` before you change it. To undo everything, point
`ApplicationPath` back at `x64sc.exe`.

## Settings file

| Key | Example | Meaning |
|---|---|---|
| `Emulator` | `J:\...\GTK3VICE-3.10-win64\bin\x64sc.exe` | Path to VICE |
| `Arguments` | `-autostart-warp -autostart-handle-tde -autostart "{rom}"` | Arguments; `{rom}` is replaced with the chosen file |
| `PreferExtensions` | `d64,t64,crt,prg,tap` | Priority order for file extensions |

## Building it yourself

Only .NET Framework 4 is required, and it ships with Windows:

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe -target:winexe -r:System.Core.dll c64-launcher.cs
```

`-target:winexe` means no console window appears when LaunchBox starts the program.

## Troubleshooting

Run the launcher directly from a command prompt to see what it does:

```
c64-launcher.exe "C:\some\folder\VERSION.NFO"
```

The result ends up in `c64-launcher.log` next to the exe file.

## Tested with

- LaunchBox (2026 build) on Windows 11
- VICE 3.10 GTK3 (`x64sc.exe`), Commodore 64 (C64SC)

## License

MIT – see [LICENSE](LICENSE).
