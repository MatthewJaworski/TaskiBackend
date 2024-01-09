namespace Taski.Api.Dtos;
public class TagDto
{
    public Guid Id { get; }
    public string Name { get; }

    public TagDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

}