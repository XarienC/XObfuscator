using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XObfuscator.Methods.FieldEncapsulation
{
    public static class FieldEncap
    {
        public static void Obfuscate(ModuleDefMD module)
        {
            foreach (var type in module.Types)
            {
                // Skip global types
                if (type.IsGlobalModuleType) continue;

                // Copy fields to a separate list to avoid modifying the original list while iterating
                var fieldsToProcess = new List<FieldDef>(type.Fields);

                foreach (var field in fieldsToProcess)
                {
                    // Skip static fields, special fields, and literals
                    if (field.IsStatic || field.IsLiteral || field.IsSpecialName) continue;

                    // Encapsulate the field
                    EncapsulateField(type, field);
                }
            }
        }

        private static void EncapsulateField(TypeDef type, FieldDef field)
        {
            // Create the backing field (e.g., _fieldName)
            var backingField = new FieldDefUser(
                $"_{field.Name}",
                new FieldSig(field.FieldSig.Type),
                field.Attributes & ~FieldAttributes.Public | FieldAttributes.Private);
            type.Fields.Add(backingField);

            // Update all references to the original field
            UpdateFieldReferences((ModuleDefMD)type.Module, field, backingField);

            // Create the getter method
            var getterMethod = new MethodDefUser(
                $"Get{field.Name.String.ToPascalCase()}",
                MethodSig.CreateInstance(field.FieldSig.Type),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName);
            var getterBody = new CilBody();
            getterBody.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0)); // Load "this"
            getterBody.Instructions.Add(Instruction.Create(OpCodes.Ldfld, backingField)); // Load field value
            getterBody.Instructions.Add(Instruction.Create(OpCodes.Ret)); // Return value
            getterMethod.Body = getterBody;
            type.Methods.Add(getterMethod);

            // Create the setter method
            var setterMethod = new MethodDefUser(
                $"Set{field.Name.String.ToPascalCase()}",
                MethodSig.CreateInstance(type.Module.CorLibTypes.Void, field.FieldSig.Type),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName);
            var setterBody = new CilBody();
            setterBody.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0)); // Load "this"
            setterBody.Instructions.Add(Instruction.Create(OpCodes.Ldarg_1)); // Load method argument
            setterBody.Instructions.Add(Instruction.Create(OpCodes.Stfld, backingField)); // Store in field
            setterBody.Instructions.Add(Instruction.Create(OpCodes.Ret)); // Return
            setterMethod.Body = setterBody;
            type.Methods.Add(setterMethod);

            // Remove the original field
            type.Fields.Remove(field);
        }

        private static void UpdateFieldReferences(ModuleDefMD module, FieldDef originalField, FieldDef backingField)
        {
            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;

                    var instructions = method.Body.Instructions;
                    foreach (var instr in instructions)
                    {
                        // Update field references
                        if (instr.Operand is FieldDef field && field == originalField)
                        {
                            instr.Operand = backingField;
                        }
                    }
                }
            }
        }
    }

    // Helper extension method for PascalCase conversion
    public static class StringExtensions
    {
        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return char.ToUpperInvariant(str[0]) + str.Substring(1);
        }
    }
}
