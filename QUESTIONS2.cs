using System;
using System.Collections.Generic;

public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item)
    {
        items.Add(item);
    }

    public List<T> GetAll()
    {
        return items;
    }

    public T? GetById(Func<T, bool> predicate)
    {
        return items.Find(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.Find(predicate);
        if (item != null)
        {
            return items.Remove(item);
        }
        return false;
    }
}

public class Patient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }
}

public class Prescription
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string MedicationName { get; set; }
    public DateTime DateIssued { get; set; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }
}

public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedData()
    {
        // Add patients
        _patientRepo.Add(new Patient(1, "John Doe", 30, "Male"));
        _patientRepo.Add(new Patient(2, "Jane Smith", 25, "Female"));
        _patientRepo.Add(new Patient(3, "Bob Johnson", 40, "Male"));

        // Add prescriptions
        _prescriptionRepo.Add(new Prescription(1, 1, "Aspirin", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(3, 2, "Paracetamol", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(4, 3, "Antibiotic", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(5, 2, "Vitamin C", DateTime.Now));
    }

    public void BuildPrescriptionMap()
    {
        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine($"Id: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
        }
    }

    public List<Prescription> GetPrescriptionsByPatientId(int patientId)
    {
        if (_prescriptionMap.TryGetValue(patientId, out var prescriptions))
        {
            return prescriptions;
        }
        return new List<Prescription>();
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        var prescriptions = GetPrescriptionsByPatientId(patientId);
        Console.WriteLine($"Prescriptions for Patient ID {patientId}:");
        foreach (var prescription in prescriptions)
        {
            Console.WriteLine($"Id: {prescription.Id}, Medication: {prescription.MedicationName}, Date Issued: {prescription.DateIssued}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        HealthSystemApp app = new HealthSystemApp();
        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();
        app.PrintPrescriptionsForPatient(1);
    }
}
