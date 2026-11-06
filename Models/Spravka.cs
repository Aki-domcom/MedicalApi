namespace MedicalApi.Models;

public class Spravka
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int PatientId { get; set; } // Связь с ID пациента из картотеки (Задание 4)
    public decimal Price { get; set; }
}