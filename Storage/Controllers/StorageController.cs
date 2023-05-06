using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Storage.Models;
using Storage.Services;
using System.Text;

namespace Storage.Controllers;

[Route("[controller]")]
[ApiController]
public class StorageController : ControllerBase
{
    private StorageService storageService;
    public const int DEFAULT_EXPIREATION_TIME = 600000;
    public StorageController(StorageService storageService)
    {
        this.storageService = storageService;
    }

    [HttpPost("{expiresIn?}")]
    public UploadReponse UploadFile([FromForm]UploadFileRequest fileRequest, int expiresIn = DEFAULT_EXPIREATION_TIME)
    {
        var fileModel = storageService.AddFile(fileRequest.files, expiresIn);
        return new UploadReponse { FileId = fileModel.FileId };
    }
    [HttpGet("{guid}")]
    public IActionResult GetFile(Guid guid)
    {
        var fileResponse = storageService.GetFile(guid);
        if (fileResponse == null)
        {
            return NotFound();
        }

        return File(fileResponse.data, "application/octet-stream", fileResponse.Filename);
    }
    [HttpDelete("{guid}")]
    public IActionResult DeleteFile(Guid guid)
    {
        var file = storageService.DeleteFile(guid);
        if (!file)
        {
            return NotFound();
        }
        return Ok("File was deleted");
    }
}

