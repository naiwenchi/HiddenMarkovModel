using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenMarkovModel
{
    class Path
    {
        private HMM myHMM;
        public string PathObservation;
        public string PathStatus;
        private List<Tuple<string, string>> pathNodes;
        public double PathProbability;

        public Path(string observations, string status, HMM _myHMM)
        {
            this.PathObservation = observations;
            this.PathStatus = status;
            string[] _statusTmp = status.Split(',');
            string[] _observationTmp = observations.Split(',');

            this.pathNodes = new List<Tuple<string, string>>();
            for (int i = 0; i != _statusTmp.Length; i++)
                pathNodes.Add(new Tuple<string, string>( _observationTmp[i], _statusTmp[i]));

            this.myHMM = _myHMM;
        }

        public double calculatePathProb()
        {
            StringBuilder sb = new StringBuilder("debug for prob, PI:");

            double result = 1.00;
            string iniState = pathNodes[0].Item2;
            result *= this.myHMM.PI[iniState]; //乘上初始機率
            sb.Append(this.myHMM.PI[iniState]);
            sb.Append(" A:");
            //乘上aij，因為一次要看一個pair，所以做到Count-1才不會索引溢位
            for (int i = 0; i != pathNodes.Count - 1; i++)
            {
                string comboIndexA = pathNodes[i].Item2 + "_" + pathNodes[i + 1].Item2;
                result *= this.myHMM.A[comboIndexA];
                sb.Append(this.myHMM.A[comboIndexA]);
                sb.Append(" ,");
            }
            sb.Append(" B:");

            for (int i = 0; i != pathNodes.Count; i++)
            {
                string comboIndexB = pathNodes[i].Item2 + "_" + pathNodes[i].Item1;
                result *= this.myHMM.B[comboIndexB];
                sb.Append(this.myHMM.B[comboIndexB]);
                sb.Append(" ,");
            }
            Console.WriteLine(sb.ToString());
            return result;

        }


    };


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
