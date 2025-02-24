using Common.Enums.Prescriptions;
using Prescriptions.Domain.Abstract;
using Prescriptions.Domain.Events;

namespace Prescriptions.Domain.Models;

public class PrescriptionItem :AggregateRoot
{
    public Guid PrescriptionItemId { get; set; }
    
    public string DrugName { get; set; }
    public string Dosage { get; set; }
    public string Frequency { get; set; }

    /// <summary>
    /// Durée du traitement (ex: "7 jours", "1 mois")
    /// </summary>
    public string? Duration { get; set; }

    /// <summary>
    /// Instructions spécifiques (ex: "À prendre avec de la nourriture", "Éviter l'alcool")
    /// </summary>
    public string? Instructions { get; set; }

    /// <summary>
    /// Type du médicament (Antibiotique, Antalgique, etc.)
    /// </summary>
    public MedicationType MedicationType { get; set; }

    /// <summary>
    /// Indique si le médicament est essentiel pour le traitement
    /// </summary>
    public bool IsEssential { get; set; }

    /// <summary>
    /// Instructions spécifiques
    /// </summary>
    public AdministrationRoute Route { get; set; }

    /// <summary>
    /// Ex: "Matin", "Avant le coucher"
    /// </summary>
    public string? TimeOfDay { get; set; }

    /// <summary>
    /// Ex: "À jeun", "Pendant les repas"
    /// </summary>
    public string? MealInstructions { get; set; }

    /// <summary>
    /// Si le médicament est "au besoin" (ex: antidouleur)
    /// </summary>
    public bool IsPrn { get; set; }
    /// <summary>
    /// Ex: "Éviter en cas d'allergie aux AINS"
    /// </summary>
    public string?  Notes { get; set; }

    public Guid FkPrescriptionId { get; set; }
    public Prescription Prescription { get; set; }


    public void Update(PrescriptionItem updatedItem)
    {
        DrugName = updatedItem.DrugName;
        Dosage = updatedItem.Dosage;
        Frequency = updatedItem.Frequency;
        Duration = updatedItem.Duration;
        Instructions = updatedItem.Instructions;
        MedicationType = updatedItem.MedicationType;
        IsEssential = updatedItem.IsEssential;
        Route = updatedItem.Route;
        TimeOfDay = updatedItem.TimeOfDay;
        MealInstructions = updatedItem.MealInstructions;
        IsPrn = updatedItem.IsPrn;
        Notes = updatedItem.Notes;
    }


    public void UpdatePrescriptionItemEvent()
    {
        Update(this);
        AddEvent(new PrescriptionItemUpdatedEvent(PrescriptionItemId, DrugName, Dosage));
    }

    public void AddPrescriptionItemEvent()
    {
        AddEvent(new PrescriptionItemCreatedEvent(PrescriptionItemId, DrugName, Dosage));
    }

    public void RemovePrescriptionItemEvent()
    {
        AddEvent(new PrescriptionItemDeletedEvent(PrescriptionItemId));
    }

}