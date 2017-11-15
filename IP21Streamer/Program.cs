using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using IP21Streamer.Application;
using System.Threading;
using IP21Streamer.Source.UaSource;
using IP21Streamer.Source.UaSource.IP21;
using UnifiedAutomation.UaBase;
using IP21Streamer.Source.IP21;
using System.Data.SqlClient;
using System.Data.Linq;
using IP21Streamer.Business;

namespace IP21Streamer
{
    class Program
    {
        static App application;

        private static string _connString;
        private static SqlConnection _connection;
        private static DataContext _dbContext;

        private static Table<TagItem> _tagItems;

        static ILog log;

        static void Main(string[] args)
        {
            // Create Logger Object
            log4net.Config.XmlConfigurator.Configure();
            log = LogManager.GetLogger("Main");

            application = new App();

            IP21Source source = new IP21Source(ApplicationInstance.Default);

            source.Connect("opc.tcp://mo-tw08:63500/InfoPlus21/OpcUa/Server", "statoil-net\\bomu", "9oyU6gof");

            initializeRepository();

            if (App.Settings.UpdateDBModel)
            {
                List<IP21Tag> tagNodes = source.GetUpdatedModel();

                List<TagItem> foundTags = new List<TagItem>();
                foundTags.FillWith(tagNodes);

                
                foreach (var foundTag in foundTags)
                {
                    // check if exists in DB
                    var results = from tags in _tagItems
                                  where tags.Tag == foundTag.Tag && tags.SAPCode == foundTag.SAPCode
                                  select tags;

                    if (results.Count() == 0)
                    {
                        _tagItems.InsertOnSubmit(foundTag);
                    }
                    else
                    {
                        foreach (var tag in results)
                        {
                            // update meta data
                            tag.Description = foundTag.Description;
                            tag.EngUnitID = foundTag.EngUnitID;
                            tag.EngUnits = foundTag.EngUnits;
                            tag.EURangeLow = foundTag.EURangeLow;
                            tag.EURangeHigh = foundTag.EURangeHigh;
                            tag.TagNodeID = foundTag.TagNodeID;
                            tag.MeasurementNodeID = foundTag.MeasurementNodeID;
                        }
                    }
                }

                _dbContext.SubmitChanges();

            }


            Console.WriteLine("Press enter to exit program...");
            Console.ReadLine();

        }

        private static void initializeRepository()
        {
            _connString = App.Settings.UaTagsDBConnString;
            _connection = new SqlConnection(_connString);

            _dbContext = new DataContext(_connString);
            _tagItems = _dbContext.GetTable<TagItem>();
            
        }
    }
}
