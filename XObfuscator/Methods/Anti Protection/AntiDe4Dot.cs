using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XObfuscator.Methods.AntiProtection
{

    // credit to nak0823

    public static class AntiDe4Dot
    {
        public static void Obfuscate(ModuleDefMD module)
        {
            // Create an invalid interface implementation
            InterfaceImplUser invalidImpl = new InterfaceImplUser(module.GlobalType);

            // Add invalid interface implementations to crash De4dot
            for (int i = 100; i < 150; i++)
            {
                // Create an invalid type definition
                TypeDefUser invalidType = new TypeDefUser(
                    "", // No namespace
                    GenerateRandomName(), // Generate a random type name
                    module.CorLibTypes.GetTypeRef("System", "Attribute") // Base type
                );

                // Add invalid interface implementations to the type
                InterfaceImplUser typeImpl = new InterfaceImplUser(invalidType);
                invalidType.Interfaces.Add(typeImpl);
                invalidType.Interfaces.Add(invalidImpl);

                // Add the invalid type to the module
                module.Types.Add(invalidType);
            }
        }

        private static string GenerateRandomName()
        {
            // Generate a random string for the invalid type name
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new System.Random();
            char[] name = new char[8]; // Length of 8 characters
            for (int i = 0; i < name.Length; i++)
            {
                name[i] = chars[random.Next(chars.Length)];
            }
            return new string(name);
        }
    }
}
