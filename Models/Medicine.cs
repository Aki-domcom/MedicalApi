namespace MedicalApi.Models;

public class Medicine
{
    public int Id { get; set; }
    public string Name { get; set; } = ""; // Название лекарства
    public int Quantity { get; set; } // Количество на складе аптеки
    public decimal Price { get; set; } // Цена за упаковку
}