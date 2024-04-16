using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RetrieveWindowsProductKey
{
    class RegistryExtractor
    {
        public static void Search()
        {
            Console.WriteLine("Starting registry search for product key...");

            RegistryKey[] rootKeys = new RegistryKey[]
            {
                Registry.ClassesRoot,
                Registry.CurrentUser,
                Registry.LocalMachine,
                Registry.Users,
                Registry.CurrentConfig
            };

            // Iterate through each root key
            foreach (RegistryKey rootKey in rootKeys)
            {
                // Recursively backup the subkeys
                SearchRegistryKey(rootKey);
            }

            Console.WriteLine("Registry search complete.");
        }

        static void SearchRegistryKey(RegistryKey key)
        {
            // Get the subkey names
            string[] subKeyNames = key.GetSubKeyNames();

            // Iterate through each subkey
            foreach (string subKeyName in subKeyNames)
            {
                try
                {
                    // Open the subkey
                    using (RegistryKey subKey = key.OpenSubKey(subKeyName))
                    {
                        if (subKey != null)
                        {

                            // Get the value names
                            string[] valueNames = subKey.GetValueNames();

                            // Write each value to the backup file
                            foreach (string valueName in valueNames)
                            {
                                var value = subKey.GetValue(valueName);

                                if (value != null)
                                {
                                    var str = value.ToString();
                                    if (str != null && IsProductKeyFormat(str))
                                    {
                                        Console.WriteLine("FOUND POSSIBLE PRODUCT KEY: " + str);
                                        Console.WriteLine("Location: " + subKey.ToString());
                                        Console.Write("\n");
                                        return;
                                    }
                                }
                            }

                            // Recursively backup subkeys
                            SearchRegistryKey(subKey);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"Error backing up subkey '{key.Name}\\{subKeyName}': {ex.Message}");
                }
            }
        }

        private static bool IsProductKeyFormat(string input)
        {
            string pattern = @"^[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}$";

            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

    }
}
