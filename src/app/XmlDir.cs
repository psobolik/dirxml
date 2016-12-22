namespace psobolik.xmldir
{
  using System.Xml;

  public class XmlDir
  {
    private string Root { get; set; }
    private int Depth { get; set; }
    private bool PrettyPrint { get; set; }

    public XmlDir() : this(root: ".", depth: 1, prettyPrint: false)
    {
    }

    public XmlDir(string root, int depth, bool prettyPrint)
    {
      Root = root;
      Depth = depth;
      PrettyPrint = prettyPrint;
    }

    public int Run()
    {
      if (string.IsNullOrWhiteSpace(Root))
      {
          Root = ".";
      }
      var fileSystemEntry = new FileSystemEntry(Root, Depth);
      var document = new XmlDocument();
      var xml = fileSystemEntry.ToXmlElement(document);
      using (var stdout = System.Console.OpenStandardOutput())
      {
        var settings = new XmlWriterSettings
        {
          CloseOutput = true,
          Indent = PrettyPrint,
        };
        using (var xmlWriter = XmlWriter.Create(stdout, settings))
        {
          xml.WriteTo(xmlWriter);
        }
      }
      // DumpFileSystemEntry(fileSystemEntry);
      return 0;
    }

    private static void DumpFileSystemEntry(FileSystemEntry directory, int level = 0)
    {
      string spaces = new string(' ', level * 2);
      System.Console.WriteLine("{0} Name: {1}", spaces, directory.FileSystemInfo.Name);
      System.Console.WriteLine("{0} Size: {1}", spaces, directory.Size);
      System.Console.WriteLine("{0} Creation Time: {1}", spaces, directory.FileSystemInfo.CreationTime);
      System.Console.WriteLine("{0} Creation Time UTC: {1}", spaces, directory.FileSystemInfo.CreationTimeUtc);
      System.Console.WriteLine("{0} Extension: {1}", spaces, directory.FileSystemInfo.Extension);
      System.Console.WriteLine("{0} Full Name: {1}", spaces, directory.FileSystemInfo.FullName);
      System.Console.WriteLine("{0} Last Access Time: {1}", spaces, directory.FileSystemInfo.LastAccessTime);
      System.Console.WriteLine("{0} Last Access Time UTC: {1}", spaces, directory.FileSystemInfo.LastAccessTimeUtc);
      System.Console.WriteLine("{0} Last Write Time: {1}", spaces, directory.FileSystemInfo.LastWriteTime);
      System.Console.WriteLine("{0} Last Write Time UTC: {1}", spaces, directory.FileSystemInfo.LastWriteTimeUtc);
      System.Console.WriteLine("{0} Attributes: {1}", spaces, directory.FileSystemInfo.Attributes);
      System.Console.WriteLine(new string('-', 80));
      if (directory.DirectoryEntries != null)
      {
        foreach (var entry in directory.DirectoryEntries)
        {
          DumpFileSystemEntry(entry, level + 1);
        }
      }
    }

  }
}