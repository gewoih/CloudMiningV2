namespace CloudMining.Application.Mappings;

public interface IMapper<TDomain, TDto>
{
    TDto ToDto(TDomain model);
    TDomain ToDomain(TDto dto);
}