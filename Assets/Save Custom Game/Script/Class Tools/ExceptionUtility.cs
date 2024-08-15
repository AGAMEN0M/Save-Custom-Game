/*
 * ---------------------------------------------------------------------------
 * Description: This utility class provides a method to retrieve detailed information 
 *              about the calling method, including the file path and line number within 
 *              the project. The information is captured using a stack trace, and 
 *              the file path is adjusted to be relative to the project's Assets folder. 
 *              It is particularly useful for debugging and logging, offering insights 
 *              into where an error or method call originated.
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/
using System.Text.RegularExpressions;
using System.Diagnostics;

public static class ExceptionUtility
{
    public static string GetCallingMethodInfo()
    {
        var stackTrace = new StackTrace(true); // Create a stack trace to capture method call information.
        var frames = stackTrace.GetFrames(); // Get the stack frames from the stack trace.

        if (frames != null)
        {
            bool foundSaveDataUtility = false; // Flag to track if SaveDataUtility methods are encountered.

            // Iterate through each stack frame.
            foreach (var frame in frames)
            {
                // Get the method information from the stack frame.
                var method = frame.GetMethod();
                var declaringType = method?.DeclaringType;

                if (declaringType != null)
                {
                    var typeName = declaringType.FullName;

                    // Check if the method is not from SaveDataUtility or ExceptionUtility.
                    if (typeName != typeof(SaveDataUtility).FullName && typeName != typeof(ExceptionUtility).FullName)
                    {
                        // Retrieve file name and line number information from the stack frame.
                        var fileName = frame.GetFileName();
                        var lineNumber = frame.GetFileLineNumber();

                        // Ensure the file name and line number are valid.
                        if (!string.IsNullOrEmpty(fileName) && lineNumber > 0)
                        {
                            var filePath = Regex.Replace(fileName, @"^.*?Assets", "Assets"); // Modify file path to show it relative to the project's Assets folder.
                            return $"(at {filePath}:{lineNumber})"; // Format and return the method call's file path and line number.
                        }
                    }
                    else if (typeName == typeof(SaveDataUtility).FullName)
                    {
                        foundSaveDataUtility = true; // Flag that SaveDataUtility methods have been encountered.
                    }
                    else if (typeName == typeof(ExceptionUtility).FullName && foundSaveDataUtility)
                    {
                        break; // Stop processing when ExceptionUtility methods are encountered after SaveDataUtility.
                    }
                }
            }
        }

        return string.Empty; // Return an empty string if method call information couldn't be retrieved.
    }
}