namespace psobolik.xmldir
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Xml;

  public class FileSystemEntry
  {
    public System.IO.FileSystemInfo FileSystemInfo { get; set; }
    public long Size { get; set; }
    public FileSystemEntry[] DirectoryEntries { get; set; }
    public bool IsDirectory { get { return (FileSystemInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory; } }
    public string ElementName { get { return IsDirectory ? "Directory" : "File"; } }
    public FileSystemEntry() { }
    public FileSystemEntry(string path, int level = 0)
    {
      var FileSystemEntry = ReadFileSystemEntry(path, level);
      FileSystemInfo = FileSystemEntry.FileSystemInfo;
      Size = FileSystemEntry.Size;
      DirectoryEntries = FileSystemEntry.DirectoryEntries;
    }

    private static FileSystemEntry ReadFileSystemEntry(string path, int level = 0)
    {
      FileSystemEntry result = null;

      if (Directory.Exists(path))
      {
        var directoryInfo = new DirectoryInfo(path);
        result = new FileSystemEntry
        {
          FileSystemInfo = directoryInfo,
          Size = 0,
        };

        if (level > 0)
        {
          try
          {
            var fileSystemInfos = directoryInfo.GetFileSystemInfos();
            var list = new List<FileSystemEntry>();
            foreach (var fileSystemInfo in fileSystemInfos)
            {
              var fileSystemEntry = new FileSystemEntry(fileSystemInfo.FullName, level - 1);
              list.Add(fileSystemEntry);
            }
            result.DirectoryEntries = list.ToArray();
          }
          catch (UnauthorizedAccessException)
          {
            // Eat the exception
          }
        }
      }
      else if (File.Exists(path))
      {
        var fileInfo = new FileInfo(path);
        result = new FileSystemEntry
        {
          FileSystemInfo = fileInfo,
          Size = fileInfo.Length,
        };
      }
      else
      {
        throw new ArgumentException(string.Format(@"Invalid path: ""{0}""", path));
      }
      return result;
    }

    public XmlElement ToXmlElement(XmlDocument document)
    {
      var result = MakeXmlElement(document);
      if (DirectoryEntries != null)
      {
        foreach (var child in DirectoryEntries)
        {
          result.AppendChild(child.ToXmlElement(document));
        }
      }
      return result;
    }

    public XmlElement MakeXmlElement(XmlDocument document)
    {
      var result = document.CreateElement(ElementName);
      result.SetAttribute("Name", FileSystemInfo.Name);
      if (!IsDirectory) result.SetAttribute("Size", Size.ToString());
      result.SetAttribute("FullName", FileSystemInfo.FullName);
      result.SetAttribute("CreationTimeUTC", FileSystemInfo.CreationTimeUtc.ToString());
      result.SetAttribute("LastAccessTimeUTC", FileSystemInfo.LastAccessTimeUtc.ToString());
      result.SetAttribute("LastWriteTimeUTC", FileSystemInfo.LastWriteTimeUtc.ToString());
      result.SetAttribute("Attributes", FileSystemInfo.Attributes.ToString());

      return result;
    }
  }
}
