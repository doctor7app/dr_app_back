using Common.Extension.Common;
using Dme.Domain.Models;

namespace Dme.UnitTest.Models;

public class DiagnosticsTests
{
    [Fact]
    public void Diagnostics_Constructor_SetsProperties()
    {
        // Arrange
        var diagnostic = new Diagnostics
        {
            DiagnosticId = Guid.NewGuid(),
            TypeDiagnostic = "Radiology",
            Description = "X-ray of the chest",
            Results = "Normal",
            Comments = "Follow-up in 6 months"
        };

        // Act

        // Assert
        Assert.NotNull(diagnostic);
        Assert.Equal("Radiology", diagnostic.TypeDiagnostic);
        Assert.Equal("X-ray of the chest", diagnostic.Description);
    }

    [Fact]
    public void Diagnostics_Equality_ChecksEqualObjects()
    {
        // Arrange
        var diagnostic1 = new Diagnostics { DiagnosticId = Guid.NewGuid(), TypeDiagnostic = "Radiology" };
        var diagnostic2 = new Diagnostics { DiagnosticId = diagnostic1.DiagnosticId, TypeDiagnostic = "Radiology" };

        // Act

        // Assert
        Assert.True(diagnostic1.AreEqual(diagnostic2)); // Assuming AreEqual is an extension method
    }
}