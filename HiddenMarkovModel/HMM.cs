using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenMarkovModel
{
    class HMM
    {
        public HashSet<string> K;
        public HashSet<string> S;
        public Dictionary<string, double> PI;
        public Dictionary<string, double> A;
        public Dictionary<string, double> B;

        public HMM()
        {

            this.K = loadKS("K.txt");
            this.S = loadKS("S.txt");
            this.A=loadABPI("A.txt");
            this.B=loadABPI("B.txt");
            this.PI=loadABPI("PI.txt");

        }

        private HashSet<string> loadKS(string fileName)
        {
            HashSet<string> result = new HashSet<string>();
            StreamReader sr = new StreamReader(fileName);
            while (sr.Peek() >= 0)
            {
                result.Add(sr.ReadLine());
            }
            return result;
        }

        private Dictionary<string, double> loadABPI(string fileName)
        {
            Dictionary<string, double> result= new Dictionary<string, double>();
            StreamReader sr = new StreamReader(fileName);
            while (sr.Peek() >= 0)
            {
                string line = sr.ReadLine();
                string key = line.Split('\t')[0];
                double value= Convert.ToDouble(line.Split('\t')[1]);
                result.Add(key,value);
            }
            return result;
        }


    }
}
