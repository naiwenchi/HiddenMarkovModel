using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenMarkovModel
{
    class Backtrack
    {
        private List<string> s;
        private string[] solution;    // 用來存放一組可能的情形
        public int T; // n 為現在正在枚舉的維度


        public Backtrack(int _T, HashSet<string> _s)
        {
            this.T = _T;
            this.s = _s.ToList<string>();
            this.solution = new string[T];
        }

        public List<string> getALLPaths()
        {
            List<string> result = new List<string>();
            string temp = "";
            GenerateAllPasswords(temp, 0, T, this.s, result);
            return result;
        }


        public static void GenerateAllPasswords(string temp, int pos, int siz, List<string> chars, List<string> result)
        {
            if (pos < siz)
            {
                //         cout << "temp= " << temp << " -1\n";     
                for (int i = 0; i < chars.Count; i++)
                {
                    GenerateAllPasswords(temp + chars[i] + ",", pos + 1, siz, chars, result);
                    //         cout << "temp= " << temp << " -3\n";  
                }
            }
            else
            {
                //Console.WriteLine("Your password is '" + temp + "'.");
                result.Add(temp.Remove(temp.Length-1,1));
                //         if (temp==pwd) { return; }
            }
        }


    };
}
