using Chirp.Core;
using Chirp.Infrastructure;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Chirp.Infrastructure.UnitTest
{
    public class LikeRepositoryTest
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<ChirpDBContext> _contextOptions;

        public LikeRepositoryTest()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<ChirpDBContext>()
                .UseSqlite(_connection)
                .Options;

            var context = new ChirpDBContext(_contextOptions);
            context.Database.EnsureCreated();
        }

        [Fact]
        public void LikeCheepAddsLikeToDatabase()
        {
            var context = new ChirpDBContext(_contextOptions);
            var likeRepository = new LikeRepository(context);

            // Arrange
            var newLike = new Like { AuthorId = 1, CheepId = 1 };

            // Act
            likeRepository.LikeCheep(newLike.AuthorId, newLike.CheepId);

            // Assert
            context.Likes.Should().ContainSingle(l => l.AuthorId == newLike.AuthorId && l.CheepId == newLike.CheepId);
        }

        [Fact]
        public void UnlikeCheepRemovesLikeFromDatabase()
        {
            var context = new ChirpDBContext(_contextOptions);
            var likeRepository = new LikeRepository(context);

            // Arrange
            var newLike = new Like { AuthorId = 1, CheepId = 1 };
            context.Likes.Add(newLike);
            context.SaveChanges();

            // Act
            likeRepository.UnlikeCheep(newLike.AuthorId, newLike.CheepId);

            // Assert
            context.Likes.Should().BeEmpty();
        }

        [Fact]
        public void LikeExistsReturnsTrueForExistingLike()
        {
            var context = new ChirpDBContext(_contextOptions);
            var likeRepository = new LikeRepository(context);

            // Arrange
            var newLike = new Like { AuthorId = 1, CheepId = 1 };
            context.Likes.Add(newLike);
            context.SaveChanges();

            // Act
            var result = likeRepository.LikeExists(newLike.AuthorId, newLike.CheepId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void DeleteAllLikesByAuthorIdRemovesAllLikesByAuthorId()
        {
            var context = new ChirpDBContext(_contextOptions);
            var likeRepository = new LikeRepository(context);

            // Arrange
            var newLike1 = new Like { AuthorId = 1, CheepId = 1 };
            var newLike2 = new Like { AuthorId = 1, CheepId = 2 };
            var newLike3 = new Like { AuthorId = 2, CheepId = 1 };
            context.Likes.Add(newLike1);
            context.Likes.Add(newLike2);
            context.Likes.Add(newLike3);
            context.SaveChanges();

            // Act
            likeRepository.DeleteAllLikesByAuthorId(1);

            // Assert
            context.Likes.Should().ContainSingle(l => l.AuthorId == 2 && l.CheepId == 1);
        }
    }
}
