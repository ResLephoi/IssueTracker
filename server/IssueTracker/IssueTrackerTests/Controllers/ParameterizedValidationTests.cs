using Domain.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IssueTrackerTests.Controllers;

[TestClass]
public class ParameterizedValidationTests
{
    [TestMethod]
    [DataRow("", false, "Title is required")]
    [DataRow("A", true, null)]
    [DataRow("This is a valid title", true, null)]
    [DataRow("This is a very long title that exceeds 100 characters This is a very long title that exceeds 100 characters This is a very long", false, "Title must be between 1 and 100 characters")]
    public void CreateBoardDTO_TitleValidation(string title, bool expectedIsValid, string expectedErrorMessageContains)
    {
        // Arrange
        var createBoardDto = new CreateBoardDTO { Title = title };
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(createBoardDto);
        
        // Act
        bool isValid = Validator.TryValidateObject(createBoardDto, validationContext, validationResults, true);
        
        // Assert
        Assert.AreEqual(expectedIsValid, isValid);
        
        if (!expectedIsValid)
        {
            Assert.IsTrue(validationResults.Any(vr => vr.ErrorMessage != null && vr.ErrorMessage.Contains(expectedErrorMessageContains)));
        }
    }

    [TestMethod]
    [DataRow("", 1, false, "Title")]
    [DataRow("Valid Title", 0, false, "Item ID")]
    [DataRow("Valid Title", 1, true, null)]
    [DataRow("This is a very long title that exceeds 200 characters This is a very long title that exceeds 200 characters This is a very long title that exceeds 200 characters This is a very long title that exceeds 200 characters This is a very long title that exceeds 200 characters", 1, false, "Title")]
    public void CreateCardDTO_Validation(string title, int itemId, bool expectedIsValid, string expectedErrorMessageContains)
    {
        // Arrange
        var createCardDto = new CreateCardDTO { Title = title, ItemId = itemId, Description = "Description" };
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(createCardDto);
        
        // Act
        bool isValid = Validator.TryValidateObject(createCardDto, validationContext, validationResults, true);
        
        // Assert
        Assert.AreEqual(expectedIsValid, isValid);
        
        if (!expectedIsValid && expectedErrorMessageContains != null)
        {
            Assert.IsTrue(validationResults.Any(vr => vr.ErrorMessage != null && vr.ErrorMessage.Contains(expectedErrorMessageContains)));
        }
    }

    [TestMethod]
    [DataRow("", 1, false, "Title")]
    [DataRow("Valid Title", 0, false, "Board ID")]
    [DataRow("Valid Title", 1, true, null)]
    [DataRow("This is a very long title that exceeds 150 characters This is a very long title that exceeds 150 characters This is a very long title that exceeds 150 characters This is a very", 1, false, "Title")]
    public void CreateItemDTO_Validation(string title, int boardId, bool expectedIsValid, string expectedErrorMessageContains)
    {
        // Arrange
        var createItemDto = new CreateItemDTO { Title = title, BoardId = boardId };
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(createItemDto);
        
        // Act
        bool isValid = Validator.TryValidateObject(createItemDto, validationContext, validationResults, true);
        
        // Assert
        Assert.AreEqual(expectedIsValid, isValid);
        
        if (!expectedIsValid && expectedErrorMessageContains != null)
        {
            Assert.IsTrue(validationResults.Any(vr => vr.ErrorMessage != null && vr.ErrorMessage.Contains(expectedErrorMessageContains)));
        }
    }
}
