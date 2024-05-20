using Microsoft.AspNetCore.Http;

namespace CloudMining.Interfaces.DTO.File;

public class FileDto
{
	public IFormFile File { get; set; }
}