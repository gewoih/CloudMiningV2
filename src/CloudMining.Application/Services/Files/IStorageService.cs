using CloudMining.Application.DTO.File;

namespace CloudMining.Application.Services.Files;

public interface IStorageService
{
	Task<string> SaveFileAsync(FileDto dto);
}