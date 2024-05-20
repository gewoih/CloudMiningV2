using Microsoft.AspNetCore.Http;

namespace CloudMining.Contracts.DTO.File;

public class FileDto
{
	public IFormFile File { get; set; }
}