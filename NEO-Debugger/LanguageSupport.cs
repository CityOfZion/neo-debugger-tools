using Neo.Debugger.Data;
using System;
using System.IO;

namespace Neo.Debugger
{
    public static class LanguageSupport
    {
        public static string[] GetLanguageKeywords(SourceLanguage language)
        {
            switch (language)
            {
                case SourceLanguage.CSharp:
                    {
                        return new string[2]
                        {
                             "class extends implements import interface new case do while else if for in switch throw get set function var try catch finally while with default break continue delete return each const namespace package include use is as instanceof typeof author copy default deprecated eventType example exampleText exception haxe inheritDoc internal link mtasc mxmlc param private return see serial serialData serialField since throws usage version langversion playerversion productversion dynamic private public partial static intrinsic internal native override protected AS3 final super this arguments null Infinity NaN undefined true false abstract as base bool break by byte case catch char checked class const continue decimal default delegate do double descending explicit event extern else enum false finally fixed float for foreach from goto group if implicit in int interface internal into is lock long new null namespace object operator out override orderby params private protected public readonly ref return switch struct sbyte sealed short sizeof stackalloc static string select this throw true try typeof uint ulong unchecked unsafe ushort using var virtual volatile void while where yield",
                             "void Null ArgumentError arguments Array Boolean Class Date Error EvalError Function int Math Namespace Number Object RangeError ReferenceError RegExp SecurityError String SyntaxError TypeError uint XML XMLList Boolean Byte Char DateTime Decimal Double Int16 Int32 Int64 IntPtr SByte Single UInt16 UInt32 UInt64 UIntPtr Void Path File System Runtime"
                        };
                    }

                case SourceLanguage.Python:
                    {
                        return new string[2]
                        {
                             "class finally is return continue for lambda try def from nonlocal while and del global not with as elif if or yield assert else import pass break except in raise",
                             "False True None Runtime"
                        };
                    }

                default: return new string[0] { };
            }
        }

        public static SourceLanguage DetectLanguage(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            switch (extension)
            {
                case ".cs": return SourceLanguage.CSharp;
                case ".java": return SourceLanguage.Java;
                case ".py": return SourceLanguage.Python;
                case ".js": return SourceLanguage.Javascript;
                case ".asm": return SourceLanguage.Assembly;

                default: return SourceLanguage.Other;
            }
        }

    }
}
