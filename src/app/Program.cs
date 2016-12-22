namespace psobolik.xmldir
{
    using System;
    using System.IO;

    public class WrongNumberOfArgumentsException : System.Exception
    {
        public WrongNumberOfArgumentsException() { }
        public WrongNumberOfArgumentsException( string message ) : base( message ) { }
        public WrongNumberOfArgumentsException( string message, System.Exception inner ) : base( message, inner ) { }
    }

    public class InvalidSwitchException : System.Exception
    {
        public InvalidSwitchException() { }
        public InvalidSwitchException( string message ) : base( message ) { }
        public InvalidSwitchException( string message, System.Exception inner ) : base( message, inner ) { }
    }

    public class XmlDirArguments
    {
        public string Root { get; set; }
        public int Depth { get; set; }
        public bool PrettyPrint { get; set; }
        public bool ShowHelp { get; set; }

        public XmlDirArguments()
        {
            Depth = 1;
        }

        public override string ToString()
        {
            return string.Format(@"Root: ""{0}""; Depth: {1}; PrettyPrint: {2}; ShowHelp: {3}", Root, Depth, PrettyPrint, ShowHelp);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // Test2();
            try
            {
                XmlDirArguments arguments = ParseArgs(args);
                // Console.WriteLine(arguments);
                if (arguments.ShowHelp)
                {
                    ShowHelp();
                }
                else
                {
                    var app = new XmlDir(arguments.Root, arguments.Depth, arguments.PrettyPrint);
                    int result = app.Run();
                }
            }
            catch (WrongNumberOfArgumentsException ex)
            {
                System.Console.WriteLine("Wrong number of arguments: " + ex.Message);
                System.Console.WriteLine();
                ShowHelp();
            }
            catch (InvalidSwitchException ex)
            {
                System.Console.WriteLine("Invalid switch: " + ex.Message);
                System.Console.WriteLine();
                ShowHelp();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: [options] path");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("  -p|--pretty-print   Pretty print the output");
            Console.WriteLine("  -d|--depth <depth>  Descend <depth> subdirectories");
            Console.WriteLine("  -?|-h|--help        Show help information");
        }

        private static XmlDirArguments ParseArgs(string[] args)
        {
            var result = new XmlDirArguments();
            int d;
            for (int i = 0, l = args.Length; i < l; ++i)
            {
                string arg = args[i];
                if (arg.StartsWith("--"))
                {
                    switch (arg.Substring(2))
                    {
                        case "help":
                            result.ShowHelp = true;
                            break;
                        case "pretty-print":
                            result.PrettyPrint = true;
                            break;
                        case "depth":
                            if (i < args.Length - 1)
                            {
                                if (int.TryParse(args[++i], out d))
                                {
                                    result.Depth = d;
                                }
                                else
                                {
                                    throw new InvalidSwitchException(string.Format("Invalid argument: '{0} {1}'", arg, args[i]));
                                }
                            }
                            else
                            {
                                throw new WrongNumberOfArgumentsException(string.Format("Missing argument: '{0}'", arg));
                            }
                            break;
                        default:
                            throw new InvalidSwitchException(string.Format("'{0}'", arg));
                    }
                }
                else if (arg.StartsWith("-"))
                {
                    switch (arg.Substring(1))
                    {
                        case "h":
                        case "?":
                            result.ShowHelp = true;
                            break;
                        case "p":
                            result.PrettyPrint = true;
                            break;
                        case "d":
                            if (i < args.Length - 1)
                            {
                                if (int.TryParse(args[++i], out d))
                                {
                                    result.Depth = d;
                                }
                                else
                                {
                                    throw new InvalidSwitchException(string.Format("Invalid argument: '{0} {1}'", arg, args[i]));
                                }
                            }
                            else
                            {
                                throw new WrongNumberOfArgumentsException(string.Format("Missing argument: '{0}'", arg));
                            }
                            break;
                        default:
                            throw new InvalidSwitchException(string.Format("'{0}'", arg));
                    }
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(result.Root))
                    {
                        result.Root = arg;
                    }
                    else
                    {
                        throw new WrongNumberOfArgumentsException();
                    }
                }
            }
            return result;
        }

        private static void Test2()
        {
            var directoryInfo = new DirectoryInfo(".");
            var fileSystemInfos = directoryInfo.GetFileSystemInfos();
            foreach (var fileSystemInfo in fileSystemInfos)
            {
                System.Console.WriteLine("{0} {1}", fileSystemInfo.Name, fileSystemInfo.FullName);
            }
        }
        private static void Test()
        {
            var entries = Directory.EnumerateFileSystemEntries(".");
            foreach (var entry in entries)
            {
                System.Console.WriteLine(entry);
            }
        }
    }
}
