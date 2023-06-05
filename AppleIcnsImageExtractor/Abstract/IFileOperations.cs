using System.Net;
using System.Xml;

namespace MediaWidget.Core.Abstract;

public interface IFileOperations
{
    void ClearAttributesOfFilesInDirectory(string path, bool recurse);

    void CopyDirectory(string source, string destination, bool recurse);

    void CopyFile(string source, string target, bool overwrite);

    void CreateDirectory(string path);

    string CreateTempDirectory();

    string CreateTempDirectory(string parentDirectory);

    string CreateTempDirectory(NetworkCredential credential, string parentDirectory);

    string CreateTempFile();

    string CreateTempFile(string path);

    void DeleteDirectory(string path);

    void DeleteDirectoryRecursive(string path);

    void DeleteFile(string filename);

    bool DirectoryExists(string path);

    void EnsureFilePathExists(string filename);

    void EnsurePathExists(string path);

    bool FileExists(string filename);

    string[] GetDirectories(string path, string filter = "*", bool recurse = false);

    DateTime GetDirectoryLastModifiedTime(string path);

    DateTime GetFileLastModifiedTime(string filename);

    string[] GetFiles(string path, string filter = "*", bool recurse = false);

    string[] GetFiles(NetworkCredential credential, string path, string filter = "*", bool recurse = false);

    ulong GetFileSize(string filename);

    string GetRelativePath(string pathFrom, string pathTo);

    string GetTempPath();

    void MoveDirectory(string from, string to);

    void MoveFile(string from, string to);

    Stream OpenFile(string filename, FileMode fileMode, FileShare fileShare = FileShare.Read);

    XmlDocument OpenXmlDocument(string filename);

    XmlDocument OpenXmlDocument(string filename, NetworkCredential credential);

    IXmlDocumentWithDefaultNamespace OpenXmlDocumentWithDefaultNamespace(
        string filename,
        string defaultNamespace,
        string defaultNamespaceUri);

    void RemoveReadOnlyAttributeFromFile(string filename);

    void SetCreatedTimestamp(string filename, DateTime timestamp);

    void SetModifiedTimestamp(string filename, DateTime timestamp);

    void TryDeleteFile(string filename);

    void WriteAllBytesToFile(string filename, byte[] data);
    
    void WriteAllTextToFile(string filename, string text);
}