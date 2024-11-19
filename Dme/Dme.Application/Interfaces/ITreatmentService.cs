using Dme.Application.DTOs.Treatments;

namespace Dme.Application.Interfaces;

public interface ITreatmentService
{
    /// <summary>
    /// Get Treatment for a specific consultation
    /// </summary>
    /// <param name="idTreatment"></param>
    /// <param name="idConsultation"></param>
    /// <returns></returns>
    Task<object> GetTreatmentForConsultationById(Guid idConsultation,Guid idTreatment);

    /// <summary>
    /// Get All Treatment For a specific Consultation
    /// </summary>
    /// <param name="idConsultation"></param>
    /// <returns></returns>
    Task<IEnumerable<TreatmentsReadDto>> GetAllTreatmentForConsultationById(Guid idConsultation);

    /// <summary>
    /// Create a treatment for a specific Consultation
    /// </summary>
    /// <param name="idConsultation"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<object> CreateTreatmentForConsultation(Guid idConsultation,TreatmentsCreateDto entity);

    /// <summary>
    /// Update a treatment for a specific consultation
    /// </summary>
    /// <param name="idTreatment"></param>
    /// <param name="entity"></param>
    /// <param name="idConsultation"></param>
    /// <returns></returns>
    Task<object> PatchTreatmentForConsultation(Guid idConsultation,Guid idTreatment, TreatmentsPatchDto entity);

    /// <summary>
    /// Delete a treatment related to a specific consultation
    /// </summary>
    /// <param name="idConsultation"></param>
    /// <param name="idTreatment"></param>
    /// <returns></returns>
    Task<object> DeleteTreatmentForConsultation(Guid idConsultation, Guid idTreatment);
}