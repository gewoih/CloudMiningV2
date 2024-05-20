using CloudMining.Interfaces.DTO.File;

namespace CloudMining.Interfaces.Interfaces;

public interface IStorageService
{
	Task<string> SaveFileAsync(FileDto dto);
}