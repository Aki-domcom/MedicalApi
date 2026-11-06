namespace MedicalApi.Models;

public class Patient
{
    // Уникальный номер пациента в базе данных (заполняется автоматически)
    public int Id { get; set; }

    // Имя пациента
    public string Name { get; set; } = "";

    // Фамилия пациента
    public string Surname { get; set; } = "";

    // Номер телефона для связи
    public string PhoneNumber { get; set; } = "";

    // Дата рождения (используем специальный тип DateTime)
    public DateTime BirthDate { get; set; }

    // Есть ли у пациента медицинская страховка (тип bool: либо true/да, либо false/нет)
    public bool HasInsurance { get; set; }
}
