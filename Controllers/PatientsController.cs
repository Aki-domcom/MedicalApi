using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models; // Подключаем нашу папку с моделями, чтобы контроллер видел класс Patient

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")] // Автоматически превратит адрес в /api/patients
public class PatientsController : ControllerBase
{
    // Временная база данных в оперативной памяти для хранения списка пациентов
    // Делаем список public static, чтобы к нему потом можно было достучаться из других мест
    public static List<Patient> PatientsList = new List<Patient>();

    // 1. GET /api/patients — Возвращает список всех пациентов
    [HttpGet]
    public List<Patient> GetAll()
    {
        return PatientsList;
    }

    // 2. GET /api/patients/{id} — Возвращает одного пациента по его ID. Если такого нет — возвращает 404 (NotFound)
    [HttpGet("{id}")]
    public ActionResult<Patient> GetById(int id)
    {
        Patient? patient = PatientsList.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound(); // Возвращает статус-код 404
        }
        return patient; // Возвращает статус-код 200 и данные пациента
    }

    // 3. POST /api/patients — Создает нового пациента и возвращает статус 201 (Created)
    [HttpPost]
    public ActionResult<Patient> Create(Patient patient)
    {
        string? error = ValidatePatient(patient);
        if (error != null) return BadRequest(error);
        // Автоматически рассчитываем новый ID на основе количества элементов в списке
        patient.Id = PatientsList.Count + 1;
        
        // Добавляем заполненную карточку пациента в наш список
        PatientsList.Add(patient);
        
        // Возвращаем статус 201 Created и ссылку на то, как получить этого пациента по ID
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }
    // 4. PUT /api/patients/{id} — Редактирование карточки пациента
    [HttpPut("{id}")]
    public ActionResult<Patient> Update(int id, Patient patientData)
    {
        // Ищем пациента в списке по ID
        Patient? existingPatient = PatientsList.FirstOrDefault(p => p.Id == id);
        if (existingPatient == null)
        {
            // Если пациента с таким номером нет — возвращаем 404
            return NotFound(); 
        }
        string? error = ValidatePatient(patientData);
        if (error != null) return BadRequest(error);

        // Переписываем все поля из пришедшего запроса (кроме самого Id)
        existingPatient.Name = patientData.Name;
        existingPatient.Surname = patientData.Surname;
        existingPatient.PhoneNumber = patientData.PhoneNumber;
        existingPatient.BirthDate = patientData.BirthDate;
        existingPatient.HasInsurance = patientData.HasInsurance;

        // Возвращаем измененную карточку обратно на фронтенд со статусом 200 OK
        return Ok(existingPatient); 
    }

    // 5. DELETE /api/patients/{id} — Удаление пациента из картотеки
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        // Ищем пациента по ID
        Patient? patient = PatientsList.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            // Если удалять некого — возвращаем 404
            return NotFound(); 
        }

        // Стираем запись из нашего статического списка PatientsList
        PatientsList.Remove(patient);

        // Возвращаем код 204 No Content (все прошло успешно, ответа в теле нет)
        return NoContent(); 
    }
    // Приватный метод для проверки данных пациента (Задание 3)
    private string? ValidatePatient(Patient patient)
    {
        if (string.IsNullOrWhiteSpace(patient.Name))
        {
            return "Имя пациента не может быть пустым";
        }
        if (string.IsNullOrWhiteSpace(patient.Surname))
        {
            return "Фамилия пациента не может быть пустой";
        }
        if (patient.BirthDate > DateTime.Now)
        {
            return "Дата рождения не может быть в будущем";
        }
        return null; // Ошибок нет
    }

}