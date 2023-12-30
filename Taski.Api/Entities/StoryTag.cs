using Taski.Api.Entities;

public class StoryTag : ITag
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<Story> Stories { get; set; }
}