using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace NZOR.Matching
{
    public class Utility
    {
        #region "Levenshtein"
        //******************************* 
        //*** Get minimum of three values 
        //******************************* 

        private static int Minimum(int a, int b, int c)
        {
            int mi = 0;

            mi = a;
            if (b < mi)
            {
                mi = b;
            }
            if (c < mi)
            {
                mi = c;
            }

            return mi;
        }

        //******************************** 
        //*** Compute Levenshtein Distance 
        //******************************** 

        public static double LevenshteinPercent(string s, string t)
        {
            int maxLen = Math.Max(s.Length, t.Length);
            int ld = LevenshteinDistance(s, t);
            return ((double)(maxLen - ld) * 100 / (double)maxLen);
        }

        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static double LevenshteinWordsPercent(string s, string t)
        {
            int maxLen = Math.Max(CountWords(s), CountWords(t));
            int ld = LevenshteinDistanceWords(s, t);
            return ((double)(maxLen - ld) * 100 / (double)maxLen);
        }

        public static int LevenshteinDistanceWords(string s, string t)
        {
            int i = 0; int j = 0; int pos = 0; int lenA = 0; int lenB = 0; int pos2 = 0;
            int result = 0;

            s = s.Replace("  ", " ");
            t = t.Replace("  ", " ");

            lenA = CountWords(s) + 1;
            lenB = CountWords(t) + 1;

            if (lenA == 1 || lenB == 1) return lenA + lenB - 2;

            ArrayList sWords = new ArrayList(s.Split(' '));
            ArrayList tWords = new ArrayList(t.Split(' '));
            
            int[,] vals = new int[lenB, lenA];

            for (i = 0; i < lenB; i++)
            {
                for (j = 0; j < lenA; j++)
                {
                    vals[i, j] = 0;
                }
            }

            for (i = 0; i < lenB; i++)
            {
                vals[i, 0] = i;
            }
            for (j = 0; j < lenA; j++)
            {
                vals[0, j] = j;
            }

            int cost = 0; int cost1 = 0; int cost2 = 0; int cost3 = 0;
            
            for (pos = 1; pos < lenB; pos++)
            {
                string tWord = tWords[pos-1].ToString();

                for (pos2 = 1; pos2 < lenA; pos2++)
                {
                    string sWord = sWords[pos2-1].ToString();

                    if (sWord == tWord) cost = 0;
                    else cost = 1; //substitute cost

                    cost1 = vals[pos, pos2 - 1] + 1; //insert penalty
                    cost2 = vals[pos - 1, pos2] + 1; //delete penalty
                    cost3 = vals[pos - 1, pos2 - 1] + cost;
                    
                    vals[pos, pos2] = Minimum(cost1, cost2, cost3);
                }
            }

            result = vals[lenB - 1, lenA - 1];

            return result;
        }

        public static int CountWords(string s)
        {
            int c = 0;
            for (int i = 1; i < s.Length; i++)
            {
                if (char.IsWhiteSpace(s[i - 1]) == true)
                {
                    if (char.IsLetterOrDigit(s[i]) == true ||
                        char.IsPunctuation(s[i]))
                    {
                        c++;
                    }
                }
            }
            if (s.Length > 2)
            {
                c++;
            }
            return c;
        }


        #endregion 
    }
}
