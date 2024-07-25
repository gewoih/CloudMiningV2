using CloudMining.Common.DTO;

namespace CloudMining.Common.Services.Storage;

public interface IStorageService
{
	Task<string> SaveFileAsync(FileDto dto);
}