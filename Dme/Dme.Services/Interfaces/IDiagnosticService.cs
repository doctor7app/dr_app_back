using Dme.Dtos.Diagnostics;
using Microsoft.AspNetCore.OData.Deltas;

namespace Dme.Services.Interfaces;

public interface IDiagnosticService
{
    /// <summary>
    /// Get Diagnostic for a specific consultation
    /// </summary>
    /// <param name="idDiagnostic"></param>
    /// <param name="idConsultation"></param>
    /// <returns></returns>
    Task<object> GetDiagnosticForConsultation(Guid idConsultation, Guid idDiagnostic);

    /// <summary>
    /// Get All Diagnostic for a consultation
    /// </summary>
    /// <param name="idConsultation"></param>
    /// <returns></returns>
    Task<IEnumerable<DiagnosticsReadDto>> GetAllDiagnosticForConsultation(Guid idConsultation);

    /// <summary>
    /// Create a Diagnostic for a specific Consultation
    /// </summary>
    /// <param name="idConsultation"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<object> CreateDiagnosticForConsultation(Guid idConsultation,DiagnosticsCreateDto entity);

    /// <summary>
    /// Update a specific Diagnostic for a specific consultation
    /// </summary>
    /// <param name="idDiagnostic"></param>
    /// <param name="entity"></param>
    /// <param name="idConsultation"></param>
    /// <returns></returns>
    Task<object> UpdateDiagnosticForConsultation(Guid idConsultation, Guid idDiagnostic, Delta<DiagnosticsUpdateDto> entity);

    /// <summary>
    /// Delete a specific diagnostic for a specific consultation
    /// </summary>
    /// <param name="idConsultation"></param>
    /// <param name="idDiagnostic"></param>
    /// <returns></returns>
    Task<object> DeleteDiagnosticForConsultation(Guid idConsultation, Guid idDiagnostic);
}