using System;
using System.Collections.Generic;
using System.IO;

// Student class to hold student data and calculate grade
public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Score { get; set; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70 && Score <= 79) return "B";
        if (Score >= 60 && Score <= 69) return "C";
        if (Score >= 50 && Score <= 59) return "D";
        return "F";
    }
}

// Custom exception for invalid score format
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

// Custom exception for missing fields
public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// Class to process student results
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        List<Student> students = new List<Student>();
        
        using (StreamReader reader = new StreamReader(inputFilePath))
        {
            string line;
            int lineNumber = 0;
            
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                string[] fields = line.Split(',');

                // Validate number of fields
                if (fields.Length != 3)
                {
                    throw new MissingFieldException($"Line {lineNumber}: Missing or extra fields in input");
                }

                // Trim fields to handle extra whitespace
                string idStr = fields[0].Trim();
                string name = fields[1].Trim();
                string scoreStr = fields[2].Trim();

                // Validate non-empty fields
                if (string.IsNullOrEmpty(idStr) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(scoreStr))
                {
                    throw new MissingFieldException($"Line {lineNumber}: One or more fields are empty");
                }

                // Parse ID
                if (!int.TryParse(idStr, out int id))
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid ID format");
                }

                // Parse score
                if (!int.TryParse(scoreStr, out int score))
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format");
                }

                // Validate score range
                if (score < 0 || score > 100)
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Score out of valid range (0-100)");
                }

                students.Add(new Student(id, name, score));
            }
        }
        
        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            foreach (Student student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            StudentResultProcessor processor = new StudentResultProcessor();
            
            // Read students from input file
            List<Student> students = processor.ReadStudentsFromFile("students.txt");
            
            // Write report to output file
            processor.WriteReportToFile(students, "report.txt");
            
            Console.WriteLine("Report generated successfully.");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: Input file not found - {ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Error: Invalid score format - {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Error: Incomplete student record - {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}
