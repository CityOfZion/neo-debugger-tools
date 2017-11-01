using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Compiler.MSIL
{
    class CctorSubVM
    {
        static Stack<object> calcStack;
        public static object Dup(object src)
        {
            if (src.GetType() == typeof(byte[]))
            {
                byte[] _src = (byte[])src;
                return _src;
            }
            else if (src.GetType() == typeof(int))
            {
                int v = (int)src;
                return v;
            }
            else if (src.GetType() == typeof(string))
            {
                string v = (string)src;
                string v2 = v;
                return v2;
            }
            else if (src.GetType() == typeof(Boolean))
            {
                Boolean v = (Boolean)src;
                return v;
            }
            else
            {
                return null;
            }
        }
        public static void Parse(ILMethod from, NeoModule to)
        {
            calcStack = new Stack<object>();
            bool bEnd = false;
            foreach (var src in from.body_Codes.Values)
            {
                if (bEnd)
                    break;

                switch (src.code)
                {
                    case CodeEx.Ret:
                        bEnd = true;
                        break;
                    case CodeEx.Ldc_I4_M1:
                        calcStack.Push((int)-1);
                        break;
                    case CodeEx.Ldc_I4_0:
                        calcStack.Push((int)0);
                        break;
                    case CodeEx.Ldc_I4_1:
                        calcStack.Push((int)1);
                        break;
                    case CodeEx.Ldc_I4_2:
                        calcStack.Push((int)2);
                        break;
                    case CodeEx.Ldc_I4_3:
                        calcStack.Push((int)3);
                        break;
                    case CodeEx.Ldc_I4_4:
                        calcStack.Push((int)4);
                        break;
                    case CodeEx.Ldc_I4_5:
                        calcStack.Push((int)5);
                        break;
                    case CodeEx.Ldc_I4_6:
                        calcStack.Push((int)6);
                        break;
                    case CodeEx.Ldc_I4_7:
                        calcStack.Push((int)7);
                        break;
                    case CodeEx.Ldc_I4_8:
                        calcStack.Push((int)8);
                        break;
                    case CodeEx.Ldc_I4:
                    case CodeEx.Ldc_I4_S:
                        calcStack.Push((int)src.tokenI32);
                        break;
                    case CodeEx.Newarr:
                        {
                            if(src.tokenType == "System.Byte")
                            {
                                var count = (int)calcStack.Pop();
                                byte[] data = new byte[count];
                                calcStack.Push(data);
                            }
                        }
                        break;
                    case CodeEx.Dup:
                        {
                            var _src = calcStack.Peek();
                            var _dest = Dup(_src);
                            calcStack.Push(_dest);
                        }
                        break;
                    case CodeEx.Ldtoken:
                        {
                            calcStack.Push(src.tokenUnknown);
                        }
                        break;
                    case CodeEx.Call:
                        {
                            var m = src.tokenUnknown as Mono.Cecil.MethodReference;
                            if(m.DeclaringType.FullName== "System.Runtime.CompilerServices.RuntimeHelpers"&&m.Name== "InitializeArray")
                            {
                                var p1 = (byte[])calcStack.Pop();
                                var p2 = (byte[])calcStack.Pop();
                                for(var i=0;i<p2.Length;i++)
                                {
                                    p2[i] = p1[i];
                                }
                            }
                        }break;
                    case CodeEx.Stsfld:
                        {
                            var field = src.tokenUnknown as Mono.Cecil.FieldReference;
                            var fname = field.DeclaringType.FullName + "::" + field.Name;
                            to.staticfields[fname] = calcStack.Pop();
                        }
                        break;
                }
            }

        }
    }
}
