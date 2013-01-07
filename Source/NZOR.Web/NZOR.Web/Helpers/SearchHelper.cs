using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NZOR.Web.Helpers
{
    public class SearchHelper
    {

        private const string AMPERSAND_TAG = "&";
        private const string AND_TAG = "AND";
        private const string ASTERISK = "*";
        private const string CLOSE_BRACE = ")";
        private const string FORMSOF_TAG = "FORMSOF";
        private const string INFLECTIONAL_TAG = "INFLECTIONAL,";
        private const string MINUS_TAG = "-";
        private const string NEAR_TAG = "NEAR";
        private const string NOT_TAG = "NOT";
        private const string OPEN_BRACE = "(";
        private const string OR_TAG = "OR";
        private const string OR_ABBREVIATED_TAG = "|";
        private const string PLUS_TAG = "+";
        private const string QUOTE = "\"";
        private const string SINGLE_QUOTE = "'";
        private const string SPACE = " ";
        private const string FUZZY_TAG = "~";

        enum ETokenState
        {
            DefaultState = 0,
            BeforeAsterisk,
            AfterAsterisk,
            AfterAnd,
            AfterNear,
            AfterNot,
            AfterOr,
            AfterPlus,
            AfterStart,
            InQuote
        };

        public static string BuildSearchString(string query, bool useFuzzyMatching)
        {
            string searchText = string.Empty;

            query = CleanSearchString(query);
            string[] Tokens = query.Split(SPACE.ToCharArray());
            ETokenState state = ETokenState.DefaultState;

            foreach (string strToken in Tokens)
            {
                switch (strToken.ToUpper())
                {
                    case AMPERSAND_TAG:
                        if (state == ETokenState.AfterOr || state == ETokenState.AfterAnd || state == ETokenState.AfterNear)
                        {
                            throw new Exception("AND cannot follow OR(+), AND or NEAR");
                        }

                        if (!string.IsNullOrEmpty(searchText.Trim()))
                        {
                            searchText += SPACE + AND_TAG;
                            state = ETokenState.AfterAnd;
                        }
                        break;
                    case AND_TAG:
                        if (state == ETokenState.AfterOr || state == ETokenState.AfterAnd || state == ETokenState.AfterNear)
                        {
                            throw new Exception("AND cannot follow OR(+), AND or NEAR");
                        }

                        if (!string.IsNullOrEmpty(searchText.Trim()))
                        {
                            searchText += SPACE + AND_TAG;
                            state = ETokenState.AfterAnd;
                        }
                        break;
                    case CLOSE_BRACE:
                        break;
                    case FUZZY_TAG:
                        if (!(state == ETokenState.AfterNear || state == ETokenState.AfterNot || state == ETokenState.AfterOr || state == ETokenState.AfterPlus) && !searchText.EndsWith(FUZZY_TAG))
                        {
                            searchText += FUZZY_TAG;
                        }
                        break;
                    case MINUS_TAG:
                        if (state == ETokenState.AfterNot || state == ETokenState.AfterNear)
                        {
                            throw new Exception("NOT(-) cannot follow NOT(-) or NEAR");
                        }

                        if (!(state == ETokenState.AfterOr || state == ETokenState.AfterAnd || string.IsNullOrEmpty(searchText.Trim())))
                        {
                            searchText += SPACE + AND_TAG;
                        }

                        searchText += SPACE + NOT_TAG + SPACE;
                        state = ETokenState.AfterNot;
                        break;
                    case NEAR_TAG:
                        if (state == ETokenState.AfterNot || state == ETokenState.AfterAnd || state == ETokenState.AfterOr || state == ETokenState.AfterNear)
                        {
                            throw new Exception("NEAR cannot follow OR, AND or NEAR");
                        }

                        searchText += SPACE + NEAR_TAG;
                        state = ETokenState.AfterNear;
                        break;
                    case NOT_TAG:
                        if (state == ETokenState.AfterNot || state == ETokenState.AfterNear)
                        {
                            throw new Exception("NOT(-) cannot follow NOT(-) or NEAR");
                        }

                        if (!(state == ETokenState.AfterOr || state == ETokenState.AfterAnd || string.IsNullOrEmpty(searchText.Trim())))
                        {
                            searchText += SPACE + AND_TAG + SPACE;
                        }

                        searchText += SPACE + NOT_TAG + SPACE;
                        state = ETokenState.AfterNot;
                        break;
                    case OPEN_BRACE:
                        break;
                    case OR_TAG:
                        if (state == ETokenState.AfterOr || state == ETokenState.AfterAnd || state == ETokenState.AfterNear)
                        {
                            throw new Exception("OR cannot follow OR, AND or NEAR");
                        }

                        searchText += SPACE + OR_TAG;
                        state = ETokenState.AfterOr;
                        break;
                    case OR_ABBREVIATED_TAG:
                        if (state == ETokenState.AfterOr || state == ETokenState.AfterAnd || state == ETokenState.AfterNear)
                        {
                            throw new Exception("OR cannot follow OR, AND or NEAR");
                        }

                        searchText += SPACE + OR_TAG;
                        state = ETokenState.AfterOr;
                        break;
                    case PLUS_TAG:
                        if (!string.IsNullOrEmpty(searchText.Trim()))
                        {
                            state = ETokenState.AfterPlus;
                        }
                        break;
                    case QUOTE:
                        if (!(state == ETokenState.AfterNot || state == ETokenState.AfterOr || state == ETokenState.AfterAnd || query.Trim() == ""))
                        {
                            if (state == ETokenState.InQuote)
                            {
                                searchText += QUOTE;
                            }
                            else
                            {
                                searchText += SPACE + QUOTE;
                            }
                        }

                        if (state == ETokenState.InQuote)
                        {
                            state = ETokenState.DefaultState;
                            if (useFuzzyMatching && !searchText.EndsWith(FUZZY_TAG))
                            {
                                searchText += FUZZY_TAG;
                            }
                        }
                        else
                        {
                            state = ETokenState.InQuote;
                        }
                        break;
                    default:
                        if (!(state == ETokenState.InQuote || state == ETokenState.AfterAnd || state == ETokenState.AfterNear || state == ETokenState.AfterOr || state == ETokenState.AfterNot || string.IsNullOrEmpty(searchText.Trim())))
                        {
                            searchText += SPACE + AND_TAG;
                        }

                        if (strToken.StartsWith(QUOTE))
                        {
                            searchText += SPACE + strToken;

                            if (!strToken.EndsWith(QUOTE))
                            {
                                state = ETokenState.InQuote;
                            }
                            else
                            {
                                state = ETokenState.DefaultState;
                                if (useFuzzyMatching && !searchText.EndsWith(FUZZY_TAG))
                                {
                                    searchText += FUZZY_TAG;
                                }
                            }
                        }
                        else if (strToken.EndsWith(QUOTE))
                        {
                            searchText += SPACE + strToken;
                            if (state != ETokenState.InQuote)
                            {
                                searchText.TrimEnd(QUOTE.ToCharArray());
                            }
                            state = ETokenState.DefaultState;
                            if (useFuzzyMatching && !searchText.EndsWith(FUZZY_TAG))
                            {
                                searchText += FUZZY_TAG;
                            }
                        }
                        else
                        {
                            if (state == ETokenState.InQuote)
                            {
                                if (searchText.EndsWith(QUOTE))
                                {
                                    searchText += strToken;
                                }
                                else
                                {
                                    searchText += SPACE + strToken;
                                }
                            }
                            else if (strToken.StartsWith(PLUS_TAG))
                            {
                                searchText += SPACE + strToken;
                                if (strToken[1].Equals(QUOTE))
                                {
                                    if (strToken.EndsWith(QUOTE))
                                    {
                                        state = ETokenState.DefaultState;
                                    }
                                    else
                                    {
                                        state = ETokenState.InQuote;
                                    }
                                }
                                else
                                {
                                    state = ETokenState.DefaultState;
                                }
                            }
                            else if (strToken.StartsWith(MINUS_TAG))
                            {
                                searchText += SPACE + strToken;
                                if (strToken[1].Equals(QUOTE))
                                {
                                    if (strToken.EndsWith(QUOTE))
                                    {
                                        state = ETokenState.DefaultState;
                                    }
                                    else
                                    {
                                        state = ETokenState.InQuote;
                                    }
                                }
                                else
                                {
                                    state = ETokenState.DefaultState;
                                }
                            }
                            else if (state == ETokenState.AfterPlus)
                            {
                                searchText += SPACE + strToken;
                                state = ETokenState.DefaultState;
                            }
                            else
                            {
                                if (useFuzzyMatching && !searchText.EndsWith(FUZZY_TAG))
                                {
                                    searchText += SPACE + strToken + FUZZY_TAG;
                                }
                                else
                                {
                                    searchText += SPACE + ASTERISK + strToken + ASTERISK;
                                }
                                state = ETokenState.DefaultState;
                            }
                        }
                        break;
                }
            }

            if (searchText.EndsWith(AND_TAG))
            {
                searchText = searchText.TrimEnd(AND_TAG.ToCharArray());
            }
            else if (searchText.EndsWith(OR_TAG))
            {
                searchText = searchText.TrimEnd(OR_TAG.ToCharArray());
            }
            else if (searchText.EndsWith(NEAR_TAG))
            {
                searchText = searchText.TrimEnd(NEAR_TAG.ToCharArray());
            }

            return searchText;
        }



        private static string CleanSearchString(string searchString)
        {
            if (searchString != string.Empty)
            {
                searchString = searchString.ToLower();

                searchString = searchString.Replace("update", "");
                searchString = searchString.Replace("select", "");
                searchString = searchString.Replace("insert", "");
                searchString = searchString.Replace("into", "");
                searchString = searchString.Replace("drop", "");
                searchString = searchString.Replace("delete", "");
                searchString = searchString.Replace("xp_", "");

                searchString = searchString.Replace("--", "");
                /*'searchString = searchString.Replace("*", "");
                searchString = searchString.Replace("%", "");*/
                searchString = searchString.Replace("'", "''");
                searchString = searchString.Replace(";", "");

                //Temporary lucene work-around
                searchString = searchString.Replace(". ", " ");
               searchString = searchString.TrimEnd(".".ToCharArray());
            }

            return searchString.Trim();
        }

    }
}