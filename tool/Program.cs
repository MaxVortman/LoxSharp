using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AstGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: generate_ast <output directory>");
                Environment.Exit(exitCode: 1);
            }
            var outputDir = args[0];
            DefineAst(outputDir, "Expr", new List<string>(){
                "Binary   : Expr left, Token op, Expr right",
                "Grouping : Expr expression",
                "Literal  : Object value",
                "Unary    : Token op, Expr right"
            });
        }

        private static void DefineAst(string outputDir, string baseName, List<string> types)
        {
            var path = outputDir + "/" + baseName + ".cs";
            using (var file = File.Open(path, FileMode.Create))
            {
                using (var writer = new StreamWriter(file, Encoding.UTF8))
                {
                    writer.WriteLine("namespace Lox_");
                    writer.WriteLine("{");
                    writer.WriteLine("abstract class " + baseName);
                    writer.WriteLine("{");

                    DefineVisitor(writer, baseName, types);

                    // The AST classes.                                     
                    foreach (var type in types)
                    {
                        var typeSplits = type.Split(":");
                        var className = typeSplits[0].Trim();
                        var fields = typeSplits[1].Trim();
                        DefineType(writer, baseName, className, fields);
                    }

                    // The base accept() method.                                   
                    writer.WriteLine();
                    writer.WriteLine("internal abstract T Accept(Visitor<T> visitor);");

                    writer.WriteLine("}");
                    writer.WriteLine("}");
                }
            }
        }

        private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
        {
            writer.WriteLine("interface Visitor<T>");
            writer.WriteLine("{");

            foreach (var type in types)
            {
                var typeName = type.Split(":")[0].Trim();
                writer.WriteLine("T Visit" + typeName + baseName + "(" +
                    typeName + " " + baseName.ToLowerInvariant() + ");");
            }

            writer.WriteLine("}");
        }

        private static void DefineType(StreamWriter writer, string baseName, string className, string fieldList)
        {
            writer.WriteLine("internal class " + className + " : " + baseName);
            writer.WriteLine("{");

            // Constructor.                                              
            writer.WriteLine("internal " + className + "(" + fieldList + ")");
            writer.WriteLine("{");

            // Store parameters in fields.                               
            var fields = fieldList.Split(", ");
            foreach (var field in fields)
            {
                var name = field.Split(" ")[1];
                writer.WriteLine("this." + name + " = " + name + ";");
            }

            writer.WriteLine("}");

            // Visitor pattern.                                      
            writer.WriteLine();
            writer.WriteLine("internal T Accept(Visitor<T> visitor)");
            writer.WriteLine("{");
            writer.WriteLine("return visitor.Visit" +
                className + baseName + "(this);");
            writer.WriteLine("}");

            // Fields.                                                   
            writer.WriteLine();
            foreach (var field in fields)
            {
                writer.WriteLine("internal readonly " + field + ";");
            }

            writer.WriteLine("}");
        }
    }
}
