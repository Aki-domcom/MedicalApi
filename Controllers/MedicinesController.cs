using Microsoft.AspNetCore.Mvc;
using MedicalApi.Models;

namespace MedicalApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController : ControllerBase
{
    private static List<Medicine> _medicines = new List<Medicine>();

    // GET /api/medicines — получить список всех лекарств
    [HttpGet]
    public List<Medicine> GetAll()
    {
        return _medicines;
    }

    // GET /api/medicines/{id} — получить одно лекарство
    [HttpGet("{id}")]
    public ActionResult<Medicine> GetById(int id)
    {
        Medicine? medicine = _medicines.FirstOrDefault(m => m.Id == id);
        if (medicine == null)
        {
            return NotFound();
        }
        return medicine;
    }

    // POST /api/medicines — добавить лекарство в аптеку
    [HttpPost]
    public ActionResult<Medicine> Create(Medicine medicine)
    {
        string? error = ValidateMedicine(medicine);
        if (error != null)
        {
            return BadRequest(error); // Код 400 с понятным текстом
        }

        medicine.Id = _medicines.Count + 1;
        _medicines.Add(medicine);
        return CreatedAtAction(nameof(GetById), new { id = medicine.Id }, medicine); // Код 201
    }

    // PUT /api/medicines/{id} — изменить данные лекарства
    [HttpPut("{id}")]
    public ActionResult<Medicine> Update(int id, Medicine medicineData)
    {
        Medicine? existingMedicine = _medicines.FirstOrDefault(m => m.Id == id);
        if (existingMedicine == null)
        {
            return NotFound();
        }

        string? error = ValidateMedicine(medicineData);
        if (error != null)
        {
            return BadRequest(error);
        }

        existingMedicine.Name = medicineData.Name;
        existingMedicine.Quantity = medicineData.Quantity;
        existingMedicine.Price = medicineData.Price;

        return Ok(existingMedicine); // Код 200
    }

    // DELETE /api/medicines/{id} — удалить лекарство из аптеки
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        Medicine? medicine = _medicines.FirstOrDefault(m => m.Id == id);
        if (medicine == null)
        {
            return NotFound();
        }

        _medicines.Remove(medicine);
        return NoContent(); // Код 204
    }

    // Проверка данных (требование из задания 3)
    private string? ValidateMedicine(Medicine medicine)
    {
        if (string.IsNullOrWhiteSpace(medicine.Name))
        {
            return "Название лекарства не может быть пустым";
        }
        if (medicine.Quantity < 0)
        {
            return "Количество лекарства не может быть отрицательным";
        }
        if (medicine.Price <= 0)
        {
            return "Цена лекарства должна быть больше 0";
        }
        return null;
    }
}
