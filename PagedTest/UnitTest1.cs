using Microsoft.EntityFrameworkCore;
using Reddit.Models;
using Reddit.Repositories;
using Reddit;

namespace PagedTest
{
    public class UnitTest1
    {
        public async Task<PagedList<T>> Pagination<T>(IQueryable<T> items, int pageNumber, int pageSize)
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return null;
            return await PagedList<T>.CreateAsync(items, pageNumber, pageSize);
        }

        //Main test case!
        [Fact]
        public async Task User_test()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);
            List<User> users = Users();

            dbContext.Users.AddRange(users);
            await dbContext.SaveChangesAsync(); // Await the SaveChangesAsync call

            var queryableUsers = dbContext.Users.AsQueryable();

            int pageNumber = 2;
            int pageSize = 2;

            // Act: Call the Pagination method
            var pagedList = await Pagination(queryableUsers, pageNumber, pageSize);

            Assert.Equal("Olivia", pagedList.Items[0].Name);
            Assert.Equal("Ava", pagedList.Items[1].Name);
            Assert.True(pagedList.HasPreviousPage);
            Assert.True(pagedList.HasNextPage);
            Assert.Equal(2, pagedList.Items.Count);


            int pageNumber2 = 1;
            int pageSize2 = 4;

            var pagedList2 = await Pagination(queryableUsers, pageNumber2, pageSize2);


            Assert.Equal("Emma", pagedList2.Items[0].Name);
            Assert.False(pagedList2.HasPreviousPage);
            Assert.True(pagedList2.HasNextPage);
            Assert.Equal("Sophia", pagedList2.Items[1].Name);
            Assert.NotEmpty(pagedList2.Items); //List not empty! test case passed

            pagedList2.Items = new List<User>();

            Assert.Empty(pagedList2.Items); // List empty! test case passed

            int pageNumber3 = 2;
            int pageSize3 = 5;

            var pagedList3 = await Pagination(queryableUsers, pageNumber3, pageSize3);

            Assert.True(pageSize3 > pagedList3.Items.Count); //PageSize is larger then the total number of items on the second page (4 users)
            Assert.Equal(4, pagedList3.Items.Count);
            Assert.True(pagedList3.TotalCount > pageSize3); // total count is more than pageSize

            int pageNumber4 = 4;
            int pageSize4 = 0;

            var pagedList4 = await Pagination(queryableUsers, pageNumber4, pageSize4);
            Assert.True(pagedList4 is null); //page size or number is 0 or less

            //var ex = Assert.Throws<ArgumentNullException>(() => foo.Bar(null));
            //Assert.That(ex.ParamName, Is.EqualTo("bar"));

            int pageNumber5 = 4;
            int pageSize5 = -2;

            var pagedList5 = await Pagination(queryableUsers, pageNumber5, pageSize5);
            Assert.True(pagedList5 is null);

        }


        [Fact]
        public async Task Community_test()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var dbContext = new ApplicationDbContext(options);

            List<Community> communities = Communities();

            dbContext.Communities.AddRange(communities);
            dbContext.Users.AddRange(Users());
            await dbContext.SaveChangesAsync();

            var queryableCommunities = dbContext.Communities.AsQueryable();

            int pageNumber = 1;
            int pageSize = 3;

            // Act: Call the Pagination method
            var pagedList = await Pagination(queryableCommunities, pageNumber, pageSize);

            Assert.Equal(pageNumber, pagedList.PageNumber);
            Assert.Equal(pageSize, pagedList.PageSize);
            Assert.Equal(communities.Count, pagedList.TotalCount);
            Assert.Equal(pageSize, pagedList.Items.Count);
            Assert.True(pagedList.HasNextPage);
            Assert.False(pagedList.HasPreviousPage);

            Assert.Equal(4, pagedList.Items[2].OwnerId);

            var person = await dbContext.Users.FirstAsync(u => u.Id == pagedList.Items[0].OwnerId);
            Assert.Equal(3, person.Id);

            int pageNumber2 = 5;
            int pageSize2 = 2;

            var pagedList2 = await Pagination(queryableCommunities, pageNumber2, pageSize2);
            Assert.Single(pagedList2.Items);
            Assert.False(pagedList2.HasNextPage);
            Assert.True(pagedList2.HasPreviousPage);
            Assert.Equal("DIY & Home Decor", pagedList2.Items[0].Name);
            Assert.NotEqual(communities.Count, pagedList2.Items.Count);

        }

        private List<User> Users()
        {
            return new List<User>
            {
                new User { Name = "Emma", Email = "emma.lifestyle@gmail.com" },
                new User { Name = "Sophia", Email = "sophia.travels@gmail.com" },
                new User { Name = "Olivia", Email = "olivia.fashion@gmail.com" },
                new User { Name = "Ava", Email = "ava.books@gmail.com" },
                new User { Name = "Mia", Email = "mia.art@gmail.com" },
                new User { Name = "Lily", Email = "lily.cooking@gmail.com" },
                new User { Name = "Isabella", Email = "isabella.wellness@gmail.com" },
                new User { Name = "Charlotte", Email = "charlotte.photography@gmail.com" },
                new User { Name = "Amelia", Email = "amelia.music@gmail.com" }
            };
        }

        private List<Community> Communities()
        {
            return new List<Community>
            {
                new Community
                {
                    Name = "Fashion & Style",
                    Description = "Latest trends, outfit inspirations, and styling tips!",
                    OwnerId = 3 // Olivia
                },
                new Community
                {
                    Name = "Travel Diaries",
                    Description = "Discover breathtaking destinations and travel hacks!",
                    OwnerId = 2 // Sophia
                },
                new Community
                {
                    Name = "Book Lovers Club",
                    Description = "Dive into the world of literature and discuss your favorite books.",
                    OwnerId = 4 // Ava
                },
                new Community
                {
                    Name = "Wellness & Self-Care",
                    Description = "A space for mindfulness, self-care routines, and healthy living.",
                    OwnerId = 7 // Isabella
                },
                new Community
                {
                    Name = "Photography Enthusiasts",
                    Description = "Learn photography tips, editing tricks, and share your best shots!",
                    OwnerId = 8 // Charlotte
                },
                new Community
                {
                    Name = "Creative Arts",
                    Description = "A community for artists, designers, and creative minds.",
                    OwnerId = 5 // Mia
                },
                new Community
                {
                    Name = "Cooking & Baking",
                    Description = "Delicious recipes, cooking tips, and baking inspiration.",
                    OwnerId = 6 // Lily
                },
                new Community
                {
                    Name = "Music & Melody",
                    Description = "A place for music lovers to discuss and share tunes.",
                    OwnerId = 9 // Amelia
                },
                new Community
                {
                    Name = "DIY & Home Decor",
                    Description = "Get crafty with home decor ideas and DIY projects!",
                    OwnerId = 1 // Emma
                }
            };
        }

    }
}