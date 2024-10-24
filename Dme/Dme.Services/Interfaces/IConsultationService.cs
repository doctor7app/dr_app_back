using Dme.Dtos.Consultations;
using Microsoft.AspNetCore.OData.Deltas;

namespace Dme.Services.Interfaces;

public interface IConsultationService
{
    #region Consultation Dme EndPoints

    /// <summary>
    /// Get One Consultation related to a specific DME 
    /// </summary>
    /// <param name="idDme"></param>
    /// <param name="idConsultation"></param>
    /// <returns></returns>
    Task<object> GetConsultationForDme(Guid idDme, Guid idConsultation);

    /// <summary>
    /// Get All Consultation Related to a specific DME
    /// </summary>
    /// <param name="idDme"></param>
    /// <returns></returns>
    Task<IEnumerable<ConsultationsReadDto>> GetAllConsultationForDme(Guid idDme);

    /// <summary>
    /// Create a consultation for a specific DME
    /// </summary>
    /// <param name="idDme"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<object> CreateConsultationForDme(Guid idDme, ConsultationsCreateDto entity);

    /// <summary>
    /// Update a consultation for a specific DME
    /// </summary>
    /// <param name="idDme"></param>
    /// <param name="idConsultation"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<object> UpdateConsultationForDme(Guid idDme, Guid idConsultation, Delta<ConsultationsUpdateDto> entity);

    /// <summary>
    /// Delete one consultation related to a specific DME
    /// </summary>
    /// <param name="idDme"></param>
    /// <param name="idConsultation"></param>
    /// <returns></returns>
    Task<object> DeleteConsultationForDme(Guid idDme, Guid idConsultation);

    #endregion

    #region Consultation EndPoints

    /// <summary>
    /// Get Consultation By it's ID
    /// </summary>
    /// <param name="idConsultation"></param>
    /// <returns></returns>
    Task<object> GetConsultationById(Guid idConsultation);

    /// <summary>
    /// Get All Consultation
    /// </summary>
    /// <returns></returns>
    Task<object> GetAllConsultation();

    /// <summary>
    /// Create a consultation
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<object> CreateConsultation(ConsultationsCreateDto entity);

    /// <summary>
    /// Update Consultation By it's Id
    /// </summary>
    /// <param name="idConsultation"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<object> UpdateConsultationById(Guid idConsultation, Delta<ConsultationsUpdateDto> entity);

    /// <summary>
    /// Delete Consultation By it's Id
    /// </summary>
    /// <param name="idConsultation"></param>
    /// <returns></returns>
    Task<object> DeleteConsultationById(Guid idConsultation);

    #endregion



}