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
        public List<string> reversedBestStatusSequence;
        public double mostPossibleProb;
        public HMM myHmm;
        public Dictionary<string, double> deltaFunctions;
        public Dictionary<string, string> psiFunctions;

        public Viterbi(HMM _myHmm, string _observation)
        {
            this.reversedBestStatusSequence = new List<string>();
            this.myHmm = _myHmm;
            deltaFunctions = new Dictionary<string, double>();
            psiFunctions = new Dictionary<string, string>();
            this.observation = _observation;
            obs = observation.Split(',');
            T = obs.Length;
        }

        public void calcBestStatusSequence()
        {



            DataTable dtTotal = new DataTable("deltaInfo");
            dtTotal.Columns.Add("time", typeof(int));
            dtTotal.Columns.Add("tag_j", typeof(string));
            dtTotal.Columns.Add("delta_j_t", typeof(double));
            dtTotal.Columns.Add("psi_j_t-1",typeof(string));

            //第一期的delta必須獨立處理
            foreach (string tag_j in this.myHmm.S)
            {
                string Index_S_INI = tag_j + "_1";
                deltaFunctions.Add(Index_S_INI, this.myHmm.PI[tag_j]);
                dtTotal.Rows.Add(1,tag_j, this.myHmm.PI[tag_j]);
            }

            //舉例：當我在第3期的時候，視同i=2, j=3我要利用到所有的delta(tag_i,2)來計算任何一個delta(tag_j, 3)
            //進入i=3時，我確信所有的delta(tag_i,2)已經完成，離開迴圈之前，我要將所有的delta(tag_j, 3)放進dictionary，
            //但只要在i=4之前完成就好，否則i=4在存取dictiopnary時會找不到

            for (int i = 1; i != T; i++) //雖然我們這裡要從t=2開始討論，但是它會用到t=1代表「上一期」
            {
                //因為我要求出每個tag_j的delta值，而它是由所有的tag_i來，所以tag_j放外層
                foreach (string tag_j in this.myHmm.S) 
                {
                    int j = i + 1; //我已故意讓迴圈的i值呈現前一期，因此j值代表本期，兩者差1，方便我們的符號慣例
                    //下面兩行：決定了這一期的delta的同時，也會同時決定上一期的psi，
                    //兩個值對應的是同樣的tag，不同的t
                    string Index_S_T1 = tag_j +"_"+ Convert.ToString(i); //上一期
                    string Index_S_T2 = tag_j +"_"+ Convert.ToString(j); //這一期

                    //因為我們把所有的候選delta暫存放在一張表中
                    DataTable dtCandidate = new DataTable("deltaInfo");
                    dtCandidate.Columns.Add("time", typeof(int));
                    dtCandidate.Columns.Add("tag_i", typeof(string));
                    dtCandidate.Columns.Add("tag_j", typeof(string));
                    dtCandidate.Columns.Add("delta_i_t", typeof(double));

                    //而tag_i當然放在內層，針對所有的tag_i對應之delta(tag_i, t-1)比大小取最大值
                    foreach (string tag_i in this.myHmm.S)
                    {
                        string index_candidate = tag_i + "_" + Convert.ToString(i);
                        //2018/01/11發現我把deltaFunction的index弄錯了，
                        //它必須跟著上一期的狀態，也就是tag_i，以及上一期的期數
                        double customDelta = 1; //只是為了連乘積而作的暫時性賦值，無意義
                        customDelta *= deltaFunctions[index_candidate]; //找出上一期的delta
                        customDelta *= this.myHmm.A[tag_i + "_" + tag_j]; //乘上上期的A
                        //B有問題，原本把i-1寫成了i，所以有結果但是是錯的，差了一期，
                        //把observation sequence跳了一格，因為陣列的index弄錯了 208/01/11解決
                        customDelta *= this.myHmm.B[tag_i + "_" + obs[i-1]]; //乘上上期的B
                        //完成上述動作之後，我得到了很多「候選」的delta，
                        //取了最大值以後才會得到delta(tag_j, t+1)
                        dtCandidate.Rows.Add(i, tag_i, tag_j, customDelta);
                        //debug
                        //Console.WriteLine("debug:"+i+"\t"+tag_i+"\t"+tag_j+"\t"+customDelta);
                        //Console.Read();
                    }//end foreach tag_i
                     //至此已經算出所有delta(tag_i, t-1)的「加工品」，這時候才可以開始比大小，
                    //比完大小得到delta(tag_j, t)

                    DataView dtView = new DataView(dtCandidate);
                    dtView.Sort = "delta_i_t DESC";
                    //表示我要依"chiSquare"這個欄位排序， DESC是遞減，可寫ASC為遞增，預設也是遞增
                    DataTable sortedTable = dtView.ToTable(); //排序過後寫到另一個table上
                    //debug
                    //for (int x = 0; x != sortedTable.Rows.Count; x++)
                    //{
                    //    Console.WriteLine(sortedTable.Rows[x]["tag_i"] + "\t" + sortedTable.Rows[x]["delta_i_t"]);
                    //}
                    //Console.WriteLine("rank is correct?");
                    //Console.Read();


                    string best_tag_i = Convert.ToString(sortedTable.Rows[0]["tag_i"]);
                    string current_tag_j = Convert.ToString(sortedTable.Rows[0]["tag_j"]);
                    double best_value = Convert.ToDouble(sortedTable.Rows[0]["delta_i_t"]);
                    //至此，已經可以將單一的delta(tag_j, t)寫入dictionary
                    deltaFunctions.Add(Index_S_T2, best_value);
                    psiFunctions.Add(Index_S_T1, best_tag_i);
                    //並且記錄在DataTable版的delta function總表當中，因為我們等一下要再排一次找最大路徑
                    dtTotal.Rows.Add(i + 1, tag_j, best_value, best_tag_i);

                }//end foreach tag_j
                //至此已經完成了所有t=i+1的delta function，因此「前一期」的最佳路徑已經可以計算，
                //以現在t=2舉例，已經可以看t=1的最佳路徑，可是這麼寫會漏掉最後一期，所以我們還是得重跑

            }//end for i

            //重跑迴圈找出最佳路徑，其實很簡單，沿途抓出該期delta function的最大值就好

            //先用最後一期的delta值找到最後一個狀態 
            DataRow[] rowsBestPath = dtTotal.Select("time='" + T + "'");

            DataTable dtTempFinal = dtTotal.Clone();
            foreach (DataRow row in rowsBestPath)
                dtTempFinal.ImportRow(row);

            DataView dtView2 = new DataView(dtTempFinal);
            dtView2.Sort = "delta_j_t DESC";
            //表示我要依"chiSquare"這個欄位排序， DESC是遞減，可寫ASC為遞增，預設也是遞增
            DataTable sortedTable2 = dtView2.ToTable(); //排序過後寫到另一個table上

            string max_tag_j = Convert.ToString(sortedTable2.Rows[0]["tag_j"]);
            double max_value = Convert.ToDouble(sortedTable2.Rows[0]["delta_j_t"]);
            reversedBestStatusSequence.Add(max_tag_j);

            for (int i = T-1; i!=0; i--)
            {
                string laterTag = reversedBestStatusSequence[reversedBestStatusSequence.Count - 1];
                string psiIndex = laterTag + "_" + Convert.ToString(i);
                string max_tag_i = this.psiFunctions[psiIndex];
                reversedBestStatusSequence.Add(max_tag_i);

            }
            //至此找完最佳路徑


            dtTotal.WriteXml("deltaFunction.xml", XmlWriteMode.WriteSchema);

            Console.WriteLine("The best path is..");
            for (int i = this.reversedBestStatusSequence.Count; i!=0; i--)
            {
                Console.Write(this.reversedBestStatusSequence[i-1] + ",");
            }

            Console.WriteLine("The best path prob is :"+max_value);

        }

        

    }
}
