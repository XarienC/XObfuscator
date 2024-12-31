using dnlib.DotNet;
using System;

namespace XObfuscator.Methods.Renaming
{
    public static class RenameProtection
    {
        private static readonly Random random = new Random();

        // List of predefined names for renaming
        private static readonly List<string> AnimeNames = new List<string>
        {
            "Naruto", "Sasuke", "Sakura", "Itachi", "Goku", "Luffy",
            "Zoro", "Ichigo", "Levi", "Eren", "Mikasa", "Hinata",
            "Reigen", "Shinra", "Deku", "Bakugo", "Todoroki", "Gojo",
            "Ayanokoji", "Nagisa", "Kaname", "Kirito", "Asuna", "Rem"
        };

        private static readonly List<string> EncryptionNames = new List<string>
        {
            "AES256", "CryptoEngine", "SecureHash", "Obfuscator", "Encryptor",
            "RSA2048", "KeyGenerator", "HMAC", "CipherBlock", "HashFunction",
            "DigitalSignature", "TLSHandshake", "SHA512", "XORCipher"
        };

        private static readonly List<string> ObfuscationNames = new List<string>
        {
            "ObfuscatedProgram", "SecureApp", "CryptoEngine", "ProtectedBinary",
            "ShieldedAssembly", "HiddenPayload", "XORCipher", "DynamicModule",
            "VMRuntime", "StealthBuild", "SecuredApplication", "MaskedBinary"
        };

        // Main method to apply Rename Protection
        public static void Obfuscate(ModuleDefMD module)
        {
            // Rename the assembly and module metadata
            RenameAssembly(module.Assembly);

            foreach (var type in module.Types)
            {
                // Skip system/global types
                if (type.IsGlobalModuleType) continue;

                // Rename the namespace (declaring namespace for the type)
                if (!string.IsNullOrEmpty(type.Namespace))
                {
                    type.Namespace = GenerateRandomNamespaceName();
                }

                // Rename the type itself
                if (!IsCriticalName(type.Name))
                {
                    type.Name = GenerateRandomName();
                }

                foreach (var method in type.Methods)
                {
                    // Skip methods without a body, constructors, or the entry point
                    if (!method.HasBody || IsCriticalName(method.Name) || method.IsConstructor) continue;

                    // Rename the method
                    method.Name = GenerateRandomName();
                }

                foreach (var field in type.Fields)
                {
                    // Skip system-critical fields
                    if (IsCriticalName(field.Name)) continue;

                    // Rename fields
                    field.Name = GenerateRandomName();
                }
            }
        }

        // Renames the assembly and module metadata
        private static void RenameAssembly(AssemblyDef assembly)
        {
            // Choose a random name for the assembly
            string newAssemblyName = ObfuscationNames[random.Next(ObfuscationNames.Count)];

            // Rename the assembly
            assembly.Name = newAssemblyName;

            // Update the module name to match the new assembly name
            foreach (var module in assembly.Modules)
            {
                module.Name = $"{newAssemblyName}.exe"; // Adjust extension as needed
            }
        }

        // Determines if the name is critical and should not be renamed
        private static bool IsCriticalName(string name)
        {
            // Skip names like Main, .ctor, or static initializers
            return name == "Main" || name == ".ctor" || name.StartsWith("<");
        }

        // Generates a random name for renaming
        private static string GenerateRandomName()
        {
            int choice = random.Next(3); // 0 = Anime, 1 = Encryption, 2 = Random String

            switch (choice)
            {
                case 0: // Anime Name
                    return AnimeNames[random.Next(AnimeNames.Count)];
                case 1: // Encryption Name
                    return EncryptionNames[random.Next(EncryptionNames.Count)];
                default: // Random String
                    return GenerateRandomString(8);
            }
        }

        // Generates a random namespace name
        private static string GenerateRandomNamespaceName()
        {
            int choice = random.Next(2); // 0 = Obfuscation Name, 1 = Random String

            switch (choice)
            {
                case 0: // Obfuscation Name
                    return ObfuscationNames[random.Next(ObfuscationNames.Count)];
                default: // Random String
                    return GenerateRandomString(10);
            }
        }

        // Generates a random alphanumeric string
        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = chars[random.Next(chars.Length)];
            }
            return new string(buffer);
        }
    }
}
