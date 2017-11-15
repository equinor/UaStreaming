using IP21Streamer.Application;
using IP21Streamer.Extensions;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP21Streamer.Repository
{
    class MetaDataStore : IRepository
    {
        private const int BATCH_SIZE = 200;

        Settings _settings;

        private string _connString;
        private SqlConnection _connection;
        private DataContext _dbContext;
        private Table<TagItem> _tagItems;

        private static ILog log = LogManager.GetLogger(typeof(MetaDataStore));

        #region Constructor
        public MetaDataStore(string connString)
        {
            _connString = connString;
        }
        #endregion

        #region Connection
        public void Initialize()
        {
            _connection = new SqlConnection(_connString);
            _dbContext = new DataContext(_connString);

            _tagItems = _dbContext.GetTable<TagItem>();
        }

        public void Dispose()
        {
            _connection.Dispose();
            _dbContext.Dispose();
        }
        #endregion

        #region Update Meta Data
        public void UpdateMetaDataWith(List<TagItem> foundTagItems)
        {
            int count = 0;

            while (foundTagItems.Any())
            {
                var tagBatch = foundTagItems.Dequeue<TagItem>(BATCH_SIZE);
                UpdateMetaDataBatch(tagBatch);

                count += tagBatch.Count;
                log.Debug($"Updated metadata of {count} tags");
            }
        }

        private void UpdateMetaDataBatch(List<TagItem> foundTagItems)
        {
            foreach (var foundTag in foundTagItems)
            {
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
        #endregion
    }
}
