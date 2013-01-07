using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NZOR.Web.ViewData;
using NZOR.Web.Helpers;
using NZOR.Web.Service.Client;
using System.Text.RegularExpressions;
using NZOR.Publish.Model.Search;
using NZOR.Publish.Model.Search.Names;

namespace NZOR.Web.Test.Controllers
{
    public class SearchController : Controller
    {
        private readonly ServiceClient _service;

        public SearchController()
        {
            _service = new ServiceClient("", System.Configuration.ConfigurationManager.AppSettings["ServiceUrl"]);
        }

        public ActionResult Search(string query, string[] filterQuery, int page = 1, int pageSize = 20, string orderby = "", bool useFuzzyMatching = false, bool refine = false)
        {
            if (query != null)
            {
                string queryLucene = HttpUtility.UrlDecode(query);
                // Replace any non-breaking spaces with normal spaces.
                queryLucene = Regex.Replace(queryLucene, @"\u00A0", " ");
                queryLucene = queryLucene.Replace(".", " ");
                queryLucene = queryLucene.Replace(",", " ");
                queryLucene = queryLucene.Replace("'", " ");
                queryLucene = queryLucene.Replace("-", " ");
                queryLucene = queryLucene.Replace("(", " ");
                queryLucene = queryLucene.Replace(")", " ");
                queryLucene = queryLucene.Replace("[", " ");
                queryLucene = queryLucene.Replace("]", " ");
                queryLucene = queryLucene.Replace("&amp;", " ");

                page = Math.Max(page, 1);

                if (refine)
                {
                    queryLucene = "\"" + queryLucene + "\"";
                }

                if (!String.IsNullOrWhiteSpace(queryLucene))
                {
                    string[] terms = queryLucene.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string[] removeTerms = { "as" };

                    for (int index = 0; index < terms.Length; index++)
                    {
                        terms[index] = terms[index].Trim();

                        if (terms[index].Length == 1 || removeTerms.Contains(terms[index]))
                        {
                            terms[index] = String.Empty;
                        }
                        else
                        {
                            if (useFuzzyMatching)
                            {
                                terms[index] += "~" + " OR *" + terms[index] + "*";
                            }
                            else
                            {
                                terms[index] = "*" + terms[index] + "*";
                            }
                        }
                    }

                    queryLucene = String.Join(" ", terms);
                }

                var request = new SearchRequest();

                request.Query = queryLucene ?? String.Empty;
                if (filterQuery != null)
                {
                    request.Filter = String.Join(",", filterQuery);
                }
                request.Page = page;
                request.PageSize = pageSize;
                request.OrderBy = orderby ?? "fullnamesort";

                NamesSearchResponse response = _service.Search(request);
                var viewModel = new SearchViewModel(response);

                viewModel.Query = query;
                viewModel.Page = page;
                viewModel.PageSize = pageSize;
                viewModel.OrderBy = orderby;
                viewModel.UseFuzzyMatching = useFuzzyMatching;

                return View(viewModel);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult GetSearchAutoComplete(String prefix, Int32 take)
        {
            List<String> names = _service.LookupNames(prefix.ToLower(), take);

            return Json(names);
        }
    }
}
