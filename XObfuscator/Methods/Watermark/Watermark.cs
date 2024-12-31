using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace XObfuscator.Methods.Watermark
{
    public static class Watermark
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

        // Adds the watermark to the obfuscated assembly
        public static void AddWatermark(ModuleDefMD module, string watermarkText)
        {
            if (string.IsNullOrWhiteSpace(watermarkText))
                return;

            // Replace spaces with dashes
            watermarkText = watermarkText.Replace(" ", "-");

            // Repeat the watermark namespace between 5 to 10 times
            int repeatCount = new Random().Next(5, 11);

            for (int i = 0; i < repeatCount; i++)
            {
                // Add watermark as a custom attribute with the watermark text
                AddWatermarkAttribute(module, $"{watermarkText}.{i}");

                // Embed the watermark as a hidden string with the watermark text as the namespace
                AddWatermarkHiddenString(module, $"{watermarkText}.{i}");
            }
        }

        // Adds a custom attribute with the watermark text to the assembly
        private static void AddWatermarkAttribute(ModuleDefMD module, string watermarkText)
        {
            // Create a new attribute type definition
            var watermarkAttributeType = new TypeDefUser(
                watermarkText, // Use the watermark text as the namespace
                GenerateRandomName().ToString(),
                module.CorLibTypes.GetTypeRef("System", "Attribute"));

            // Add the attribute type to the module
            module.Types.Add(watermarkAttributeType);

            // Create a field to store the watermark text
            var watermarkField = new FieldDefUser(
                "Watermark",
                new FieldSig(module.CorLibTypes.String),
                FieldAttributes.Private);
            watermarkAttributeType.Fields.Add(watermarkField);

            // Create the constructor for the attribute
            var constructor = new MethodDefUser(
                ".ctor",
                MethodSig.CreateInstance(module.CorLibTypes.Void, module.CorLibTypes.String),
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            constructor.Body = new CilBody();
            var baseConstructor = module.Import(typeof(object).GetConstructor(System.Type.EmptyTypes));

            // Add constructor instructions
            constructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0)); // Load 'this'
            constructor.Body.Instructions.Add(Instruction.Create(OpCodes.Call, baseConstructor)); // Call base constructor
            constructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0)); // Load 'this' again
            constructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1)); // Load string argument
            constructor.Body.Instructions.Add(Instruction.Create(OpCodes.Stfld, watermarkField)); // Store string in the field
            constructor.Body.Instructions.Add(Instruction.Create(OpCodes.Ret)); // Return
            watermarkAttributeType.Methods.Add(constructor);

            // Add the attribute to the assembly
            var attribute = new CustomAttribute(constructor);
            attribute.ConstructorArguments.Add(new CAArgument(module.CorLibTypes.String, watermarkText));
            module.CustomAttributes.Add(attribute);
        }

        // Adds the watermark as a hidden string in the assembly's code
        private static void AddWatermarkHiddenString(ModuleDefMD module, string watermarkText)
        {
            var type = new TypeDefUser(watermarkText, GenerateRandomName().ToString(), module.CorLibTypes.Object.TypeDefOrRef);
            module.Types.Add(type);

            var method = new MethodDefUser(
                GenerateRandomName().ToString(),
                MethodSig.CreateStatic(module.CorLibTypes.Void),
                MethodAttributes.Private | MethodAttributes.Static);

            var body = new CilBody();
            body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, watermarkText)); // Load the watermark text
            body.Instructions.Add(Instruction.Create(OpCodes.Pop)); // Pop it off the stack
            body.Instructions.Add(Instruction.Create(OpCodes.Ret)); // Return

            method.Body = body;
            type.Methods.Add(method);
        }

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
