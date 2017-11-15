using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using log4net;

namespace IP21Streamer.Business
{
    [Table(Name = "UATAGS")]
    public class TagItem
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TagItem));

        private int _id;
        [Column(IsPrimaryKey = true, Storage = "_id", AutoSync = AutoSync.OnInsert, IsDbGenerated = true)]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private int _sapCode;
        [Column(Storage = "_sapCode")]
        public int SAPCode
        {
            get { return _sapCode; }
            set { _sapCode = value; }
        }

        private string _stidCode;
        [Column(Storage = "_stidCode")]
        public string STIDCode
        {
            get { return _stidCode; }
            set { _stidCode = value; }
        }

        private string _tag;
        [Column(Storage = "_tag")]
        public string Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        private string _description;
        [Column(Storage = "_description")]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private int _engUnitId;
        [Column(Storage = "_engUnitId")]
        public int EngUnitID
        {
            get { return _engUnitId; }
            set { _engUnitId = value; }
        }

        private string _engUnit;
        [Column(Storage = "_engUnit")]
        public string EngUnits
        {
            get { return _engUnit; }
            set { _engUnit = value; }
        }

        private double _euRangeLow;
        [Column(Storage = "_euRangeLow")]
        public double EURangeLow
        {
            get { return _euRangeLow; }
            set { _euRangeLow = value; }
        }

        private double _euRangeHigh;
        [Column(Storage = "_euRangeHigh")]
        public double EURangeHigh
        {
            get { return _euRangeHigh; }
            set { _euRangeHigh = value; }
        }

        private int _subscribe;
        [Column(Storage = "_subscribe")]
        public int Subscribe
        {
            get { return _subscribe; }
            set { _subscribe = value; }
        }

        private int _tagNodeId;
        [Column(Storage = "_tagNodeId")]
        public int TagNodeID
        {
            get { return _tagNodeId; }
            set { _tagNodeId = value; }
        }

        private byte[] _measurementNodeId;
        [Column(Storage = "_measurementNodeId")]
        public byte[] MeasurementNodeID
        {
            get { return _measurementNodeId; }
            set { _measurementNodeId = value; }
        }

    }
}
