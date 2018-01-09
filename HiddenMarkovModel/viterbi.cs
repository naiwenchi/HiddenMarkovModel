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
        public List<Tuple<int,string,string,double>> bestStatusSequence;
        public double mostPossibleProb;
        public HMM myHmm;
        public Dictionary<string, double> deltaFunctions;

        public Viterbi(HMM _myHmm, string _observation)
        {
            this.bestStatusSequence = new List<Tuple<int, string, string, double>>();
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

            DataTable dt = new DataTable("delta");
            dt.Columns.Add("time", typeof(int));
            dt.Columns.Add("tag_i", typeof(string));
            dt.Columns.Add("tag_j", typeof(string));
            dt.Columns.Add("value", typeof(double));

            //舉例：當我在第3期的時候，視同i=2, j=3我要利用到所有的delta(tag_i,2)來計算任何一個delta(tag_j, 3)
            //進入i=3時，我確信所有的delta(tag_i,2)已經完成，離開迴圈之前，我要將所有的delta(tag_j, 3)放進dictionary，
            //但只要在i=4之前完成就好，否則i=4在存取dictiopnary時會找不到

            for (int i = 1; i != T; i++) 
            {
                //因為我要求出每個tag_j的delta值，而它是由所有的tag_i來，所以tag_j放外層
                foreach (string tag_j in this.myHmm.S) 
                {
                    int j = i + 1; //我已故意讓迴圈的i值呈現前一期，因此j值代表本期，兩者差1，方便我們的符號慣例
                    string Index_S_T1 = tag_j +"_"+ Convert.ToString(i); //上一期
                    string Index_S_T2 = tag_j +"_"+ Convert.ToString(j); //這一期

                    //而tag_i當然放在內層，針對所有的tag_i對應之delta(tag_i, t-1)比大小取最大值
                    foreach (string tag_i in this.myHmm.S)
                    {
                        double customDelta = 1;
                        customDelta *= deltaFunctions[Index_S_T1];
                        customDelta *= this.myHmm.A[tag_i + "_" + tag_j];
                        customDelta *= this.myHmm.B[tag_i + "_" + obs[i]];
                        dt.Rows.Add(i, tag_i, tag_j, customDelta);
                    }//end foreach tag_i
                     //至此已經算出所有delta(tag_i, t-1)的「加工品」，這時候才可以開始比大小，
                    //比完大小得到delta(tag_j, t)

                    DataTable dt2 = dt.Clone(); //因為我們把所有的delta暫存放在一張表中，所以這裡要篩選出t=i
                    DataRow[] selected = dt.Select("time='" + i + "'");
                    foreach (DataRow row in selected)
                        dt2.ImportRow(row);

                    DataView dtView = new DataView(dt2);
                    dtView.Sort = "value DESC";
                    //表示我要依"chiSquare"這個欄位排序， DESC是遞減，可寫ASC為遞增，預設也是遞增
                    DataTable sortedTable = dtView.ToTable(); //排序過後寫到另一個table上


                    string best_tag_i = Convert.ToString(sortedTable.Rows[0]["tag_i"]);
                    string best_tag_j = Convert.ToString(sortedTable.Rows[0]["tag_j"]);
                    double best_value = Convert.ToDouble(sortedTable.Rows[0]["value"]);
                    //至此，已經可以將單一的delta(tag_j, t)寫入dictionary
                    deltaFunctions.Add(Index_S_T2, best_value);


                }//end foreach tag_j


            }//end for i

            //完成所有delta function後，最後才來找最佳path
            for (int i = 0; i != T; i++)
            {
                foreach (string tag_i in this.myHmm.S)
                {
                    //string Index_S_T1 = tag_i + "_" + Convert.ToString(i+1); 
                    //double customDelta = 1;
                    //customDelta *= deltaFunctions[Index_S_T1];
                    //customDelta *= this.myHmm.A[tag_i + "_" + tag_j];
                    //customDelta *= this.myHmm.B[tag_i + "_" + obs[i]];
                    //dt.Rows.Add(i, tag_i, tag_j, customDelta);

                    //this.bestStatusSequence.Add(new Tuple<int, string, string, double>(i, best_tag_i, best_tag_j, best_value));
                    //Console.WriteLine("the best path element of t=" + i + " is " + tag_j + " , the prob of delta is: " + sortedTable.Rows[0]["value"]);

                }//end tag_i


            }





            dt.WriteXml("bestPath.xml", XmlWriteMode.WriteSchema);

            Console.WriteLine("The best path is..");
            for (int i = 0; i != this.bestStatusSequence.Count; i++)
            {
                Console.Write(this.bestStatusSequence[i].Item1 + ",");
                Console.Write(this.bestStatusSequence[i].Item2 + ",");
                Console.Write(this.bestStatusSequence[i].Item3 + ",");
                Console.WriteLine(this.bestStatusSequence[i].Item4);

            }

        }

        

    }
}
