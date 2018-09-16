using AspenStreamer.KDI;
using Common;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaClient;

namespace UaStreamer.Extensions
{
    internal static class ModelExtensions
    {
        public static void FillWith(this EventVqt @event, NodeData data, DataChange change)
        {
            @event.tag = data.Tag;
            @event.time = change.Value.SourceTimestamp;
            @event.status = TranslateStatus(change.Value.StatusCode);
            @event.value = ParseValue(change.Value.Value, ref @event.status);
        }

        public static void KSpiceFillWith(this EventVqt @event, KSpiceVariableData data, DataChange change)
        {
            @event.tag = data.tagName;
            @event.time = change.Value.SourceTimestamp;
            @event.status = TranslateStatus(change.Value.StatusCode);
            @event.value = ParseValue(change.Value.Value, ref @event.status);
        }

        private static double ParseValue(object value, ref int status)
        {
            if (!double.TryParse(value.ToString(), out double result))
            {
                status = Constants.Bad;
                return -1;
            }

            return result;
        }

        private static int TranslateStatus(StatusCode statusCode)
        {
            // https://techsupport.osisoft.com/Documentation/PI-AF-SDK/html/T_OSIsoft_AF_Asset_AFValueStatus.htm
            if (statusCode.IsGood())
                return Constants.Good;
            else if (statusCode.IsBad())
                return Constants.Bad;
            else
                return Constants.Questionable;
        }
    }
}
