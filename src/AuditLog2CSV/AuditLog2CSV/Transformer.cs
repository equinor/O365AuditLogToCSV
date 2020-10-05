using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Text;
using AuditLog2CSV.Data;
using Newtonsoft.Json;
using NLog;

namespace AuditLog2CSV {
    internal class Transformer {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Transform Audit Log to CSV file only
        /// </summary>
        /// <param name="inputFile">Input file and path to transform as string</param>
        internal void TransformFileToCSV(string inputFile) {
            try {
                logger.Info("Loading audit log items from {0}", inputFile);
                var auditLogItems = CsvCollection<AuditLogItem>.CreateFromFile(inputFile);

                logger.Trace("Creating CSV collection for all records");
                var csvCollection = new CsvCollection<dynamic>();
                int error = 0, success = 0;

                foreach (AuditLogItem item in auditLogItems) {
                    try {
                        logger.Trace("Creating dynamic object with information from item with creation date {0}", item.CreationDate);
                        dynamic newCsvItem = GetDynamicObjectFromAuditLogItem(item);

                        csvCollection.Add(newCsvItem);
                        success += 1;
                    } catch (Exception e) {
                        logger.Warn(e, "Unable to transform record with creation date {0}", item.CreationDate);
                        error += 1;
                    }
                }
                logger.Info("Transforming items completed with:");
                logger.Info("Succesed items: {0} | Failed items: {1} | Total items: {2}", success, error, success + error);

                string saveFilePath = Global.Config.ExportFolder;
                if (!saveFilePath.EndsWith('\\')) saveFilePath = saveFilePath + "\\";
                saveFilePath = saveFilePath + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".csv";
                csvCollection.SaveToCsvFile(saveFilePath);
            } catch (Exception e) {
                logger.Error(e, "Transform to csv failed");
            }
            finally {
                logger.Info("Transformer finished task");

            }
        }

        /// <summary>
        /// Transform Audit Log to CSV file only
        /// </summary>
        /// <param name="inputFile">Input file and path to transform as string</param>
        internal void TransformFileToJson(string inputFile) {
            try {
                logger.Info("Loading audit log items from {0}", inputFile);
                var auditLogItems = CsvCollection<AuditLogItem>.CreateFromFile(inputFile);

                logger.Trace("Creating CSV collection for all records");
                var collection = new List<dynamic>();
                int error = 0, success = 0;

                foreach (AuditLogItem item in auditLogItems) {
                    try {
                        logger.Trace("Creating dynamic object with information from item with creation date {0}", item.CreationDate);
                        dynamic newJsonItem = GetDynamicObjectFromAuditLogItem(item);

                        collection.Add(newJsonItem);
                        success += 1;
                    } catch (Exception e) {
                        logger.Warn(e, "Unable to transform record with creation date {0}", item.CreationDate);
                        error += 1;
                    } finally {
                        logger.Info("Transforming items completed with:");
                        logger.Info("Succesed items: {0} | Failed items: {1} | Total items: {2}", success, error, success + error);
                    }
                }
                string saveFilePath = Global.Config.ExportFolder;
                if (!saveFilePath.EndsWith('\\')) saveFilePath = saveFilePath + "\\";
                saveFilePath = saveFilePath + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".csv";

                using (StreamWriter file = File.CreateText(saveFilePath)) {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, collection);
                }
            } catch (Exception e) {
                logger.Error(e, "Transform to json failed");
            }
            finally {
                logger.Info("Transformer finished task");

            }


        }

        private dynamic GetDynamicObjectFromAuditLogItem(AuditLogItem item) {
            dynamic newCsvItem = new ExpandoObject();
            newCsvItem.CreationDate = item.CreationDate;
            newCsvItem.UserIds = item.UserIds;
            newCsvItem.Operations = item.Operations;

            logger.Trace("Transforming json data into the data object");
            JsonConvert.PopulateObject(item.AuditData, newCsvItem);

            return newCsvItem;
        }
    }
}
