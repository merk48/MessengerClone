using AutoMapper;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.IRepository;
using MessengerClone.Domain.IUnitOfWork;
using MessengerClone.Service.Features.Chats.Interfaces;
using MessengerClone.Service.Features.Chats.Services;
using MessengerClone.Service.Features.Messages.Interfaces;
using MessengerClone.Service.Features.Users.Interfaces;
using MockQueryable;
using Moq;
using MockQueryable.Moq;
using FluentAssertions;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Chats.DTOs;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.General.DTOs;
using System.Linq.Expressions;

namespace MessengerClone.Tests.Services
{

    public class ChatServiceTests
    {
        //private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        //private readonly Mock<IRepository<Chat>> _mockChatRepo;
        //private readonly Mock<IMapper> _mockMapper;
        //private readonly Mock<IUserService> _mockUserService;
        //private readonly Mock<IChatMemeberService> _mockMemberService;
        //private readonly Mock<IMessageService> _mockMessageService;

        //private readonly ChatService _chatService;

        //public ChatServiceTests()
        //{
        //    _mockUnitOfWork = new Mock<IUnitOfWork>();
        //    _mockChatRepo = new Mock<IRepository<Chat>>();
        //    _mockMapper = new Mock<IMapper>();
        //    _mockUserService = new Mock<IUserService>();
        //    _mockMemberService = new Mock<IChatMemeberService>();
        //    _mockMessageService = new Mock<IMessageService>();

        //    // Whenever the service does _unitOfWork.Repository<Chat>(),
        //    // return our _mockChatRepo.Object
        //    _mockUnitOfWork
        //        .Setup(u => u.Repository<Chat>())
        //        .Returns(_mockChatRepo.Object);

        //    _chatService = new ChatService(
        //        _mockUnitOfWork.Object,
        //        _mockMapper.Object,
        //        _mockUserService.Object,
        //        _mockMemberService.Object,
        //        _mockMessageService.Object
        //    );
        //}

        //#region GetAllForSidebarByUserIdAsync Tests
        ////[Fact]
        ////public async Task GetAllForSidebarByUserIdAsync_ShouldReturnAllChats_WhenNoPaging()
        ////{
        ////    // Arrange
        ////    int userId = 42;

        ////    // Create 3 Chat entities in memory; only 2 have ChatMember.UserId == 42
        ////    var inMemoryChats = new List<Chat>
        ////{
        ////    new Chat {
        ////        Id = 100,
        ////        Type = enChatType.Direct,
        ////        CreatedAt = new DateTime(2023, 1, 1),
        ////        ChatTheme = enChatTheme.Light,
        ////        LastMessage = new LastMessageSnapshot
        ////        {
        ////            Id = 1,
        ////            Content = "Hello",
        ////            SentAt = new DateTime(2023, 1, 10),
        ////            SenderUserame = "alice",
        ////            Type = enMessageType.Text
        ////        },
        ////        ChatMembers = new List<ChatMember>
        ////        {
        ////            new ChatMember { UserId = 42, CreatedAt = new DateTime(2023, 1, 1) }
        ////        },
        ////        Messages = new List<Message>()
        ////    },
        ////    new Chat {
        ////        Id = 200,
        ////        Type = enChatType.Group,
        ////        CreatedAt = new DateTime(2023, 2, 1),
        ////        ChatTheme = enChatTheme.Light,
        ////        LastMessage = new LastMessageSnapshot
        ////        {
        ////            Id = 2,
        ////            Content = "Hey there",
        ////            SentAt = new DateTime(2023, 2, 5),
        ////            SenderUserame = "bob",
        ////            Type = enMessageType.Text
        ////        },
        ////        ChatMembers = new List<ChatMember>
        ////        {
        ////            new ChatMember { UserId = 42, CreatedAt = new DateTime(2023, 2, 1) }
        ////        },
        ////        Messages = new List<Message>()
        ////    },
        ////    new Chat {
        ////        Id = 300,
        ////        Type = enChatType.Direct,
        ////        CreatedAt = new DateTime(2023, 3, 1),
        ////        ChatTheme = enChatTheme.Light,
        ////        LastMessage = new LastMessageSnapshot
        ////        {
        ////            Id = 3,
        ////            Content = "Greetings",
        ////            SentAt = new DateTime(2023, 3, 7),
        ////            SenderUserame = "carol",
        ////            Type = enMessageType.Text
        ////        },
        ////        ChatMembers = new List<ChatMember>
        ////        {
        ////            new ChatMember { UserId = 99, CreatedAt = new DateTime(2023, 3, 1) }
        ////        },
        ////        Messages = new List<Message>()
        ////    }
        ////};

        ////    // Build a mock IQueryable<Chat> from that list
        ////    var mockChatQueryable = inMemoryChats.AsQueryable().BuildMock();

        ////    // When ChatService does `_unitOfWork.Repository<Chat>().Table`, return this IQueryable
        ////    _mockChatRepo.Setup(r => r.Table).Returns(mockChatQueryable);

        ////    // Prepare the “mapped” ChatSidebarDto list containing 2 items (for Chat Id 100 & 200)
        ////    var inMemoryDtos = new List<ChatSidebarDto>
        ////{
        ////    new ChatSidebarDto
        ////    {
        ////        Id = 100,
        ////        Type = enChatType.Direct,
        ////        LastMessage = new LastMessageDto
        ////        {
        ////            Id = 1,
        ////            Content = "Hello",
        ////            SentAt = new DateTime(2023, 1, 10),
        ////            SenderUserame = "alice",
        ////            Type = enMessageType.Text
        ////        },
        ////        UnreadCount = 0,              // will be overwritten by IMessageService mock
        ////        Title = "Direct Chat with Alice",
        ////        GroupPhotoUrl = "",           // not used in a direct chat
        ////        Description = null
        ////    },
        ////    new ChatSidebarDto
        ////    {
        ////        Id = 200,
        ////        Type = enChatType.Group,
        ////        LastMessage = new LastMessageDto
        ////        {
        ////            Id = 2,
        ////            Content = "Hey there",
        ////            SentAt = new DateTime(2023, 2, 5),
        ////            SenderUserame = "bob",
        ////            Type = enMessageType.Text
        ////        },
        ////        UnreadCount = 0,
        ////        Title = "Project Group",
        ////        GroupPhotoUrl = "https://example.com/group200.png",
        ////        Description = "Team chat"
        ////    }
        ////};

        ////    // Mock AutoMapper: whenever Map<List<ChatSidebarDto>>(...) is invoked, return our inMemoryDtos
        ////    _mockMapper
        ////        .Setup(m => m.Map<List<ChatSidebarDto>>(
        ////            It.IsAny<IEnumerable<Chat>>(),
        ////            (Action<IMappingOperationOptions<object, List<ChatSidebarDto>>>)It.IsAny<Action< MappingOperationOptions<IEnumerable<Chat>, List<ChatSidebarDto>>>> ()
        ////        ))
        ////        .Returns(inMemoryDtos);

        ////    // Mock IMessageService.GetChatUnreadMessagesCount for both chat IDs
        ////    _mockMessageService
        ////        .Setup(ms => ms.GetChatUnreadMessagesCount(100, userId))
        ////        .ReturnsAsync(Result<int>.Success(5));
        ////    _mockMessageService
        ////        .Setup(ms => ms.GetChatUnreadMessagesCount(200, userId))
        ////        .ReturnsAsync(Result<int>.Success(2));

        ////    // Act
        ////    var result = await _chatService.GetAllForSidebarByUserIdAsync(
        ////        userId,
        ////        page: null,
        ////        size: null,
        ////        filter: null
        ////    );

        ////    // Assert: Success path
        ////    result.Succeeded.Should().BeTrue();
        ////    var dataResult = result.Data;
        ////    dataResult.TotalRecordsCount.Should().Be(2); // only Chat 100 & 200 have UserId = 42

        ////    var returnedDtos = dataResult.Data.ToList();
        ////    returnedDtos.Should().HaveCount(2);

        ////    // They should be ordered descending by LastMessage.SentAt:
        ////    //  Chat 200 (SentAt = 2023-02-05) comes before Chat 100 (SentAt = 2023-01-10)
        ////    returnedDtos[0].Id.Should().Be(200);
        ////    returnedDtos[1].Id.Should().Be(100);

        ////    // Ensure UnreadCount was set by IMessageService mock:
        ////    returnedDtos.First(d => d.Id == 100).UnreadCount.Should().Be(5);
        ////    returnedDtos.First(d => d.Id == 200).UnreadCount.Should().Be(2);
        ////}

        ////[Fact]
        ////public async Task GetAllForSidebarByUserIdAsync_ShouldReturnPaginatedResult_WhenPageAndSizeProvided()
        ////{
        ////    // Arrange
        ////    int userId = 42;

        ////    // Two in-memory Chat entities (both belong to userId = 42)
        ////    var inMemoryChats = new List<Chat>
        ////{
        ////    new Chat {
        ////        Id = 100,
        ////        Type = enChatType.Direct,
        ////        CreatedAt = new DateTime(2023, 1, 1),
        ////        ChatTheme = enChatTheme.Light,
        ////        LastMessage = new LastMessageSnapshot
        ////        {
        ////            Id = 1,
        ////            Content = "Hello",
        ////            SentAt = new DateTime(2023, 1, 10),
        ////            SenderUserame = "alice",
        ////            Type = enMessageType.Text
        ////        },
        ////        ChatMembers = new List<ChatMember>
        ////        {
        ////            new ChatMember { UserId = 42, CreatedAt = new DateTime(2023, 1, 1) }
        ////        },
        ////        Messages = new List<Message>()
        ////    },
        ////    new Chat {
        ////        Id = 200,
        ////        Type = enChatType.Group,
        ////        CreatedAt = new DateTime(2023, 2, 1),
        ////        ChatTheme = enChatTheme.Light,
        ////        LastMessage = new LastMessageSnapshot
        ////        {
        ////            Id = 2,
        ////            Content = "Hey there",
        ////            SentAt = new DateTime(2023, 2, 5),
        ////            SenderUserame = "bob",
        ////            Type = enMessageType.Text
        ////        },
        ////        ChatMembers = new List<ChatMember>
        ////        {
        ////            new ChatMember { UserId = 42, CreatedAt = new DateTime(2023, 2, 1) }
        ////        },
        ////        Messages = new List<Message>()
        ////    }
        ////};

        ////    var mockChatQueryable = inMemoryChats.AsQueryable().BuildMock();
        ////    _mockChatRepo.Setup(r => r.Table).Returns(mockChatQueryable);

        ////    // Paging: page = 1, size = 1 → only the top Chat (Id = 200) after sorting
        ////    _mockMapper
        ////        .Setup(m => m.Map<List<ChatSidebarDto>>(
        ////            It.IsAny<IEnumerable<Chat>>(),
        ////            (Action<IMappingOperationOptions<object, List<ChatSidebarDto>>>)It.IsAny<Action<MappingOperationOptions<IEnumerable<Chat>, List<ChatSidebarDto>>>>()
        ////        ))
        ////        .Returns((IEnumerable<Chat> srcChats, Action<MappingOperationOptions<IEnumerable<Chat>, List<ChatSidebarDto>>> opts) =>
        ////        {
        ////            // srcChats here should contain exactly one Chat (Id = 200), because of pagination
        ////            var chatEntity = srcChats.First();
        ////            return new List<ChatSidebarDto>
        ////            {
        ////            new ChatSidebarDto
        ////            {
        ////                Id = chatEntity.Id,
        ////                Type = chatEntity.Type,
        ////                LastMessage = chatEntity.LastMessage == null
        ////                    ? null
        ////                    : new LastMessageDto
        ////                    {
        ////                        Id = chatEntity.LastMessage.Id,
        ////                        Content = chatEntity.LastMessage.Content,
        ////                        SentAt = chatEntity.LastMessage.SentAt,
        ////                        SenderUserame = chatEntity.LastMessage.SenderUserame,
        ////                        Type = chatEntity.LastMessage.Type
        ////                    },
        ////                UnreadCount = 0,
        ////                Title = chatEntity.Type == enChatType.Direct
        ////                    ? "Direct Chat"
        ////                    : "Group Chat",
        ////                GroupPhotoUrl = chatEntity.Type == enChatType.Group
        ////                    ? "https://example.com/group200.png"
        ////                    : "",
        ////                Description = chatEntity.Type == enChatType.Group
        ////                    ? "Team chat"
        ////                    : null
        ////            }
        ////            };
        ////        });

        ////    _mockMessageService
        ////        .Setup(ms => ms.GetChatUnreadMessagesCount(200, userId))
        ////        .ReturnsAsync(Result<int>.Success(3));

        ////    // Act
        ////    int pageNumber = 1, pageSize = 1;
        ////    var result = await _chatService.GetAllForSidebarByUserIdAsync(
        ////        userId,
        ////        page: pageNumber,
        ////        size: pageSize,
        ////        filter: null
        ////    );

        ////    // Assert: Should return a PaginatedResult<ChatSidebarDto>
        ////    result.Succeeded.Should().BeTrue();
        ////    result.Data.Should().BeOfType<PaginatedResult<ChatSidebarDto>>();

        ////    var paginated = (PaginatedResult<ChatSidebarDto>)result.Data;
        ////    paginated.PageNumber.Should().Be(1);
        ////    paginated.PageSize.Should().Be(1);
        ////    paginated.TotalRecordsCount.Should().Be(2); // still 2 in total, even though pageSize=1

        ////    paginated.Data.Should().HaveCount(1);
        ////    paginated.Data.First().Id.Should().Be(200);
        ////    paginated.Data.First().UnreadCount.Should().Be(3);
        ////}

        ////[Fact]
        ////public async Task GetAllForSidebarByUserIdAsync_ShouldApplyFilter()
        ////{
        ////    // Arrange
        ////    int userId = 42;

        ////    // Two in-memory Chat entities, both belong to userId = 42
        ////    var inMemoryChats = new List<Chat>
        ////{
        ////    new Chat {
        ////        Id = 200,
        ////        Type = enChatType.Group,
        ////        CreatedAt = new DateTime(2023, 2, 1),
        ////        ChatTheme = enChatTheme.Light,
        ////        LastMessage = new LastMessageSnapshot
        ////        {
        ////            Id = 2,
        ////            Content = "Hey there",
        ////            SentAt = new DateTime(2023, 2, 5),
        ////            SenderUserame = "bob",
        ////            Type = enMessageType.Text
        ////        },
        ////        ChatMembers = new List<ChatMember>
        ////        {
        ////            new ChatMember { UserId = 42, CreatedAt = new DateTime(2023, 2, 1) }
        ////        },
        ////        Messages = new List<Message>()
        ////    },
        ////    new Chat {
        ////        Id = 300,
        ////        Type = enChatType.Direct,
        ////        CreatedAt = new DateTime(2023, 3, 1),
        ////        ChatTheme = enChatTheme.Light,
        ////        LastMessage = new LastMessageSnapshot
        ////        {
        ////            Id = 3,
        ////            Content = "Greetings",
        ////            SentAt = new DateTime(2023, 3, 7),
        ////            SenderUserame = "carol",
        ////            Type = enMessageType.Text
        ////        },
        ////        ChatMembers = new List<ChatMember>
        ////        {
        ////            new ChatMember { UserId = 42, CreatedAt = new DateTime(2023, 3, 1) }
        ////        },
        ////        Messages = new List<Message>()
        ////    }
        ////};

        ////    var mockChatQueryable = inMemoryChats.AsQueryable().BuildMock();
        ////    _mockChatRepo.Setup(r => r.Table).Returns(mockChatQueryable);

        ////    // Filter = only chat with Id == 200
        ////    Expression<Func<Chat, bool>> filter = c => c.Id == 200;

        ////    _mockMapper
        ////        .Setup(m => m.Map<List<ChatSidebarDto>>(
        ////            It.IsAny<IEnumerable<Chat>>(),
        ////            (Action<IMappingOperationOptions<object, List<ChatSidebarDto>>>)It.IsAny<Action<MappingOperationOptions<IEnumerable<Chat>, List<ChatSidebarDto>>>>()
        ////        ))
        ////        .Returns((IEnumerable<Chat> srcChats, Action<MappingOperationOptions<IEnumerable<Chat>, List<ChatSidebarDto>>> opts) =>
        ////        {
        ////            var onlyChat200 = srcChats.Single();
        ////            return new List<ChatSidebarDto>
        ////            {
        ////            new ChatSidebarDto
        ////            {
        ////                Id = onlyChat200.Id,
        ////                Type = onlyChat200.Type,
        ////                LastMessage = onlyChat200.LastMessage == null
        ////                    ? null
        ////                    : new LastMessageDto
        ////                    {
        ////                        Id = onlyChat200.LastMessage.Id,
        ////                        Content = onlyChat200.LastMessage.Content,
        ////                        SentAt = onlyChat200.LastMessage.SentAt,
        ////                        SenderUserame = onlyChat200.LastMessage.SenderUserame,
        ////                        Type = onlyChat200.LastMessage.Type
        ////                    },
        ////                UnreadCount = 0,
        ////                Title = onlyChat200.Type == enChatType.Direct
        ////                    ? "Direct Chat"
        ////                    : "Group Chat",
        ////                GroupPhotoUrl = onlyChat200.Type == enChatType.Group
        ////                    ? "https://example.com/group200.png"
        ////                    : "",
        ////                Description = onlyChat200.Type == enChatType.Group
        ////                    ? "Team chat"
        ////                    : null
        ////            }
        ////            };
        ////        });

        ////    _mockMessageService
        ////        .Setup(ms => ms.GetChatUnreadMessagesCount(200, userId))
        ////        .ReturnsAsync(Result<int>.Success(7));

        ////    // Act
        ////    var result = await _chatService.GetAllForSidebarByUserIdAsync(
        ////        userId,
        ////        page: null,
        ////        size: null,
        ////        filter: filter
        ////    );

        ////    // Assert
        ////    result.Succeeded.Should().BeTrue();
        ////    var dataResult = result.Data;
        ////    dataResult.TotalRecordsCount.Should().Be(1);

        ////    var dto = dataResult.Data.Single();
        ////    dto.Id.Should().Be(200);
        ////    dto.UnreadCount.Should().Be(7);
        ////}

        ////[Fact]
        ////public async Task GetAllForSidebarByUserIdAsync_ShouldReturnFailure_OnException()
        ////{
        ////    // Arrange
        ////    int userId = 42;

        ////    // Make _mockChatRepo.Table throw an exception
        ////    _mockChatRepo
        ////        .Setup(r => r.Table)
        ////        .Throws(new InvalidOperationException("DB failure"));

        ////    // Act
        ////    var result = await _chatService.GetAllForSidebarByUserIdAsync(
        ////        userId,
        ////        page: null,
        ////        size: null,
        ////        filter: null
        ////    );

        ////    // Assert
        ////    result.Succeeded.Should().BeFalse();
        ////    result.Errors.ToString().Should().Be("Failed to retrieve user chats from the database");
        ////    result.Data.Should().BeNull();
        ////}

        //#endregion

        //#region GetUserAllChatIdsAsync Tests
        //[Fact]
        //public async Task GetUserAllChatIdsAsync_ShouldReturnChatIds_WhenUserIsMember()
        //{
        //    // Arrange
        //    var userId = 1;
        //    var chatList = new List<Chat>
        //        {
        //            new Chat { Id = 10, ChatMembers = new List<ChatMember> { new ChatMember { UserId = 1 } } },
        //            new Chat { Id = 20, ChatMembers = new List<ChatMember> { new ChatMember { UserId = 1 } } },
        //            new Chat { Id = 30, ChatMembers = new List<ChatMember> { new ChatMember { UserId = 99 } } }, // Not included
        //        };

        //    var mockChatQueryable = chatList.AsQueryable().BuildMock(); // Requires MockQueryable

        //    _mockChatRepo.Setup(r => r.Table).Returns(mockChatQueryable);

        //    // Act
        //    var result = await _chatService.GetUserAllChatIdsAsync(userId);

        //    // Assert
        //    Assert.True(result.Succeeded);
        //    Assert.Equal(2, result.Data.TotalRecordsCount);
        //    Assert.Contains(10, result.Data.Data);
        //    Assert.Contains(20, result.Data.Data);
        //    Assert.DoesNotContain(30, result.Data.Data);
        //}

        //[Fact]
        //public async Task GetUserAllChatIdsAsync_ShouldReturnEmptyList_WhenNoChatsExist()
        //{
        //    var userId = 5;
        //    var chatList = new List<Chat>();

        //    var mockChatQueryable = chatList.AsQueryable().BuildMock();
        //    _mockChatRepo.Setup(r => r.Table).Returns(mockChatQueryable);

        //    var result = await _chatService.GetUserAllChatIdsAsync(userId);

        //    Assert.True(result.Succeeded);
        //    Assert.Empty(result.Data.Data);
        //    Assert.Equal(0, result.Data.TotalRecordsCount);
        //}

        //[Fact]
        //public async Task GetUserAllChatIdsAsync_ShouldReturnFailure_WhenExceptionOccurs()
        //{
        //    var userId = 1;

        //    _mockChatRepo.Setup(r => r.Table).Throws(new Exception("DB error"));

        //    var result = await _chatService.GetUserAllChatIdsAsync(userId);

        //    Assert.False(result.Succeeded);
        //    Assert.Equal("Failed to retrieve chats ids from the database", string.Join(",", result.Errors));
        //}

        //#endregion
    }

}