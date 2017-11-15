using IP21Streamer.Application;
using IP21Streamer.Business;
using IP21Streamer.Source.IP21;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP21Streamer.Repository
{
    public static class Extensions
    {
        internal static void FillWith(this List<TagItem> destTags, List<IP21Tag> sourceTags)
        {
            var sourceEnum = sourceTags.GetEnumerator();

            while (sourceEnum.MoveNext())
            {
                var newTag = new TagItem
                {
                    SAPCode = App.Settings.SAPCode,
                    STIDCode = App.Settings.STIDCode,
                    Tag = sourceEnum.Current.DisplayName,
                    Description = sourceEnum.Current.Measurement.Description,
                    EngUnitID = sourceEnum.Current.Measurement.EngineeringUnits.UnitId,
                    EngUnits = sourceEnum.Current.Measurement.EngineeringUnits.DisplayName.Text,
                    EURangeLow = sourceEnum.Current.Measurement.EuRange.Low,
                    EURangeHigh = sourceEnum.Current.Measurement.EuRange.High,
                    Subscribe = 0,
                    TagNodeID = Convert.ToInt32(sourceEnum.Current.NodeId.Identifier),
                    MeasurementNodeID = sourceEnum.Current.Measurement.NodeId.Identifier as byte[]
                };

                destTags.Add(newTag);
            }
        }
    }
}
