using Microsoft.AspNetCore.Http;

namespace CloudMining.Application.DTO.File;

public class FileDto
{
	public IFormFile File { get; set; }
}