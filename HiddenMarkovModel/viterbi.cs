using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenMarkovModel
{
    class Viterbi
    {
        private string observation;
        private string[] obs;
        private int T;
        public List<string> bestStatusSequence;
        public double mostPossibleProb;
        public HMM myHmm;
        public Dictionary<string, double> deltaFunctions;

        public Viterbi(HMM _myHmm, string _observation)
        {
            this.bestStatusSequence = new List<string>();
            this.myHmm = _myHmm;
            deltaFunctions = new Dictionary<string, double>();
            this.observation = _observation;
            obs = observation.Split(',');
            T = obs.Length;
        }

        public void calcBestStatusSequence()
        {

            foreach (string tag_i in this.myHmm.S)
            {
                string Index_S_INI = tag_i + "_1";
                deltaFunctions.Add(Index_S_INI, this.myHmm.PI[tag_i]);
            }


            for (int i = 1; i != T; i++)
            {
                foreach (string tag_i in this.myHmm.S)
                {
                    int t = i + 1;
                    string Index_S_T1 = tag_i +"_"+ Convert.ToString(i); //上一期
                    string Index_S_T2 = tag_i +"_"+ Convert.ToString(t); //這一期

                    DataTable dt = new DataTable("delta");
                    dt.Columns.Add("tag_i", typeof(string));
                    dt.Columns.Add("tag_j", typeof(string));
                    dt.Columns.Add("value", typeof(double));

                    foreach (string tag_j in this.myHmm.S)
                    {
                        double customDelta = 1;
                        customDelta *= deltaFunctions[Index_S_T1];
                        customDelta *= this.myHmm.A[tag_i + "_" + tag_j];
                        customDelta *= this.myHmm.B[tag_i + "_" + obs[i]];
                        dt.Rows.Add(tag_i, tag_j, customDelta);
                    }//end foreachtag_j

                    DataView dtView = new DataView(dt);
                    dtView.Sort = "value DESC";
                    //表示我要依"chiSquare"這個欄位排序， DESC是遞減，可寫ASC為遞增，預設也是遞增
                    DataTable sortedTable = dtView.ToTable(); //排序過後寫到另一個table上

                    this.bestStatusSequence.Add(Convert.ToString(sortedTable.Rows[0]["tag_i"]));
                    deltaFunctions.Add(Index_S_T2, Convert.ToDouble(sortedTable.Rows[0]["value"]));
                    Console.WriteLine("the best path element of t=" + i + " is " + tag_i + " , the prob of delta is: " + sortedTable.Rows[0]["value"]);


                }//end foreach tag i

            }//end for i

            Console.WriteLine("The best path is..");
            for (int i = 0; i != this.bestStatusSequence.Count; i++)
                Console.Write(this.bestStatusSequence[i] + ",");

        }

        

    }
}
