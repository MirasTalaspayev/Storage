namespace Storage.Models;
public class FileModel
{
    public Guid FileId { get; set; }
    public byte[] FileContent { get; set; }
    public string Name { get; set; }
    public int ExpiresIn { get; set; }
    public DateTime CreationTimeUtc { get; set; }
    public byte[] Key { get; set; }
    public byte[] IV { get; set; }

    public FileModel(byte[] FileContent, int expiresIn, string name, byte[] key, byte[] iV)
    {
        this.FileId = Guid.NewGuid();
        this.FileContent = FileContent;
        this.ExpiresIn = expiresIn;
        this.CreationTimeUtc = DateTime.UtcNow;
        this.Name = name;
        this.Key = key;
        this.IV = iV;
    }
}

