using SambaServiceClient.Models;

namespace SambaServiceClient
{
    public class SambaClient
    {
        private string smbPath;

        public SambaClient(string smbPth)
        {
            this.smbPath = smbPth;
        }

        public async Task MoveFile(string path, string filepath) 
        {
            await Task.Run(() => File.Copy(path, filepath, true));
        }

        public async Task CopyFile(string source, string destination) 
        {
            await Task.Run(() => File.Copy(source, destination));
        }

        public async Task DeleteFile(string filePath) 
        {
            await Task.Run(() => File.Delete(filePath));
        }

        public async Task<bool> MakeDirectory(string path) 
        {
            await Task.Run(() => Directory.CreateDirectory(path));
            return Directory.Exists(path);
        }

        public DirectoryContentModel ListDirectory(string path) 
        {
            DirectoryInfo dirIndo = new DirectoryInfo(path);

            FileInfo[] files = dirIndo.GetFiles();
            DirectoryInfo[] directories = dirIndo.GetDirectories();

            return new DirectoryContentModel
            {
                Directories = directories.Select(d => d.FullName).ToArray(),
                Files = files.Select(f => f.FullName).ToArray()
            };
        }
    }
}