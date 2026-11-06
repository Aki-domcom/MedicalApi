using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    private static List<Spravka> _spravki = new List<Spravka>();

    [HttpGet]
    public List<Spravka> GetAll()
    {
        return _spravki;
    }

    [HttpGet("{id}")]
    public ActionResult<Spravka> GetById(int id)
    {
        Spravka? spravka = _spravki.FirstOrDefault(s => s.Id == id);
        if (spravka == null)
        {
            return NotFound();
        }
        return spravka;
    }

    [HttpPost]
    public ActionResult<Spravka> Create(Spravka spravka)
    {
        string? error = ValidateSpravka(spravka);
        if (error != null)
        {
            return BadRequest(error); // Вернет 400, если данные неверны или пациент не найден
        }

        spravka.Id = _spravki.Count + 1;
        _spravki.Add(spravka);
        return CreatedAtAction(nameof(GetById), new { id = spravka.Id }, spravka);
    }

    [HttpPut("{id}")]
    public ActionResult<Spravka> Update(int id, Spravka spravkaData)
    {
        Spravka? existingSpravka = _spravki.FirstOrDefault(s => s.Id == id);
        if (existingSpravka == null)
        {
            return NotFound();
        }

        string? error = ValidateSpravka(spravkaData);
        if (error != null)
        {
            return BadRequest(error);
        }

        existingSpravka.Name = spravkaData.Name;
        existingSpravka.PatientId = spravkaData.PatientId; // Обновляем PatientId (Задание 4)
        existingSpravka.Price = spravkaData.Price;

        return Ok(existingSpravka);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        Spravka? spravka = _spravki.FirstOrDefault(s => s.Id == id);
        if (spravka == null)
        {
            return NotFound();
        }

        _spravki.Remove(spravka);
        return NoContent();
    }

    // Метод проверки данных, обновленный под Задание 4
    private string? ValidateSpravka(Spravka spravka)
    {
        if (string.IsNullOrWhiteSpace(spravka.Name))
        {
            return "Название справки не может быть пустым";
        }
        if (spravka.Price <= 0)
        {
            return "Цена должна быть больше 0";
        }

        // Проверка из Задания 4: ищем, есть ли такой пациент в базе регистратуры
        bool patientExists = PatientsController.PatientsList.Any(p => p.Id == spravka.PatientId);
        if (!patientExists)
        {
            return $"Пациент с ID {spravka.PatientId} не найден в картотеке клиники. Сначала создайте пациента.";
        }

        return null; // Всё хорошо
    }
}
