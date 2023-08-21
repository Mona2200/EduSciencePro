using AutoMapper;
using Azure;
using EduSciencePro.Models;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos;

public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public TagRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<Tag>> GetTags()
    {
        return await _db.Tags.ToListAsync();
    }

    public async Task<List<Tag>> GetTagsByPostId(Guid postId)
    {
        List<TagPost> tagPosts = await _db.TagPosts.Where(t => t.PostId == postId).ToListAsync();
        List<Tag> tags = new();
        foreach (TagPost tagPost in tagPosts)
            tags.Add(await GetTagById(tagPost.TagId));

        return tags;
    }

    public async Task<Tag?> GetTagById(Guid id)
    {
        return await _db.Tags.SingleOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Tag?> GetTagByName(string name)
    {
        return await _db.Tags.SingleOrDefaultAsync(t => t.Name == name);
    }

    public async Task<List<Tag>> GetTagsSearch(string search)
    {
        return await _db.Tags.Where(t => t.Name.ToLower().Contains(search.ToLower())).ToListAsync();
    }

    public async Task Save(string[] tags, Guid postId)
    {
        List<TagPost> oldTagPosts = await _db.TagPosts.Where(t => t.PostId == postId).ToListAsync();
        foreach (TagPost tagPost in oldTagPosts)
            _db.TagPosts.Remove(tagPost);

        foreach (string tag in tags)
        {
            Tag? tryTag = await GetTagByName(tag);
            if (tryTag == null)
            {
                Tag newTag = new Tag() { Name = tag };
                await _db.Tags.AddAsync(newTag);

                TagPost tagPost = new TagPost() { PostId = postId, TagId = newTag.Id };
                await _db.TagPosts.AddAsync(tagPost);
            }
            else
            {
                TagPost newTagPost = new TagPost() { PostId = postId, TagId = tryTag.Id };
                await _db.TagPosts.AddAsync(newTagPost);
            }
        }
    }
}

public interface ITagRepository
{
    Task<List<Tag>> GetTags();
    Task<Tag?> GetTagByName(string name);
    Task<List<Tag>> GetTagsByPostId(Guid postId);
    Task<Tag?> GetTagById(Guid id);
    Task<List<Tag>> GetTagsSearch(string search);
    Task Save(string[] tags, Guid postId);
}
