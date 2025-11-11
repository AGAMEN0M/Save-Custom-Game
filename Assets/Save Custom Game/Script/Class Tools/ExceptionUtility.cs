/*
 * ---------------------------------------------------------------------------
 * Description: Utility class for retrieving method call information for debugging. 
 *              It uses a stack trace to extract the file path and line number from 
 *              where a method was called, excluding internal utility frames. 
 *              This is especially useful for logging the exact source of errors 
 *              or events in the SaveCustomGame system.
 *              
 * Author: Lucas Gomes Cecchini
 * Pseudonym: AGAMENOM
 * ---------------------------------------------------------------------------
*/

using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SaveCustomGame
{
    public static class ExceptionUtility
    {
        #region === Public Methods ===

        /// <summary>
        /// Retrieves the file path and line number of the external method that invoked this utility.
        /// Skips internal frames from SaveDataUtility and ExceptionUtility to return the most relevant call source.
        /// Useful for pinpointing where events or errors originated in the debug logs.
        /// </summary>
        public static string GetCallingMethodInfo()
        {
            // Create a stack trace containing method call information, including file and line number.
            var stackTrace = new StackTrace(true);

            // Retrieve the stack frames from the created stack trace.
            var frames = stackTrace.GetFrames();

            // Ensure stack frames exist before processing.
            if (frames != null)
            {
                // This flag tracks whether a SaveDataUtility method was encountered.
                bool foundSaveDataUtility = false;

                // Iterate through the stack frames to find the relevant external caller.
                foreach (var frame in frames)
                {
                    // Obtain the method associated with this stack frame.
                    var method = frame.GetMethod();
                    var declaringType = method?.DeclaringType;

                    // Ensure the declaring type is valid before reading its information.
                    if (declaringType != null)
                    {
                        // Full name of the declaring type for comparison.
                        var typeName = declaringType.FullName;

                        // Check if this frame belongs to neither SaveDataUtility nor ExceptionUtility.
                        if (typeName != typeof(SaveDataUtility).FullName && typeName != typeof(ExceptionUtility).FullName)
                        {
                            // Extract source file name and line number.
                            var fileName = frame.GetFileName();
                            var lineNumber = frame.GetFileLineNumber();

                            // Validate the extracted file information.
                            if (!string.IsNullOrEmpty(fileName) && lineNumber > 0)
                            {
                                // Convert full system file path to a Unity project relative path.
                                var filePath = Regex.Replace(fileName, @"^.*?Assets", "Assets");

                                // Return formatted call site information.
                                return $"(at {filePath}:{lineNumber})";
                            }
                        }
                        else if (typeName == typeof(SaveDataUtility).FullName)
                        {
                            // Mark that a SaveDataUtility call was detected.
                            foundSaveDataUtility = true;
                        }
                        else if (typeName == typeof(ExceptionUtility).FullName && foundSaveDataUtility)
                        {
                            // Stop scanning after returning from SaveDataUtility into ExceptionUtility.
                            break;
                        }
                    }
                }
            }

            // Return empty string if no suitable call information was located.
            return string.Empty;
        }

        #endregion
    }
}