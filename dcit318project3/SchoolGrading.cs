using System;
using System.Collections.Generic;
using System.IO;

class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

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

class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        List<Student> students = new List<Student>();
        using (StreamReader reader = new StreamReader(inputFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] fields = line.Split(',');
                if (fields.Length < 3)
                {
                    throw new MissingFieldException($"Incomplete record in file: {line}");
                }

                if (!int.TryParse(fields[0].Trim(), out int id))
                {
                    throw new InvalidScoreFormatException($"Invalid ID format in record: {line}");
                }

                string fullName = fields[1].Trim();
                if (string.IsNullOrEmpty(fullName))
                {
                    throw new MissingFieldException($"Missing name in record: {line}");
                }

                if (!int.TryParse(fields[2].Trim(), out int score))
                {
                    throw new InvalidScoreFormatException($"Invalid score format in record: {line}");
                }

                students.Add(new Student(id, fullName, score));
            }
        }
        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
            }
        }
    }
}

class Program
{
    static void Main()
    {
        StudentResultProcessor processor = new StudentResultProcessor();
        try
        {
            List<Student> students = processor.ReadStudentsFromFile("students.txt");
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
            Console.WriteLine($"Error: Missing field in record - {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}