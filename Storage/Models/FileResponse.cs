namespace Storage.Models;
public class FileResponse
{
    public byte[] data { get; set; }
    public string Filename { get; set; }

    public FileResponse(byte[] data, string filename)
    {
        this.data = data;
        this.Filename = filename;
    }
}

