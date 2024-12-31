using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XObfuscator.Methods.Local2Field
{
    public static class Local2Field
    {
        private static Dictionary<Local, FieldDef> _convertedLocals = new Dictionary<Local, FieldDef>();

        public static void Obfuscate(ModuleDefMD module)
        {
            // Iterate through all types except the global type
            foreach (var type in module.Types.Where(t => t != module.GlobalType))
            {
                // Iterate through all methods with a body and instructions, excluding constructors
                foreach (var method in type.Methods.Where(m => m.HasBody && m.Body.HasInstructions && !m.IsConstructor))
                {
                    // Reset converted locals for each method
                    _convertedLocals = new Dictionary<Local, FieldDef>();

                    // Process the method
                    Process(module, method);
                }
            }
        }

        private static void Process(ModuleDef module, MethodDef method)
        {
            // Simplify macros for the method body
            method.Body.SimplifyMacros(method.Parameters);

            // Get the method's instructions
            var instructions = method.Body.Instructions;

            foreach (var instruction in instructions)
            {
                // Check if the instruction operand is a local variable
                if (!(instruction.Operand is Local local)) continue;

                FieldDef fieldDef;

                // If the local variable hasn't been converted, create a new field
                if (!_convertedLocals.ContainsKey(local))
                {
                    fieldDef = new FieldDefUser(
                        GenerateRandomName(),
                        new FieldSig(local.Type),
                        FieldAttributes.Public | FieldAttributes.Static);

                    // Add the new field to the global type
                    module.GlobalType.Fields.Add(fieldDef);

                    // Map the local variable to the field
                    _convertedLocals.Add(local, fieldDef);
                }
                else
                {
                    // Use the existing mapped field
                    fieldDef = _convertedLocals[local];
                }

                // Replace the instruction's opcode and operand
                switch (instruction.OpCode.Code)
                {
                    case Code.Ldloc:
                        instruction.OpCode = OpCodes.Ldsfld;
                        break;
                    case Code.Ldloca:
                        instruction.OpCode = OpCodes.Ldsflda;
                        break;
                    case Code.Stloc:
                        instruction.OpCode = OpCodes.Stsfld;
                        break;
                    default:
                        continue;
                }
                instruction.Operand = fieldDef;
            }

            // Remove all local variables that were converted
            _convertedLocals.ToList().ForEach(localToField => method.Body.Variables.Remove(localToField.Key));

            // Clear the converted locals map for the method
            _convertedLocals.Clear();
        }

        private static string GenerateRandomName()
        {
            // Generate a random string for the field name
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new System.Random();
            return new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
