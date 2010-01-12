using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Matching
{
    class Utility
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

        #endregion 
    }
}
