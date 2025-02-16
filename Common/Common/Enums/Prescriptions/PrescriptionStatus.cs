namespace Common.Enums.Prescriptions;

public enum PrescriptionStatus
{
    Draft,
    Validated,
    Sent,
    Canceled,
    Superseded // Prescription est changé et dois garder l'hisotirque
}