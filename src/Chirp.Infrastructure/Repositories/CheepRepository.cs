using Chirp.Core;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure;

public class CheepRepository : ICheepRepository
{
    private ChirpDBContext _db;
    protected IAuthorRepository _authorRepository;
    protected IFollowRepository _followRepository;

    public CheepRepository(ChirpDBContext db, IFollowRepository followRepository, IAuthorRepository authorRepository)
    {
        _db = db;
        _followRepository = followRepository;
        _authorRepository = authorRepository;
    }
    
    public void StoreCheep(Cheep cheep)
    {   
        var newCheepId = _db.Cheeps.Any() ? _db.Cheeps.Max(cheep => cheep.CheepId) + 1 : 1;
        StoreCheeps(new List<Cheep> { new Cheep { AuthorId= cheep.AuthorId, CheepId = newCheepId, Text = cheep.Text, TimeStamp = cheep.TimeStamp} });
    }

    public void StoreCheeps(List<Cheep> entities)
    {
        _db.Cheeps.AddRange(entities);
        _db.SaveChanges();
    }

    public IEnumerable<Cheep> QueryCheeps(int pageNumber, int amount, string? author = null, bool isAuthor = false)
    {
        int startIndex = (pageNumber -1) * amount;
        
        IQueryable<Cheep> queryResult;

        if (string.IsNullOrEmpty(author))
        {
            queryResult = _db.Cheeps;
        } else {

            var authors = _authorRepository.FindAuthorsByName(author);
            if(!authors.Any()) return new List<Cheep>();
            var authorId = authors.First().AuthorId;
            IEnumerable<int> followedIds = new List<int>();
            if (isAuthor) {
                followedIds = _followRepository.FindFollowingByAuthorId(authorId).Select(f => f.FollowedId);
            }
            queryResult = _db.Cheeps.Where(c => followedIds.Contains(c.AuthorId) || c.AuthorId == authorId);
        }

        return queryResult.OrderByDescending(c => c.TimeStamp).Skip(startIndex).Include(c => c.Author).Take(amount);
    }
        
    public int QueryCheepCount(string? author = null, bool isAuthor = false)
    {
        IQueryable<Cheep> queryResult;

        if (string.IsNullOrEmpty(author))
        {
            queryResult = _db.Cheeps;
        } else {
            var authors = _authorRepository.FindAuthorsByName(author);
            if(!authors.Any()) return 0;
            var authorId = authors.First().AuthorId;
            IEnumerable<int> followedIds = new List<int>();
            if (isAuthor) {
                followedIds = _followRepository.FindFollowingByAuthorId(authorId).Select(f => f.FollowedId);
            }
            queryResult = _db.Cheeps.Where(c => followedIds.Contains(c.AuthorId) || c.AuthorId == authorId);
        }

        return queryResult.Count();
    }

    public void DeleteCheep(Cheep cheep) 
    {
        _db.Cheeps.Remove(cheep);
        _db.SaveChanges();
    }

    public void DeleteAllCheepsByAuthorId(int authorId) 
    {
        _db.Cheeps.Where(c => c.AuthorId == authorId).ToList().ForEach(c => _db.Cheeps.Remove(c));
        _db.SaveChanges();
    }

}