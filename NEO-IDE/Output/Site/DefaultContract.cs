using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace Example
{
    public class Calculator : SmartContract
    {
        public static int Main(string operation, params object[] args)
        {
            /*Runtime.Notify(operation);
            Runtime.Notify(args);
            return 1;*/
            int arg0 = (int)args[0];
            int arg1 = (int)args[1];
            int result = -1;

            Runtime.Notify("CalculatorContract Init", arg0, arg1);

            if (operation == "add")
            {
                result = CalculatorAdd(arg0, arg1);
            }

            if (operation == "sub")
            {
                result = CalculatorSub(arg0, arg1);
            }

            Runtime.Notify("Calculator", operation, result);
            return result;
        }

        static private int CalculatorSub(int a, int b)
        {
            int sum = a - b;

            Runtime.Notify("CalculatorSub Received", a, b);
            Runtime.Notify("CalculatorSub Result", sum);

            return sum;
        }

        static private int CalculatorAdd(int a, int b)
        {
            int sum = a + b;

            Runtime.Notify("CalculatorAdd Received", a, b);
            Runtime.Notify("CalculatorAdd Result", sum);

            return sum;
        }
    }
}
