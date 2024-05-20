using CloudMining.Contracts.DTO.File;

namespace CloudMining.Contracts.Interfaces;

public interface IStorageService
{
	Task<string> SaveFileAsync(FileDto dto);
}