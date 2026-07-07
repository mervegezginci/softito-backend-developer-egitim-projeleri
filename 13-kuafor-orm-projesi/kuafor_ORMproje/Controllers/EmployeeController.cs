using kuafor_ORMproje.Data.Repository.IRepository;
using kuafor_ORMproje.Model;
using Microsoft.AspNetCore.Mvc;
using System;

namespace kuafor_ORMproje.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Employee
        public IActionResult Index()
        {
            var employees = _unitOfWork.Employee.GetAll();
            return View(employees);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,FullName,Speciality,ImageUrl,Phone,IsActive")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(employee.ImageUrl))
                {
                    employee.ImageUrl = "/img/default-employee.jpg";
                }
                _unitOfWork.Employee.Add(employee);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Çalışan başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employee/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var employee = _unitOfWork.Employee.GetFirstOrDefault(u => u.Id == id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,FullName,Speciality,ImageUrl,Phone,IsActive")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(employee.ImageUrl))
                {
                    employee.ImageUrl = "/img/default-employee.jpg";
                }
                _unitOfWork.Employee.Update(employee);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Çalışan bilgileri güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var employee = _unitOfWork.Employee.GetFirstOrDefault(u => u.Id == id);
            if (employee != null)
            {
                _unitOfWork.Employee.Remove(employee);
                _unitOfWork.Save();
                TempData["SuccessMessage"] = "Çalışan başarıyla silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
