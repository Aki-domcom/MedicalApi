using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpravkiController : ControllerBase
{
    private static List<Spravka> _spravki = new List<Spravka>();

    // Идея 1: Тестовые данные (Пункт 7). Заполняем базу сразу при старте
    static SpravkiController()
    {
        _spravki.Add(new Spravka { Id = 1, Name = "Справка в бассейн", PatientId = 1, Price = 1500 });
        _spravki.Add(new Spravka { Id = 2, Name = "Больничный лист", PatientId = 1, Price = 0 });
        _spravki.Add(new Spravka { Id = 3, Name = "Водительская комиссия", PatientId = 2, Price = 5000 });
    }

    // Идея 2: Поиск и фильтрация через Query string (Пункт 7)
    // Доступно как обычный /api/spravki или с фильтром /api/spravki?patientId=1
    [HttpGet]
    public List<Spravka> GetAll(int? patientId)
    {
        // Если параметр patientId передан в адресе
        if (patientId != null)
        {
            // Отбираем и возвращаем только справки этого конкретного пациента
            return _spravki.Where(s => s.PatientId == patientId).ToList();
        }

        // Если параметра нет, возвращаем абсолютно все справки клиники
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
            return BadRequest(error);
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
        existingSpravka.PatientId = spravkaData.PatientId;
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

    private string? ValidateSpravka(Spravka spravka)
    {
        if (string.IsNullOrWhiteSpace(spravka.Name))
        {
            return "Название справки не может быть пустым";
        }
        if (spravka.Price < 0) // Разрешаем 0 для бесплатных больничных
        {
            return "Цена не может быть отрицательной";
        }

        // Проверяем существование пациента, только если в картотеке PatientsController уже кто-то есть
        // (чтобы наши заготовленные тестовые данные не ломали запуск, если список пациентов пуст)
        if (PatientsController.PatientsList.Any())
        {
            bool patientExists = PatientsController.PatientsList.Any(p => p.Id == spravka.PatientId);
            if (!patientExists)
            {
                return $"Пациент с ID {spravka.PatientId} не найден в картотеке клиники.";
            }
        }

        return null;
    }
}
