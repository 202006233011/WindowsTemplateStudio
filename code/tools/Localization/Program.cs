﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using CommandLine;
using Localization.Options;

namespace Localization
{
    public class Program
    {
        private const string Separator = "**********************************************************************";
        private const string ArgumentNewLine = "\r\n\t\t\t\t   ";

        [STAThread]
        public static void Main(string[] args)
        {
            var options = new CommandLineOptions();
            var parser = new Parser(s => s.MutuallyExclusive = true);
            string verb = null;
            object subOptions = null;

            if (args?.Any() == false || !parser.ParseArguments(args, options, (v, o) =>
            {
                verb = v;
                subOptions = o;
            }))
            {
                ShowHelp(verb, args);

                // TO-DO: Use options.GetUsage(verb) to auto-generate help
                // Console.WriteLine(options.GetUsage(verb));
                ExitWithError();
            }

            ProcessCommand(verb, subOptions);
        }

        private static void ShowHelp(string verb, string[] args)
        {
            if (!string.IsNullOrEmpty(verb) && verb.ToLowerInvariant() == "help" && args?.Count() > 1 && args[1] != null)
            {
                PrintHelp(args[1]);
            }
            else
            {
                PrintHelp(verb);
            }
        }

        private static void ProcessCommand(string verb, object options)
        {
            try
            {
                var tool = new LocalizationTool();

                switch (verb)
                {
                    case "ext":
                        tool.ExtractLocalizableItems(options as ExtractOptions);
                        break;
                    case "gen":
                        tool.GenerateProjectTemplatesAndCommandsHandler(options as GenerationOptions);
                        break;
                    case "verify":
                        var result = tool.VerifyLocalizableItems(options as VerifyOptions);
                        if (!result)
                        {
                            ExitWithError();
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error executing command {verb}:");
                Console.Error.WriteLine(ex.ToString());
                ExitWithError();
            }
        }

        private static void PrintHelp(string verb)
        {
            if (string.IsNullOrEmpty(verb))
            {
                Console.WriteLine("For more information on a specific command, type HELP command-name");
                Console.WriteLine("EXT\tExtract localizable items for different cultures.");
                Console.WriteLine("GEN\tGenerates Project Templates for different cultures.");
                Console.WriteLine("VERIFY\tVerify if exist localizable items for different cultures.");
                Console.WriteLine("HELP\tProvides Help information for Windows Template Studio Localization Tool.");
                Console.WriteLine();
            }
            else
            {
                switch (verb.ToUpperInvariant())
                {
                    case "EXT":
                        Console.WriteLine("Extract localizable items for different cultures.");
                        Console.WriteLine();
                        Console.WriteLine("Localization ext -o \"original_WTS_folder\" -a \"actual_WTS_folder\" -d \"destinationDirectory\"");
                        Console.WriteLine();
                        Console.WriteLine($"\toriginal_WTS_folder\t - path to the folder that contains{ArgumentNewLine}old version of WTS to compare");
                        Console.WriteLine();
                        Console.WriteLine($"\tactual_WTS_folder\t - path to the folder that contains{ArgumentNewLine}actual version of WTS to compare");
                        Console.WriteLine();
                        Console.WriteLine($"\tdestinationDirectory\t - path to the folder in which will be{ArgumentNewLine}saved all extracted items.");
                        Console.WriteLine();
                        Console.WriteLine("Example:");
                        Console.WriteLine();
                        Console.WriteLine("\tLocalization ext -o \"C:\\Projects\\wts_old\" - a \"C:\\Projects\\wts\" - d \"C:\\MyFolder\\Extracted\"");
                        Console.WriteLine();
                        break;
                    case "GEN":
                        Console.WriteLine("Generates Project Templates for different cultures.");
                        Console.WriteLine();
                        Console.WriteLine("Localization gen -s \"sourceDirectory\" -d \"destinationDirectory\"");
                        Console.WriteLine();
                        Console.WriteLine($"\tsourceDirectory\t\t - path to the folder that contains{ArgumentNewLine}source files for Project Templates{ArgumentNewLine}(it's root project folder).");
                        Console.WriteLine();
                        Console.WriteLine($"\tdestinationDirectory\t - path to the folder in which will be{ArgumentNewLine}saved all localized Project{ArgumentNewLine}Templates (parent for CSharp.UWP.{ArgumentNewLine}2017.Solution directory).");
                        Console.WriteLine();
                        Console.WriteLine("Example:");
                        Console.WriteLine();
                        Console.WriteLine("\tLocalization gen -s \"C:\\MyFolder\\wts\" -d \"C:\\MyFolder\\Generated\\ProjectTemplates\"");
                        Console.WriteLine();
                        break;
                    case "VERIFY":
                        Console.WriteLine("Verify if exist localizable items for different cultures.");
                        Console.WriteLine();
                        Console.WriteLine("Localization verify -s \"sourceDirectory\"");
                        Console.WriteLine();
                        Console.WriteLine($"\tsourceDirectory\t\t - path to the folder that contains{ArgumentNewLine}source files for verify{ArgumentNewLine}(it's root project folder).");
                        Console.WriteLine();
                        Console.WriteLine("Example:");
                        Console.WriteLine();
                        Console.WriteLine("\tLocalization verify -s \"C:\\MyFolder\\wts\"");
                        Console.WriteLine();
                        break;
                    case "HELP":
                        Console.WriteLine("Provides Help information for Windows Template Studio Localization Tool.");
                        Console.WriteLine();
                        Console.WriteLine("Localization help [command]");
                        Console.WriteLine();
                        Console.WriteLine("\tcommand - displays help information on that command.");
                        Console.WriteLine();
                        break;
                    default:
                        Console.WriteLine("Command unknown.");
                        break;
                }
            }
        }

        private static void ExitWithError()
        {
            Environment.Exit(Parser.DefaultExitCodeFail);
        }
    }
}
