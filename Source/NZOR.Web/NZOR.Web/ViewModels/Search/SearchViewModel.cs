using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NZOR.Publish.Model.Search.Names;

namespace NZOR.Web.ViewData
{
    public class SearchViewModel
    {
        NamesSearchResponse _searchResponse;

        public SearchViewModel()
        {
        }

        public SearchViewModel(NamesSearchResponse searchResponse)
        {
            _searchResponse = searchResponse;
        }

        public SelectList PageSizes
        {
            get
            {
                return new SelectList(new Int32[] { 20, 50, 100 });
            }
        }

        public SelectList OrderBys
        {
            get
            {
                var selects = new List<OrderBySelect>();

                selects.Add(new OrderBySelect("Full Name", "fullnamesort"));
                selects.Add(new OrderBySelect("Taxon Rank", "ranksort,fullnamesort"));
                selects.Add(new OrderBySelect("Score", "score"));

                return new SelectList(selects, "Value", "Name");
            }
        }

        public string Query { get; set; }

        public bool UseFuzzyMatching { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int LastPage
        {
            get
            {
                if (SearchResponse == null || SearchResponse.PageSize == 0)
                {
                    return 1;
                }
                else
                {
                    return (int)Math.Ceiling((decimal)SearchResponse.Total / SearchResponse.PageSize);
                }
            }
        }

        public string OrderBy { get; set; }

        public NamesSearchResponse SearchResponse
        {
            get { return _searchResponse; }
        }
    }

    class OrderBySelect
    {
        public String Name { get; set; }
        public String Value { get; set; }

        public OrderBySelect(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}