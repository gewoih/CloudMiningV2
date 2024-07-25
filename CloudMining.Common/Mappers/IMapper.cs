namespace CloudMining.Common.Mappers;

public interface IMapper<TDomain, TDto>
{
	TDto ToDto(TDomain model);
	TDomain ToDomain(TDto dto);
}