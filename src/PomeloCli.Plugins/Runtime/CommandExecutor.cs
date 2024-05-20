#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PomeloCli.Plugins.Runtime;

class CommandExecutor
{
    public static CommandExecuteResult Start(string commandText, string args,
        Encoding encoding = null, bool quiet = false, string workingDir = null, bool waitForExit = true)
    {
        ProcessStartInfo startInfo = null;
        try
        {
            startInfo = GetProcessStartInfo(commandText, args, useCmdExec: false, encoding, workingDir);
        }
        catch (Win32Exception)
        {
            startInfo = GetProcessStartInfo(commandText, args, useCmdExec: true, encoding, workingDir);
        }

        using var process = Process.Start(startInfo);
        try
        {
            var lines = Start(process, quiet, encoding);
            if (waitForExit)
            {
                return new CommandExecuteResult(lines.ToArray(), process.ExitCode);
            }

            return new CommandExecuteResult(lines);
        }
        finally
        {
            if (process.HasExited == false)
            {
                try
                {
                    process.Kill();
                }
                catch
                {
                    // ignored
                }
            }
        }
    }

    private static IEnumerable<string> Start(Process p, bool quiet, Encoding encoding)
    {
        var errors = new StringBuilder();
        var locker = new object();
        p.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs errorArgs)
        {
            if (errorArgs.Data != null)
            {
                lock (locker)
                {
                    errors.AppendLine(errorArgs.Data);
                }
            }
        };

        p.BeginErrorReadLine();
        var s = p.StandardOutput.ReadLine();
        while (s != null)
        {
            if (!quiet)
            {
                Console.WriteLine(s);
            }

            yield return s;
            s = p.StandardOutput.ReadLine();
        }

        p.WaitForExit();

        if (p.ExitCode != 0 && errors.Length > 0)
        {
            throw new CommandExecuteException(errors.ToString(), p.ExitCode);
        }
    }

    private static ProcessStartInfo GetProcessStartInfo(string cmdText, string args, bool useCmdExec, Encoding encoding, string workingDir = null)
    {
        if (encoding == null)
        {
            encoding = Encoding.UTF8;
        }

        cmdText = cmdText.Trim();

        var startInfo = new ProcessStartInfo
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = encoding,
            StandardInputEncoding = encoding,
            StandardErrorEncoding = encoding,
        };


        if (useCmdExec)
        {
            startInfo.FileName = "cmd.exe";
            if (cmdText.Contains(' '))
            {
                cmdText = "\"" + cmdText + "\"";
            }

            var arguments = encoding.EncodingName == "Unicode"
                ? "/u /c " + cmdText
                : "/c " + cmdText;

            if (!string.IsNullOrEmpty(args))
            {
                arguments += " " + args;
            }

            startInfo.Arguments = arguments;
        }
        else
        {
            startInfo.FileName = cmdText;
            startInfo.Arguments = args;
            if (string.IsNullOrEmpty(args) && cmdText.Contains(' ') && !File.Exists(cmdText))
            {
                var array = cmdText.Split(" ".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
                if (File.Exists(array[0]))
                {
                    startInfo.FileName = array[0];
                    startInfo.Arguments = array[1];
                }
            }

            if (workingDir != null)
            {
                startInfo.WorkingDirectory = workingDir;
            }
        }

        return startInfo;
    }
}



class CommandExecuteResult
{
    public int ExitCode { get; }
    public IEnumerable<string> Lines { get; }

    public CommandExecuteResult(IEnumerable<string> lines)
    {
        Lines = lines;
    }

    public CommandExecuteResult(IEnumerable<string> lines, int exitCode)
        : this(lines)
    {
        ExitCode = exitCode;
    }
}

public class CommandExecuteException : Exception
{
    public int ExitCode { get; private set; }

    public CommandExecuteException(string msg, int exitCode)
        : base(msg ?? "The process returned an exit code of " + exitCode)
    {
        ExitCode = exitCode;
    }
}