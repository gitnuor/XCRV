using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCRV.Web.Helpers
{
    public class MaskCardNumber
    {

        public readonly static int d = 256;

        public MaskCardNumber()
        {
        }
        public string Mask(string txt, string[] patCollection)
        {
            List<int> matchedIndex = new List<int>();
            //1....match search one by one and the position
            foreach (string st in patCollection)
            {
                List<int> result = search(st, txt);
                foreach (int num in result)
                    matchedIndex.Add(num);
            }
            //2...confirm its a card number(16 disgit confirm)
            foreach (int data in matchedIndex)
            {
                //send sub sTRING
                if (isValid(txt, data))
                {
                    txt = replace(txt, data);
                }
            }
            return txt;
            //Console.WriteLine(txt);
        }
        private string replace(string txt, int startPosition)
        {
            StringBuilder sb = new StringBuilder(txt);
            //replace text here
            for (int i = startPosition + 6; i < startPosition + 12; i++)
            {
                sb[i] = '*';
            }
            return sb.ToString();
        }

        public bool isValid(string number, int position)
        {
            string st = number.Substring(position);
            if (st.Length < 16)
                return false;
            else

                return (IsNumber(number.Substring(position, 16)));
        }
        ///robin karp algorithm
        static List<int> search(String pat, String txt)
        {
            List<int> listData = new List<int>();

            // A prime number 
            int q = 101;

            int M = pat.Length;
            int N = txt.Length;
            int i, j;
            int p = 0; // hash value for pattern 
            int t = 0; // hash value for txt 
            int h = 1;

            // The value of h would be "pow(d, M-1)%q" 
            for (i = 0; i < M - 1; i++)
                h = (h * d) % q;
            // Calculate the hash value of pattern and first 
            // window of text 
            for (i = 0; i < M; i++)
            {
                p = (d * p + pat[i]) % q;
                t = (d * t + txt[i]) % q;
            }
            // Slide the pattern over text one by one 
            for (i = 0; i <= N - M; i++)
            {
                // Check the hash values of current window of text 
                // and pattern. If the hash values match then only 
                // check for characters on by one 
                if (p == t)
                {
                    /* Check for characters one by one */
                    for (j = 0; j < M; j++)
                    {
                        if (txt[i + j] != pat[j])
                            break;
                    }
                    // if p == t and pat[0...M-1] = txt[i, i+1, ...i+M-1] 
                    if (j == M)
                    {
                        //Console.WriteLine("Pattern found at index " + i);
                        listData.Add(i);
                    }
                }

                // Calculate hash value for next window of text: Remove 
                // leading digit, add trailing digit 
                if (i < N - M)
                {
                    t = (d * (t - txt[i] * h) + txt[i + M]) % q;

                    // We might get negative value of t, converting it 
                    // to positive 
                    if (t < 0)
                        t = (t + q);
                }
            }
            return listData;
        }
        private bool IsNumber(string s)
        {
            foreach (char c in s)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return s.Any();
        }
    }
}
