using Microsoft.Win32; // Provides OpenFileDialog functionality
using System.Windows;  // For message boxes

namespace XObfuscator.Functions
{
    // Static class to handle file loading functionality
    public static class FileLoader
    {
        // Property to store the path of the selected file
        public static string SelectedFilePath { get; private set; } = string.Empty;

        // Method to open a file dialog and allow the user to select an executable or DLL file
        public static void LoadExecutable()
        {
            // Creates an OpenFileDialog for selecting .exe or .dll files
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Executable and DLL Files (*.exe;*.dll)|*.exe;*.dll", // Allow both .exe and .dll
                Title = "Select an Executable or DLL"
            };

            // Shows the dialog and checks if the user selected a file
            if (openFileDialog.ShowDialog() == true)
            {
                // Stores the selected file's path in the SelectedFilePath property
                SelectedFilePath = openFileDialog.FileName;

                // Notifies the user that the file was successfully loaded
                MessageBox.Show($"File loaded: {SelectedFilePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Warns the user if no file was selected
                MessageBox.Show("No file selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
