﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenMarkovModel
{
    class Program
    {
        static void Main(string[] args)
        {
            //List<string> result = new List<string>();
            //string chars = "abc";           
            ////GenerateAllPasswords("", 0, 3, chars, result);
            ////Console.WriteLine(result.Count);

            //List<string> bitch = new List<string>();
            //bitch.Add("CP");
            //bitch.Add("IP");

            //GenerateAllPasswords("", 0, 4, bitch, result);
            //Console.WriteLine(result.Count);

            DataTable dt = new DataTable("bruteForceRecords");
            dt.Columns.Add("Status",typeof(string));
            dt.Columns.Add("Prob_OAndS", typeof(string));
            dt.Columns.Add("Prob_SunderO", typeof(string));


            HMM crazySoftDrinkMachine = new HMM();
            //Backtrack brute = new Backtrack(2, crazySoftDrinkMachine.S);
            Backtrack brute = new Backtrack(4, crazySoftDrinkMachine.S);
            List<string> result = brute.getALLPaths();
            //foreach (string str in result)
            //    Console.WriteLine(str);

            //string observation = "lem,icet";
            string observation = "lem,icet,cola,lem";
            double totalObsProb = 0;
            List<Tuple<string, double>> pathAndProb = new List<Tuple<string, double>>();
            for (int i = 0; i != result.Count; i++)
            {
                Path myPath1 = new Path(observation,result[i], crazySoftDrinkMachine);
                double result1=myPath1.calculatePathProb();
                Console.WriteLine("This status path is: " + result[i]);
                Console.WriteLine("The probablity of P(o,s) is: " + result1);
                pathAndProb.Add(new Tuple<string, double>(result[i], result1));
                totalObsProb += result1;
            }
            Console.WriteLine("This observation path is: "+observation);
            Console.WriteLine("The total probability of this observation path, P(o) is: " + totalObsProb);

            foreach (Tuple<string, double> tb in pathAndProb)
            {
                dt.Rows.Add(tb.Item1, tb.Item2, tb.Item2 / totalObsProb);
            }
            dt.WriteXml("bruteForce.xml",XmlWriteMode.WriteSchema);

            Viterbi myViterbi = new Viterbi(crazySoftDrinkMachine, observation);
            myViterbi.calcBestStatusSequence();

            //Path myPath1 = new Path("lem,icet","CP,CP", crazySoftDrinkMachine);
            //double result1=myPath1.calculatePathProb();
            //Console.WriteLine(result1);
            //Path myPath2 = new Path("lem,icet", "CP,IP", crazySoftDrinkMachine);
            //double result2 = myPath2.calculatePathProb();
            //Console.WriteLine(result2);
            //Console.WriteLine("final: "+(result1+result2));

        }

        #region prototype of backtrace

        //public static void GenerateAllPasswords(string temp, int pos, int siz, string chars, List<string> result)
        //{
        //    if (pos < siz)
        //    {
        //        //         cout << "temp= " << temp << " -1\n";     
        //        for (int i = 0; i < chars.Length; i++)
        //        {
        //            GenerateAllPasswords(temp + chars[i], pos + 1, siz, chars, result);
        //            //         cout << "temp= " << temp << " -3\n";  
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine( "Your password is '" + temp + "'.");
        //        result.Add(temp);
        //        //         if (temp==pwd) { return; }
        //    }
        //}

        //public static void GenerateAllPasswords(string temp, int pos, int siz, List<string> chars, List<string> result)
        //{
        //    if (pos < siz)
        //    {
        //        //         cout << "temp= " << temp << " -1\n";     
        //        for (int i = 0; i < chars.Count; i++)
        //        {
        //            GenerateAllPasswords(temp + chars[i]+ "," , pos + 1, siz, chars, result);
        //            //         cout << "temp= " << temp << " -3\n";  
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("Your password is '" + temp + "'.");
        //        result.Add(temp);
        //        //         if (temp==pwd) { return; }
        //    }
        //}
        #endregion


    };
}
