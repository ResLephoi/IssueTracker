# IssueTracker Unit Tests

This folder contains unit tests for the IssueTracker application controllers. The tests use MSTest and Moq to mock dependencies and verify controller behavior.

## Tests Coverage

The tests cover all endpoints in the following controllers:

### BoardController
- `GetById`: Tests retrieving a board by ID (both existing and non-existing)
- `GetBoardWithItemsAndCards`: Tests retrieving a board with its items and cards (both existing and non-existing)
- `GetAll`: Tests retrieving all boards
- `Create`: Tests creating a new board
- `Update`: Tests updating an existing board (with matching and mismatched IDs)
- `Delete`: Tests deleting a board

### CardController
- `GetById`: Tests retrieving a card by ID (both existing and non-existing)
- `GetAll`: Tests retrieving all cards
- `Create`: Tests creating a new card
- `Update`: Tests updating an existing card (with valid and invalid IDs)
- `Delete`: Tests deleting a card

### ItemController
- `GetById`: Tests retrieving an item by ID (both existing and non-existing)
- `GetAll`: Tests retrieving all items
- `Create`: Tests creating a new item
- `Update`: Tests updating an existing item (with matching and mismatched IDs)
- `Delete`: Tests deleting an item

## Running the Tests

1. Open the IssueTracker solution in Visual Studio
2. Make sure all NuGet packages are restored
3. Build the solution
4. Open Test Explorer (Test > Test Explorer)
5. Run all tests or select specific tests to run

## Dependencies

These tests require the following packages:
- MSTest.TestAdapter
- MSTest.TestFramework
- Moq
- Castle.Core (for Moq)
- System.Runtime.CompilerServices.Unsafe (for Moq)
- System.Threading.Tasks.Extensions (for Moq)

## Test Structure

Each test follows the Arrange-Act-Assert pattern:
- **Arrange**: Set up the test dependencies, mocks, and expected values
- **Act**: Call the controller method being tested
- **Assert**: Verify the result matches expectations

## Examples

### Testing a successful GET request:
```csharp
[TestMethod]
public async Task GetById_ExistingId_ReturnsOkWithBoard()
{
    // Arrange
    var boardId = 1;
    var expectedBoard = new Board { Id = boardId, Title = "Test Board" };
    
    _mockBoardRepository.Setup(repo => repo.GetByIdAsync(boardId))
        .ReturnsAsync(expectedBoard);

    // Act
    var result = await _boardController.GetById(boardId);

    // Assert
    var okResult = result as OkObjectResult;
    Assert.IsNotNull(okResult);
    Assert.AreEqual(200, okResult.StatusCode);
    
    var returnedBoard = okResult.Value as Board;
    Assert.IsNotNull(returnedBoard);
    Assert.AreEqual(expectedBoard.Id, returnedBoard.Id);
}
```

### Testing a not found response:
```csharp
[TestMethod]
public async Task GetById_NonExistingId_ReturnsNotFound()
{
    // Arrange
    var nonExistingId = 999;
    
    _mockBoardRepository.Setup(repo => repo.GetByIdAsync(nonExistingId))
        .ReturnsAsync((Board)null);

    // Act
    var result = await _boardController.GetById(nonExistingId);

    // Assert
    Assert.IsInstanceOfType(result, typeof(NotFoundResult));
}
```
