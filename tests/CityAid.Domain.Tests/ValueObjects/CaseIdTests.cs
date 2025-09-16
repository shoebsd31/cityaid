using CityAid.Domain.Enums;
using CityAid.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CityAid.Domain.Tests.ValueObjects;

[TestClass]
public class CaseIdTests
{
    [TestMethod]
    public void CaseId_ShouldCreate_WithCorrectFormat()
    {
        // Arrange
        var year = 2025;
        var cityCode = CityCode.Pune;
        var teamType = TeamType.Alpha;
        var caseNumber = 1;

        // Act
        var caseId = CaseId.Create(year, cityCode, teamType, caseNumber);

        // Assert
        Assert.AreEqual("CS-2025-PUN-AL-001", caseId.Value);
    }

    [TestMethod]
    public void CaseId_ShouldCreate_ForBetaTeam()
    {
        // Arrange
        var year = 2025;
        var cityCode = CityCode.Delhi;
        var teamType = TeamType.Beta;
        var caseNumber = 123;

        // Act
        var caseId = CaseId.Create(year, cityCode, teamType, caseNumber);

        // Assert
        Assert.AreEqual("CS-2025-DEL-BE-123", caseId.Value);
    }

    [TestMethod]
    public void CaseId_ShouldParseFromString_WhenValidFormat()
    {
        // Arrange
        var validCaseId = "CS-2025-PUN-AL-001";

        // Act
        var caseId = CaseId.FromString(validCaseId);

        // Assert
        Assert.AreEqual(validCaseId, caseId.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CaseId_ShouldThrowException_WhenInvalidFormat()
    {
        // Arrange
        var invalidCaseId = "INVALID-FORMAT";

        // Act & Assert
        CaseId.FromString(invalidCaseId);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CaseId_ShouldThrowException_WhenNullOrEmpty()
    {
        // Act & Assert
        CaseId.FromString(string.Empty);
    }

    [TestMethod]
    public void CaseId_ShouldImplicitlyConvertToString()
    {
        // Arrange
        var caseId = CaseId.Create(2025, CityCode.Pune, TeamType.Alpha, 1);

        // Act
        string stringValue = caseId;

        // Assert
        Assert.AreEqual("CS-2025-PUN-AL-001", stringValue);
    }
}