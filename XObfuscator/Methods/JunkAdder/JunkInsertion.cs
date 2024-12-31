using dnlib.DotNet.Emit;
using dnlib.DotNet;

namespace XObfuscator.Methods.JunkAdder
{
    public static class JunkInsertion
    {
        private static readonly Random random = new Random();

        // List of anime names for method and variable names
        private static readonly List<string> AnimeNames = new List<string>
        {
            "Naruto", "Sasuke", "Sakura", "Itachi", "Goku", "Luffy",
            "Zoro", "Ichigo", "Levi", "Eren", "Mikasa", "Hinata",
            "Reigen", "Shinra", "Deku", "Bakugo", "Todoroki", "Gojo",
            "Ayanokoji", "Nagisa", "Kaname", "Kirito", "Asuna", "Rem"
        };

        // List of junk namespace names
        private static readonly List<string> JunkNamespaceNames = new List<string>
        {
            "System.FakeRuntime",
            "Microsoft.SecurityToolkit",
            "Obfuscation.HiddenPayload",
            "Example.FakeUtilities",
            "Shielded.JunkLayer",
            "Crypto.FakeCrypto",
            "Debug.FakeDebugger",
            "Unreal.VirtualMachine",
            "Fake.NamespaceCollection"
        };

        // Main method to apply Junk Code Insertion
        public static void Obfuscate(ModuleDefMD module)
        {
            // Add junk namespaces
            AddJunkNamespaces(module);

            foreach (var type in module.Types)
            {
                // Skip global module types
                if (type.IsGlobalModuleType) continue;

                // Add a lot of useless void methods to the type
                AddUselessVoids(type, random.Next(10, 30)); // Add 10-30 junk methods to each type

                foreach (var method in type.Methods)
                {
                    // Skip methods without a body or very small methods
                    if (!method.HasBody || method.Body.Instructions.Count < 5) continue;

                    // Insert a lot of junk code into existing methods
                    InsertJunkCode(method, random.Next(10, 30)); // Insert 10-30 junk code blocks into each method
                }
            }
        }

        // Adds junk namespaces to the module
        private static void AddJunkNamespaces(ModuleDefMD module)
        {
            int numberOfJunkNamespaces = random.Next(5, 10); // Add 5-10 junk namespaces

            for (int i = 0; i < numberOfJunkNamespaces; i++)
            {
                string junkNamespaceName = JunkNamespaceNames[random.Next(JunkNamespaceNames.Count)];

                // Create a junk type within the junk namespace
                var junkType = new TypeDefUser(
                    junkNamespaceName,
                    GenerateRandomTypeName(),
                    module.CorLibTypes.Object.TypeDefOrRef);

                // Add the junk type to the module first
                module.Types.Add(junkType);

                // Now add meaningless methods to the junk type
                AddJunkMethods(junkType);
            }
        }

        // Adds a lot of useless void methods to the type
        private static void AddUselessVoids(TypeDef type, int numberOfVoids)
        {
            for (int i = 0; i < numberOfVoids; i++)
            {
                // Generate a fake method name using anime names
                string methodName = AnimeNames[random.Next(AnimeNames.Count)] + random.Next(1000, 9999);

                // Create a new void method
                var method = new MethodDefUser(
                    methodName,
                    MethodSig.CreateStatic(type.Module.CorLibTypes.Void),
                    MethodAttributes.Public | MethodAttributes.Static);

                // Add a lot of junk instructions to the method
                var body = new CilBody();
                for (int j = 0; j < random.Next(10, 20); j++) // Add 10-20 junk instructions per method
                {
                    body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, GenerateRandomString(10))); // Load random string
                    body.Instructions.Add(Instruction.Create(OpCodes.Pop)); // Pop it off the stack
                    body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, random.Next(1, 100))); // Load random int
                    body.Instructions.Add(Instruction.Create(OpCodes.Pop)); // Pop it off the stack
                }
                body.Instructions.Add(Instruction.Create(OpCodes.Ret)); // Return
                method.Body = body;

                // Add the method to the type
                type.Methods.Add(method);
            }
        }

        // Adds junk methods to a given type
        private static void AddJunkMethods(TypeDef junkType)
        {
            if (junkType.Module == null)
            {
                throw new InvalidOperationException("The junk type must be added to the module before creating methods.");
            }

            int numberOfMethods = random.Next(3, 6); // Add 3-6 junk methods per type

            for (int i = 0; i < numberOfMethods; i++)
            {
                // Generate a fake method name
                string methodName = GenerateRandomMethodName();

                // Create a junk method
                var junkMethod = new MethodDefUser(
                    methodName,
                    MethodSig.CreateStatic(junkType.Module.CorLibTypes.Void),
                    MethodAttributes.Public | MethodAttributes.Static);

                // Add a basic junk body to the method
                var body = new CilBody();
                body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, random.Next(0, 100))); // Push a random int
                body.Instructions.Add(Instruction.Create(OpCodes.Pop)); // Remove it from the stack
                body.Instructions.Add(Instruction.Create(OpCodes.Ldstr, GenerateRandomString(10))); // Load a random string
                body.Instructions.Add(Instruction.Create(OpCodes.Pop)); // Remove it from the stack
                body.Instructions.Add(Instruction.Create(OpCodes.Ret)); // Return

                junkMethod.Body = body;

                // Add the junk method to the junk type
                junkType.Methods.Add(junkMethod);
            }
        }

        // Inserts junk code into the method
        private static void InsertJunkCode(MethodDef method, int junkCodeBlocks)
        {
            var instructions = method.Body.Instructions;

            for (int i = 0; i < junkCodeBlocks; i++)
            {
                // Generate a junk code block
                var junkCode = GenerateJunkCode();

                // Insert the junk code block at a random position in the method
                int insertionIndex = random.Next(0, instructions.Count);
                foreach (var instr in junkCode)
                {
                    instructions.Insert(insertionIndex, instr);
                    insertionIndex++; // Move the index to avoid overwriting the inserted instructions
                }
            }

            // Simplify and optimize the body
            method.Body.SimplifyBranches();
            method.Body.OptimizeBranches();
        }

        // Generates a random block of junk code
        private static Instruction[] GenerateJunkCode()
        {
            int junkCodeLength = random.Next(3, 6); // Junk code block length
            var junkCode = new List<Instruction>();

            for (int i = 0; i < junkCodeLength; i++)
            {
                junkCode.Add(Instruction.Create(OpCodes.Ldstr, GenerateRandomString(10))); // Load random string
                junkCode.Add(Instruction.Create(OpCodes.Pop));                            // Remove it from the stack
                junkCode.Add(Instruction.Create(OpCodes.Ldc_I4, random.Next(1, 100)));    // Load a random integer
                junkCode.Add(Instruction.Create(OpCodes.Pop));                            // Remove it from the stack
            }

            return junkCode.ToArray();
        }

        // Generates a random type name
        private static string GenerateRandomTypeName()
        {
            return "Type" + GenerateRandomString(6);
        }

        // Generates a random method name
        private static string GenerateRandomMethodName()
        {
            return "Method" + GenerateRandomString(6);
        }

        // Generates a random string of letters
        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            char[] buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = chars[random.Next(chars.Length)];
            }
            return new string(buffer);
        }
    }
}
