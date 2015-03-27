using log4net.Core;
using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Em.FundTrade.LogExtension.Log
{
    public class AccessLogMessagePatternConverter:PatternLayoutConverter
    {
        protected override void Convert(System.IO.TextWriter writer, log4net.Core.LoggingEvent loggingEvent)
        {
            if (Option != null)
            {
                // Write the value for the specified key
                WriteObject(writer, loggingEvent.Repository, LookupProperty(Option, loggingEvent));
            }
            else
            {
                // Write all the key value pairs
                WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            }
            //if (Option != null)
            //{
            //    // Write the value for the specified key
            //    WriteObject(writer, loggingEvent.Repository, loggingEvent.LookupProperty(Option));
            //}
            //else
            //{
            //    // Write all the key value pairs
            //    WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
            //}
        }
        private object LookupProperty(string property, LoggingEvent loggingEvent)
        {
            object propertyValue = string.Empty;
            PropertyInfo propertyInfo = loggingEvent.MessageObject.GetType().GetProperty(property);
            if (propertyInfo != null)
            {
                propertyValue = propertyInfo.GetValue(loggingEvent.MessageObject, null);
            }

            if (propertyInfo.PropertyType.Equals(typeof(System.DateTime)))
                return ((DateTime)propertyValue).ToString("yyyy/MM/dd HH:mm:ss.fff");
            else
                return propertyValue;
        }
    }

    
}
