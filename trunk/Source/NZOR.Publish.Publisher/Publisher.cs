using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace NZOR.Publish.Publisher
{
    public class Publisher
    {
        private IndexBuilder _builder = null;
        private bool _debug = false;

        public Publisher(IndexBuilder builder, bool debug)
        {
            _builder = builder;
            _debug = debug;
        }

        public void GenerateIndexes()
        {
            var stopwatch = new Stopwatch();

            if (_debug) stopwatch.Restart();

            _builder.BuildStatistics();

            if (_debug)
            {
                Console.WriteLine(String.Format("Statistics.Build: {0:#,##0}s", stopwatch.Elapsed.TotalSeconds));
                stopwatch.Restart();
            }

            _builder.BuildDeprecatedRecords();

            if (_debug)
            {
                Console.WriteLine(String.Format("DeprecatedRecords.Build: {0:#,##0}s", stopwatch.Elapsed.TotalSeconds));
                stopwatch.Restart();
            }

            _builder.BuildProviders();

            if (_debug)
            {
                Console.WriteLine(String.Format("Providers.Build: {0:#,##0}s", stopwatch.Elapsed.TotalSeconds));
                stopwatch.Restart();
            }
            
            _builder.BuildGeographicSchemas();

            if (_debug)
            {
                Console.WriteLine(String.Format("GeographicSchemas.Build: {0:#,##0}s", stopwatch.Elapsed.TotalSeconds));
                stopwatch.Restart();
            }
            
            _builder.BuildTaxonProperties();

            if (_debug)
            {
                Console.WriteLine(String.Format("TaxonProperties.Build: {0:#,##0}s", stopwatch.Elapsed.TotalSeconds));
                stopwatch.Restart();
            }

            _builder.BuildTaxonRanks();

            if (_debug)
            {
                Console.WriteLine(String.Format("TaxonRanks.Build: {0:#,##0}s", stopwatch.Elapsed.TotalSeconds));
                stopwatch.Restart();
            }
            
            _builder.BuildVocabularies();

            if (_debug)
            {
                Console.WriteLine(String.Format("Vocabularies.Build: {0:#,##0}s", stopwatch.Elapsed.TotalSeconds));
                Console.WriteLine("Starting Names.Build...");
                stopwatch.Restart();
            }

            _builder.BuildNames();

            if (_debug)
            {
                Console.WriteLine(String.Format("Names.Index.Build: {0:#,##0}s", stopwatch.Elapsed.TotalSeconds));
                Console.WriteLine("Done");
            }
        }
    }
}
