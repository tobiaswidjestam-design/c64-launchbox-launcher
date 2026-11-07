using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// C64-launcher: tar emot speltiteln/sokvagen som LaunchBox skickar, valjer ratt
// diskettbild (d64/t64/crt/prg/tap) i samma mapp och startar VICE med den.
// Vantar tills VICE stangs sa att LaunchBox rensar sin temp-mapp forst darefter.
class C64Launcher
{
    static string exeDir;
    static string logFile;

    static int Main(string[] args)
    {
        exeDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        logFile = Path.Combine(exeDir, "c64-launcher.log");
        try
        {
            Log("Argument: " + string.Join(" ", args));

            string romPath = ResolveRomPath(args);
            if (romPath == null)
            {
                Log("FEL: hittar ingen sokvag i argumenten");
                return 1;
            }

            string target = PickImage(romPath);
            Log("Inkommande: " + romPath);
            Log("Vald fil:   " + target);

            string emu = ReadSetting("Emulator", "");
            string argsTemplate = ReadSetting("Arguments", "-autostart \"{rom}\"");
            if (emu.Length == 0 || !File.Exists(emu))
            {
                Log("FEL: emulator-exe saknas (" + emu + ")");
                return 1;
            }

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = emu;
            psi.Arguments = argsTemplate.Replace("{rom}", target);
            psi.WorkingDirectory = Path.GetDirectoryName(emu);
            psi.UseShellExecute = false;
            Log("Startar: " + emu + " " + psi.Arguments);

            using (Process p = Process.Start(psi))
            {
                p.WaitForExit();
                Log("VICE avslutades, exitkod " + p.ExitCode);
                return p.ExitCode;
            }
        }
        catch (Exception e)
        {
            Log("FEL: " + e);
            return 1;
        }
    }

    static string ResolveRomPath(string[] args)
    {
        for (int i = args.Length - 1; i >= 0; i--)
        {
            string a = args[i].Trim('"');
            if (a.Length == 0 || a.StartsWith("-")) continue;
            if (a.IndexOfAny(new char[] { '\\', '/', ':' }) >= 0 || File.Exists(a)) return a;
        }
        return null;
    }

    static string PickImage(string romPath)
    {
        string dir = Path.GetDirectoryName(romPath);
        if (dir != null && Directory.Exists(dir))
        {
            string[] prefs = ReadSetting("PreferExtensions", "d64,t64,crt,prg,tap").Split(',');
            foreach (string raw in prefs)
            {
                string ext = raw.Trim().TrimStart('.');
                if (ext.Length == 0) continue;
                string[] files = Directory.GetFiles(dir)
                    .Where(f => string.Equals(Path.GetExtension(f).TrimStart('.'), ext, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                if (files.Length > 0)
                {
                    Array.Sort(files, StringComparer.OrdinalIgnoreCase);
                    return files[0];
                }
            }
        }
        return romPath;
    }

    static string ReadSetting(string key, string def)
    {
        string ini = Path.Combine(exeDir, "c64-launcher.ini");
        if (!File.Exists(ini)) return def;
        foreach (string line in File.ReadAllLines(ini))
        {
            string l = line.Trim();
            if (l.Length == 0 || l.StartsWith("#") || l.StartsWith(";")) continue;
            int i = l.IndexOf('=');
            if (i <= 0) continue;
            if (string.Equals(l.Substring(0, i).Trim(), key, StringComparison.OrdinalIgnoreCase))
                return l.Substring(i + 1).Trim();
        }
        return def;
    }

    static void Log(string msg)
    {
        try
        {
            File.AppendAllText(logFile,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " - " + msg + Environment.NewLine,
                new UTF8Encoding(false));
        }
        catch { }
    }
}
