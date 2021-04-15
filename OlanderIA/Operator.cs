using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace OlanderIA
{
    public class Operator
    {
        // Dictionary
        Dictionary<int, MethodInfo> OperatorList = new Dictionary<int, MethodInfo>();
        public Operator()
        {
            // This adds all the operator methods to the dictionary in a specific order
            Type temp = this.GetType();
            OperatorList.Add(1, temp.GetMethod("LessThan"));
            OperatorList.Add(2, temp.GetMethod("LessThanEqual"));
            OperatorList.Add(3, temp.GetMethod("GreaterThan"));
            OperatorList.Add(4, temp.GetMethod("GreaterThanEqual"));
            OperatorList.Add(5, temp.GetMethod("AND"));
            OperatorList.Add(6, temp.GetMethod("OR"));
            OperatorList.Add(7, temp.GetMethod("NAND"));
            OperatorList.Add(8, temp.GetMethod("NOR"));
            OperatorList.Add(9, temp.GetMethod("XOR"));
            OperatorList.Add(10, temp.GetMethod("XNOR"));
            OperatorList.Add(11, temp.GetMethod("Multiply"));
            OperatorList.Add(12, temp.GetMethod("Divide"));
            OperatorList.Add(13, temp.GetMethod("Add"));
            OperatorList.Add(14, temp.GetMethod("Subtract"));
        }
        public object CallFunction(int Function, object a, object b)
        {
            try
            {
                return OperatorList[Function].Invoke(this, new object[] { a, b });
            }
            catch(ArgumentException)
            {
                return new object();
            }
        }

        // Number operators
        public bool LessThan(double a, double b)
        {
            if (a < b)
                return true;
            else
                return false;
        }
        public bool LessThanEqual(double a, double b)
        {
            if (a <= b)
                return true;
            else
                return false;
        }
        public bool GreaterThan(double a, double b)
        {
            if (a > b)
                return true;
            else
                return false;
        }
        public bool GreaterThanEqual(double a, double b)
        {
            if (a >= b)
                return true;
            else
                return false;
        }
        // Boolean logic gates
        public bool AND(bool a, bool b)
        {
            if (a && b)
                return true;
            else
                return false;
        }
        public bool OR(bool a, bool b)
        {
            if (!a && !b)
                return false;
            else
                return true;
        }
        public bool NAND(bool a, bool b)
        {
            if (a && b)
                return false;
            else
                return true;
        }
        public bool NOR(bool a, bool b)
        {
            if (!a && !b)
                return true;
            else
                return false;
        }
        public bool XOR(bool a, bool b)
        {
            if (a == b)
                return false;
            else
                return true;
        }
        public bool XNOR(bool a, bool b)
        {
            if (a == b)
                return true;
            else
                return false;
        }
        // Arithmetic operators
        public double Multiply(double a, double b)
        {
            return (a * b);
        }
        public double Divide(double a, double b)
        {
            return (a / b);
        }
        public double Add(double a, double b)
        {
            return (a + b);
        }
        public double Subtract(double a, double b)
        {
            return (a - b);
        }
    }
}
