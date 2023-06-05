using System.Net;
using System.Xml;
using MediaWidget.Core.Abstract;

namespace MediaWidget.Core.Concrete
{
    public class FileOperations : IFileOperations
    {
        private readonly Func<NetworkCredential, string, IUncPathAuthenticator> pathAuthenticatorFactory;

        public FileOperations(Func<NetworkCredential, string, IUncPathAuthenticator> pathAuthenticatorFactory)
        {
            this.pathAuthenticatorFactory = pathAuthenticatorFactory;
        }

        public void ClearAttributesOfFilesInDirectory(string path, bool recurse)
        {
            var searchOption = SearchOption.TopDirectoryOnly;
            if (recurse)
            {
                searchOption = SearchOption.AllDirectories;
            }

            foreach (string filename in Directory.GetFiles(path, "*", searchOption))
            {
                File.SetAttributes(filename, FileAttributes.Normal);
            }
        }

        public void CopyDirectory(string source, string destination, bool recurse)
        {
            var searchOption = SearchOption.TopDirectoryOnly;
            if (recurse)
            {
                searchOption = SearchOption.AllDirectories;
            }

            string[] files = Directory.GetFiles(source, "*", searchOption);
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    string targetFile = file.Replace(source, destination);
                    CopyFile(file, targetFile, true);
                }
            }
        }

        public void CopyFile(string source, string target, bool overwrite)
        {
            if (FileExists(target))
            {
                TryDeleteFile(target);
            }

            EnsurePathExists(Path.GetDirectoryName(target));
            File.Copy(source, target, overwrite);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public string CreateTempDirectory(string parentDirectory)
        {
            if (string.IsNullOrWhiteSpace(parentDirectory))
            {
                throw new ArgumentNullException(nameof(parentDirectory));
            }

            if (parentDirectory.EndsWith("\\"))
            {
                parentDirectory = parentDirectory.Remove(parentDirectory.Length - 1);
            }

            string result = Path.Combine(parentDirectory, Guid.NewGuid().ToString());
            Directory.CreateDirectory(result);
            return result;
        }

        public string CreateTempDirectory()
        {
            return CreateTempDirectory(GetTempPath());
        }

        public string CreateTempDirectory(NetworkCredential credential, string parentDirectory)
        {
            using (AcquireNetworkAccess(parentDirectory, credential))
            {
                return CreateTempDirectory(parentDirectory);
            }
        }

        public string CreateTempFile()
        {
            return Path.GetTempFileName();
        }

        public string CreateTempFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (path.EndsWith("\\"))
            {
                path = path.Remove(path.Length - 1);
            }

            return Path.Combine(path, Guid.NewGuid().ToString());
        }

        public void DeleteDirectory(string path)
        {
            Directory.Delete(path, false);
        }

        public void DeleteDirectoryRecursive(string path)
        {
            Directory.Delete(path, true);
        }

        public void DeleteFile(string filename)
        {
            File.SetAttributes(filename, FileAttributes.Normal);
            File.Delete(filename);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void EnsureFilePathExists(string filename)
        {
            var path = Path.GetDirectoryName(filename);
            EnsurePathExists(path);
        }

        public void EnsurePathExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public bool FileExists(string filename)
        {
            return File.Exists(filename);
        }

        public string[] GetDirectories(string path, string filter = "*", bool recurse = false)
        {
            var option = SearchOption.TopDirectoryOnly;
            if (recurse)
            {
                option = SearchOption.AllDirectories;
            }

            return Directory.GetDirectories(path, filter, option);
        }

        public DateTime GetDirectoryLastModifiedTime(string path)
        {
            return Directory.GetLastWriteTime(path);
        }

        public DateTime GetFileLastModifiedTime(string filename)
        {
            return File.GetLastWriteTime(filename);
        }

        public string[] GetFiles(string path, string filter = "*", bool recurse = false)
        {
            var option = SearchOption.TopDirectoryOnly;
            if (recurse)
            {
                option = SearchOption.AllDirectories;
            }

            return Directory.GetFiles(path, filter, option);
        }

        public string[] GetFiles(NetworkCredential credential, string path, string filter = "*", bool recurse = false)
        {
            using (AcquireNetworkAccess(path, credential))
            {
                return GetFiles(path, filter, recurse);
            }
        }

        public ulong GetFileSize(string filename)
        {
            var fileInfo = new FileInfo(filename);
            return (ulong)fileInfo.Length;
        }

        public string GetRelativePath(string pathFrom, string pathTo)
        {
            if (string.IsNullOrEmpty(pathFrom))
            {
                throw new ArgumentNullException(nameof(pathFrom));
            }

            if (string.IsNullOrEmpty(pathTo))
            {
                throw new ArgumentNullException(nameof(pathTo));
            }

            var fromUri = new Uri(pathFrom);
            var toUri = new Uri(pathTo);

            if (fromUri.Scheme != toUri.Scheme)
            {
                return pathTo;
            }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE")
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        public string GetTempPath()
        {
            return Path.GetTempPath();
        }

        public void MoveDirectory(string from, string to)
        {
            Directory.Move(from, to);
        }

        public void MoveFile(string from, string to)
        {
            File.Move(from, to);
        }

        public Stream OpenFile(string filename, FileMode fileMode, FileShare fileShare = FileShare.Read)
        {
            return new FileStream(
                filename,
                fileMode,
                fileMode == FileMode.Open ? FileAccess.Read : FileAccess.ReadWrite,
                fileShare);
        }

        public XmlDocument OpenXmlDocument(string filename)
        {
            var document = new XmlDocument();
            document.Load(filename);
            return document;
        }

        public XmlDocument OpenXmlDocument(string filename, NetworkCredential credential)
        {
            using (AcquireNetworkAccess(Path.GetDirectoryName(filename), credential))
            {
                return OpenXmlDocument(filename);
            }
        }

        public IXmlDocumentWithDefaultNamespace OpenXmlDocumentWithDefaultNamespace(
            string filename,
            string defaultNamespace,
            string defaultNamespaceUri)
        {
            var document = new XmlDocumentWithDefaultNamespace(defaultNamespace, defaultNamespaceUri);
            document.Load(filename);
            return document;
        }

        public void RemoveReadOnlyAttributeFromFile(string filename)
        {
            if (File.Exists(filename))
            {
                FileAttributes attributes = File.GetAttributes(filename);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attributes &= ~FileAttributes.ReadOnly;
                }

                File.SetAttributes(filename, attributes);
            }
        }

        public void SetCreatedTimestamp(string filename, DateTime timestamp)
        {
            File.SetCreationTime(filename, timestamp);
        }

        public void SetModifiedTimestamp(string filename, DateTime timestamp)
        {
            File.SetLastWriteTime(filename, timestamp);
        }

        public void TryDeleteFile(string filename)
        {
            try
            {
                DeleteFile(filename);
            }
            catch
            {
            }
        }

        public void WriteAllBytesToFile(string filename, byte[] data)
        {            
            File.WriteAllBytes(filename, data);            
        }

        public void WriteAllTextToFile(string filename, string text)
        {
            File.WriteAllText(filename, text);
        }

        private IUncPathAuthenticator AcquireNetworkAccess(string path, NetworkCredential credential)
        {
            var authenticator = pathAuthenticatorFactory(credential, path);
            authenticator.AcquireAccess();
            return authenticator;
        }
    }
}