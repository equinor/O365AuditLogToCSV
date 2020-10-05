using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using NLog;

namespace AuditLog2CSV.Data {
    internal class CsvCollection<T> : List<T> {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Default CSV file name for collection
        /// </summary>
        public string DefaultFileName { get; protected set; }
        /// <summary>
        /// Delimiter used when saving CSV collection to file
        /// </summary>
        internal string SaveDelimiter { get; set; } = Global.Config.CsvSaveDelimiter;
        /// <summary>
        /// Delimiter used when reading CSV file to collection
        /// </summary>
        internal string ReadDelimiter { get; set; } = Global.Config.AuditLogCsvDelimiter;
        /// <summary>
        /// Ignore quotes while reading the CSV file record
        /// </summary>
        internal bool IgnoreQuotesWhileReading = false;

        /// <summary>
        /// Save Collection to CSV file
        /// </summary>
        /// <param name="filename">filename of csv file as string</param>
        internal void SaveToCsvFile(string filename = "") {
            try {
                logger.Trace("Entering SaveToCsvFile()");
                logger.Info("Saving {0} to file: {1}", this.GetType().Name, filename);
                if (String.IsNullOrEmpty(filename)) filename = DefaultFileName;

                using (var writer = new StreamWriter(filename)) {
                    CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture) {
                        MissingFieldFound = null,
                        Delimiter = SaveDelimiter,
                        SanitizeForInjection = Global.Config.UseCsvSanitizeForInjection
                    };
                    using (var csv = new CsvWriter(writer, csvConfig)) {
                        csv.WriteRecords(this);
                    }
                }
            } catch (Exception e) {
                logger.Error(e, $"Unable to save {this.GetType().Name} to file");
                throw;
            } finally {
                logger.Trace("Leaving SaveToCsvFile()");
            }
        }

        /// <summary>
        /// Create collection from file
        /// </summary>
        /// <param name="filename">Filename or path as string</param>
        /// <returns>Items as CsvCollection T</returns>
        internal static CsvCollection<T> CreateFromFile(string filename) {
            var collection = new CsvCollection<T>();
            collection.AddRecordsFromCsvFile(filename);

            return collection;
        }


        /// <summary>
        /// Add records from CSV file into collection
        /// </summary>
        /// <param name="filename">filename as string</param>
        protected void AddRecordsFromCsvFile(string filename = "") {
            try {
                logger.Trace("Entering AddRecordsFromCsvFile()");
                logger.Info("Getting {0} from file: {1}", this.GetType().Name, filename);
                if (String.IsNullOrEmpty(filename)) filename = DefaultFileName;

                using (var reader = new StreamReader(filename)) {
                    CsvConfiguration csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture) {
                        Delimiter = ReadDelimiter,
                        MissingFieldFound = null,
                        BadDataFound = x => { logger.Warn("Bad data found in CSV file {0}: {1}>", filename, x.RawRecord); },
                        IgnoreQuotes = IgnoreQuotesWhileReading
                    };
                    using (var csv = new CsvReader(reader, csvConfig)) {
                        var records = csv.GetRecords<T>().ToList();
                        logger.Trace("Adding csv records from CSV file into collection");
                        this.AddRange(records);
                    }
                }
            } catch (Exception e) {
                logger.Error(e, "Unable to read csv file <{0}> into {1}", filename, this.GetType().Name);
                throw;
            } finally {
                logger.Trace("Leaving AddRecordsFromCsvFile()");
            }
        }
    }
}
