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

            int m = 0;
            // length of t 
            int n = 0;
            // length of s 
            int i = 0;
            // iterates through s 
            int j = 0;
            // iterates through t 
            string s_i = null;
            // ith character of s 
            string t_j = null;
            // jth character of t 
            int cost = 0;
            // cost 

            // Step 1 
            n = s.Length;
            m = t.Length;
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }

            int[,] d = new int[n + 1, m + 1];
            // matrix 

            // Step 2 
            for (i = 0; i <= n; i++)
            {
                d[i, 0] = i;
            }

            for (j = 0; j <= m; j++)
            {
                d[0, j] = j;
            }

            // Step 3 
            for (i = 1; i <= n; i++)
            {
                s_i = s.Substring(i, 1);

                // Step 4 
                for (j = 1; j <= m; j++)
                {
                    t_j = t.Substring(j, 1);

                    // Step 5 
                    if (s_i == t_j)
                    {
                        cost = 0;
                    }
                    else
                    {
                        cost = 1;
                    }

                    // Step 6 
                    d[i, j] = Minimum(d[i - 1, j] + 1, d[i, j - 1] + 1, d[i - 1, j - 1] + cost);
                }
            }

            // Step 7 
            return d[n, m];
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
