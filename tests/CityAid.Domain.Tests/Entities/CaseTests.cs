using CityAid.Domain.Entities;
using CityAid.Domain.Enums;
using CityAid.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CityAid.Domain.Tests.Entities;

[TestClass]
public class CaseTests
{
    [TestMethod]
    public void Case_ShouldCreate_WithValidParameters()
    {
        // Arrange
        var caseId = CaseId.Create(2025, CityCode.Pune, TeamType.Alpha, 1);
        var cityCode = CityCode.Pune;
        var teamType = TeamType.Alpha;
        var title = "Test Case";
        var description = "Test Description";
        var createdBy = "test-user";

        // Act
        var @case = new Case(caseId, cityCode, teamType, title, description, createdBy);

        // Assert
        Assert.AreEqual(caseId, @case.Id);
        Assert.AreEqual(cityCode, @case.CityCode);
        Assert.AreEqual(teamType, @case.TeamType);
        Assert.AreEqual(CaseState.Initiated, @case.State);
        Assert.AreEqual(title, @case.Title);
        Assert.AreEqual(description, @case.Description);
        Assert.AreEqual(createdBy, @case.CreatedBy);
        Assert.IsTrue(@case.DomainEvents.Any());
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Case_ShouldThrowException_WhenCreatedByFinanceTeam()
    {
        // Arrange
        var caseId = CaseId.Create(2025, CityCode.Pune, TeamType.Finance, 1);
        var cityCode = CityCode.Pune;
        var teamType = TeamType.Finance;
        var title = "Test Case";
        var createdBy = "test-user";

        // Act & Assert
        var @case = new Case(caseId, cityCode, teamType, title, null, createdBy);
    }

    [TestMethod]
    public void Case_ShouldSubmitToFinance_WhenInAnalysisState()
    {
        // Arrange
        var @case = CreateTestCase();
        @case.SubmitForAnalysis("test-user");

        // Act
        @case.SubmitToFinance("test-user");

        // Assert
        Assert.AreEqual(CaseState.Pending_Finance, @case.State);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Case_ShouldThrowException_WhenSubmittingToFinanceFromWrongState()
    {
        // Arrange
        var @case = CreateTestCase();

        // Act & Assert
        @case.SubmitToFinance("test-user");
    }

    [TestMethod]
    public void Case_ShouldApproveByFinance_WhenInPendingFinanceState()
    {
        // Arrange
        var @case = CreateTestCase();
        @case.SubmitForAnalysis("test-user");
        @case.SubmitToFinance("test-user");

        // Act
        @case.ApproveByFinance("finance-user");

        // Assert
        Assert.AreEqual(CaseState.Pending_PMO, @case.State);
    }

    [TestMethod]
    public void Case_ShouldEnforceRBAC_CorrectlyForAlphaTeam()
    {
        // Arrange
        var @case = CreateTestCase();

        // Act & Assert
        Assert.IsTrue(@case.CanBeViewedByUser(CityCode.Pune, TeamType.Alpha));
        Assert.IsFalse(@case.CanBeViewedByUser(CityCode.Pune, TeamType.Beta));
        Assert.IsTrue(@case.CanBeViewedByUser(CityCode.Pune, TeamType.Finance));
        Assert.IsTrue(@case.CanBeViewedByUser(CityCode.Pune, TeamType.PMO));
        Assert.IsFalse(@case.CanBeViewedByUser(CityCode.Delhi, TeamType.Alpha));
    }

    [TestMethod]
    public void Case_ShouldRetriggerByPMO_WhenRejected()
    {
        // Arrange
        var @case = CreateTestCase();
        @case.SubmitForAnalysis("test-user");
        @case.SubmitToFinance("test-user");
        @case.RejectByFinance("finance-user", "Budget too high");

        // Act
        @case.RetriggerByPMO("pmo-user");

        // Assert
        Assert.AreEqual(CaseState.Pending_Finance, @case.State);
    }

    private static Case CreateTestCase()
    {
        var caseId = CaseId.Create(2025, CityCode.Pune, TeamType.Alpha, 1);
        return new Case(caseId, CityCode.Pune, TeamType.Alpha, "Test Case", "Test Description", "test-user");
    }
}