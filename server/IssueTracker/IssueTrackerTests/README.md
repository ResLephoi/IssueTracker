# IssueTracker Unit Tests

This project contains unit tests for the IssueTracker application controllers. The tests use MSTest and Moq to mock dependencies and verify controller behavior.

## Tests Coverage

The tests cover:
1. All endpoints in controllers (success/failure scenarios)
2. Error handling in controllers
3. Validation of input data (using both regular and parameterized tests)

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
2. Build the solution
3. Open Test Explorer (Test > Test Explorer)
4. Run all tests or select specific tests to run

Alternatively, you can run the tests from the command line:

```powershell
# Navigate to the solution directory
cd c:\path\to\IssueTracker\server\IssueTracker

# Run all tests
dotnet test

# Run specific tests (e.g., only BoardControllerTests)
dotnet test --filter "FullyQualifiedName~BoardControllerTests"
```

## Test Types

1. **Controller Tests**: Test each controller endpoint for success and failure scenarios
2. **Error Handling Tests**: Verify that controllers properly handle exceptions
3. **Validation Tests**: Check that model validations work as expected
4. **Parameterized Validation Tests**: Data-driven tests that validate input data against expected results

## Test Structure

Each test follows the Arrange-Act-Assert pattern:
- **Arrange**: Set up the test dependencies, mocks, and expected values
- **Act**: Call the controller method being tested
- **Assert**: Verify the result matches expectations

## Best Practices Used in These Tests

1. **Mocking Dependencies**: Using Moq to create mock versions of services and repositories
2. **Isolating Units**: Testing controller methods in isolation from their dependencies
3. **Readable Test Names**: Using descriptive test names that explain what they're testing
4. **Testing Happy and Error Paths**: Testing both successful scenarios and error cases
5. **Consistent Setup**: Using TestInitialize to set up common test dependencies
6. **Checking Complete Behavior**: Verifying both the response type and content for each endpoint

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
