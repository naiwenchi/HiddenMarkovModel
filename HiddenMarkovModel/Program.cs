using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenMarkovModel
{
    class Program
    {
        static void Main(string[] args)
        {
            HMM crazySoftDrinkMachine = new HMM();
            Path myPath1 = new Path("lem,icet","CP,CP", crazySoftDrinkMachine);
            double result1=myPath1.calculatePathProb();
            Console.WriteLine(result1);
            Path myPath2 = new Path("lem,icet", "CP,IP", crazySoftDrinkMachine);
            double result2 = myPath2.calculatePathProb();
            Console.WriteLine(result2);
            Console.WriteLine("final: "+(result1+result2));

        }
    }
}
