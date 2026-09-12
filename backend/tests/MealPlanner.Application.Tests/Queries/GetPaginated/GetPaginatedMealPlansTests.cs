// using DannyGoodacre.Cqrs.Testing;
// using DannyGoodacre.Primitives;
// using DannyGoodacre.Testing;
// using MealPlanner.Application;
// using MealPlanner.Application.Abstractions.Repositories;
// using MealPlanner.Application.Models;
// using MealPlanner.Application.Entities;
// using MealPlanPlanner.Application.Queries;
// using Microsoft.Extensions.Logging;
// using Moq;
// using NUnit.Framework;
//
// namespace MealPlanPlanner.Application.Tests.Queries.GetPaginated;
//
// [TestFixture]
// internal sealed class GetPaginatedMealPlansTests : QueryHandlerTestBase<GetPaginatedMealPlansHandler, PaginatedPublicEntityResponse<MealPlanResponse>>
// {
//     protected override string QueryName => "Get Paginated MealPlans";
//
//     private int _requestLimit;
//
//     private int _requestPage;
//
//     private int _testCount;
//
//     private List<MealPlan> _testMealPlans = null!;
//
//     private PaginatedPublicEntityResponse<MealPlanResponse> _testResponse = null!;
//
//     private Mock<IMealPlanRepository> _foodRepositoryMock;
//
//     protected override IResult<<PaginatedPublicEntityResponse<MealPlanResponse>>> Act()
//         => QueryHandler.ExecuteAsync(_requestPage, _requestLimit, CancellationToken);
//
//     [SetUp]
//     public void SetUp()
//     {
//         _foodRepositoryMock = new Mock<IMealPlanRepository>();
//
//         QueryHandler = new GetPaginatedMealPlansHandler(LoggerMock.Object, _foodRepositoryMock.Object);
//     }
//
//     [Test]
//     public async Task WhenRequestInvalid_ShouldReturnInvalid()
//     {
//         // Arrange
//         _requestLimit = 0;
//
//         _requestPage = -1;
//
//         SetupLogger_FailedValidation($"Page:{Environment.NewLine}  - Must be greater than or equal to 0.{Environment.NewLine}Limit:{Environment.NewLine}  - Must be greater than 0.");
//
//         // Act
//         var result = await Act();
//
//         // Assert
//         AssertInvalid(result);
//     }
//
//     [Test]
//     public async Task WhenNoMealPlansFound_ShouldReturnSuccessWithNoItems()
//     {
//         // Arrange
//         _requestPage = 2;
//
//         _requestLimit = 3;
//
//         _testCount = 0;
//
//         _testMealPlans = [];
//
//         _testResponse = new PaginatedPublicEntityResponse<MealPlanResponse>
//         {
//             Items = [],
//             CurrentPage = _requestPage,
//             TotalItemsCount = 0,
//             TotalPagesCount = 0
//         };
//
//         SetupLogger_QueryStarted();
//
//         SetupMealPlanRepository_GetCountAsync();
//
//         SetupMealPlanRepository_GetPaginatedAsync();
//
//         // Act
//         var result = await Act();
//
//         // Assert
//         AssertSuccess(result, _testResponse);
//     }
//
//     [Test]
//     public async Task WhenFullPageRequested_ShouldReturnSuccessWithFullPageOfItems()
//     {
//         // Arrange
//         _requestPage = 2;
//
//         _requestLimit = 3;
//
//         _testCount = 11;
//
//         _testMealPlans = CreateTestMealPlans(3);
//
//         _testResponse = new PaginatedPublicEntityResponse<MealPlanResponse>
//         {
//             Items = _testMealPlans.Select(x => x.ToResponse()).ToList(),
//             CurrentPage = _requestPage,
//             TotalItemsCount = _testCount,
//             TotalPagesCount = 4
//         };
//
//         SetupLogger_QueryStarted();
//
//         SetupMealPlanRepository_GetCountAsync();
//
//         SetupMealPlanRepository_GetPaginatedAsync();
//
//         // Act
//         var result = await Act();
//
//         // Assert
//         AssertSuccess(result, _testResponse);
//     }
//
//     [Test]
//     public async Task WhenLastPageRequested_ShouldReturnSuccessWithPartialPageOfItems()
//     {
//         // Arrange
//         _requestPage = 4;
//
//         _requestLimit = 3;
//
//         _testCount = 11;
//
//         _testMealPlans = CreateTestMealPlans(2);
//
//         _testResponse = new PaginatedPublicEntityResponse<MealPlanResponse>
//         {
//             Items = _testMealPlans.Select(x => x.ToResponse()).ToList(),
//             CurrentPage = 4,
//             TotalItemsCount = 11,
//             TotalPagesCount = 4,
//         };
//
//         SetupLogger_QueryStarted();
//
//         SetupMealPlanRepository_GetCountAsync();
//
//         SetupMealPlanRepository_GetPaginatedAsync();
//
//         // Act
//         var result = await Act();
//
//         // Assert
//         AssertSuccess(result, _testResponse);
//     }
//
//     [Test]
//     public async Task WhenPageOutOfRange_ShouldReturnSuccessWithNoItems()
//     {
//         // Arrange
//         _requestLimit = 3;
//
//         _requestPage = 100;
//
//         _testCount = 11;
//
//         _testMealPlans = [];
//
//         _testResponse = new PaginatedPublicEntityResponse<MealPlanResponse>
//         {
//             Items = [],
//             CurrentPage = _requestPage,
//             TotalItemsCount = 11,
//             TotalPagesCount = 4
//         };
//
//         SetupLogger_QueryStarted();
//
//         SetupMealPlanRepository_GetCountAsync();
//
//         SetupMealPlanRepository_GetPaginatedAsync();
//
//         // Act
//         var result = await Act();
//
//         // Assert
//         AssertSuccess(result, _testResponse);
//     }
//
//     private static List<MealPlan> CreateTestMealPlans(int count)
//     {
//         return Enumerable.Range(0, count).Select(x => new MealPlan
//         {
//             PublicId = new Guid($"00000000-0000-0000-0000-00000000000{x}"),
//             Name = $"Test MealPlan Name {x}",
//             NormalizedName = $"Test Normalized MealPlan Name {x}"
//         }).ToList();
//     }
//
//     private void SetupLogger_QueryStarted()
//         => LoggerMock.Setup(LogLevel.Information, $"Query '{QueryName}' started with Page '{_requestPage}' and Limit '{_requestLimit}'.");
//
//     private void SetupMealPlanRepository_GetCountAsync()
//         => _foodRepositoryMock
//             .Setup(x =>
//                 x.GetCountAsync(It.Is<CancellationToken>(y => y == CancellationToken)))
//             .ReturnsAsync(_testCount)
//             .Verifiable(Times.Once);
//
//     private void SetupMealPlanRepository_GetPaginatedAsync()
//         => _foodRepositoryMock
//             .Setup(x =>
//                 x.GetPaginatedAsync(
//                     It.Is<int>(y => y == _requestPage),
//                     It.Is<int>(y => y == _requestLimit),
//                     It.Is<CancellationToken>(y => y == CancellationToken)))
//             .ReturnsAsync(_testMealPlans)
//             .Verifiable(Times.Once);
// }
