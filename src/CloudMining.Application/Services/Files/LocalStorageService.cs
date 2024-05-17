using CloudMining.Application.DTO.File;
using CloudMining.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace CloudMining.Application.Services.Files;

public sealed class LocalStorageService : IStorageService
{
	private readonly string _defaultPath;

	public LocalStorageService(IOptions<StorageSettings> settings)
	{
		_defaultPath = settings.Value.Path;
	}

	public async Task<string> SaveFileAsync(FileDto dto)
	{
		if (!Directory.Exists(_defaultPath))
			Directory.CreateDirectory(_defaultPath);

		var randomFileName = Path.GetRandomFileName();
		var filePath = Path.Combine(_defaultPath, randomFileName);

		await using var fileStream = new FileStream(filePath, FileMode.Create);
		await dto.File.CopyToAsync(fileStream);

		return filePath;
	}
}