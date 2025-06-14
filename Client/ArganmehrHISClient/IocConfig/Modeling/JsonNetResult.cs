﻿using Newtonsoft.Json;
using ServiceLayer.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Helpers.Extentions;
using Newtonsoft.Json.Converters;

namespace IocConfig.Modeling
{
    public class JsonNetResult : JsonResult
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly JsonSerializerSettings _settings;

        public JsonNetResult(IDateTimeHelper dateTimeHelper)
            : this(dateTimeHelper, null)
        {
        }

        public JsonNetResult(IDateTimeHelper dateTimeHelper, JsonSerializerSettings settings)
        {
            

            _dateTimeHelper = dateTimeHelper;
            _settings = settings;
        }

        public override void ExecuteResult(ControllerContext context)
        {
          

            if (this.Data == null)
                return;

            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && context.HttpContext.Request.HttpMethod.IsCaseInsensitiveEqual("GET"))
            {
                throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request.To allow GET requests, set JsonRequestBehavior to AllowGet.");
            }

            var response = context.HttpContext.Response;

            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            response.ContentType = this.ContentType.NullEmpty() ?? "application/json";

            var serializerSettings = _settings ?? new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,

                // Limit the object graph we'll consume to a fixed depth. This prevents stackoverflow exceptions
                // from deserialization errors that might occur from deeply nested objects.
                MaxDepth = 32,

                // Do not change this setting
                // Setting this to None prevents Json.NET from loading malicious, unsafe, or security-sensitive types
                TypeNameHandling = TypeNameHandling.None
            };

            if (_settings == null)
            {
                serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
                serializerSettings.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;

                var utcDateTimeConverter = new UTCDateTimeConverter(_dateTimeHelper, new JavaScriptDateTimeConverter());
                serializerSettings.Converters.Add(utcDateTimeConverter);
            }

            using (var jsonWriter = new JsonTextWriter(response.Output))
            {
                jsonWriter.CloseOutput = false;
                var jsonSerializer = JsonSerializer.Create(serializerSettings);
                jsonSerializer.Serialize(jsonWriter, this.Data);
            }
        }

        class UTCDateTimeConverter : DateTimeConverterBase
        {
            private readonly IDateTimeHelper _dateTimeHelper;
            private readonly DateTimeConverterBase _innerConverter;

            public UTCDateTimeConverter(IDateTimeHelper dateTimeHelper, DateTimeConverterBase innerConverter)
            {
               

                _dateTimeHelper = dateTimeHelper;
                _innerConverter = innerConverter;
            }

            public override bool CanConvert(Type objectType)
            {
                return _innerConverter.CanConvert(objectType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return _innerConverter.ReadJson(reader, objectType, existingValue, serializer);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value is DateTime)
                {
                    var d = (DateTime)value;
                    if (d.Kind == DateTimeKind.Unspecified)
                    {
                        // when DateTime kind is "Unspecified", it was very likely converted from UTC to 
                        // SERVER*s preferred local time before (with DateTimeHelper.ConvertToUserTime()).
                        // While this works fine during server-time rendering, it can lead to wrong UTC offsets
                        // on the client (e.g. in AJAX mode Grids, where rendering is performed locally with JSON data).
                        // The issue occurs when the client's time zone is not the same as "CurrentTimeZone" (configured in the backend).
                        // To fix it, we have to convert the date back to UTC kind, but with the SERVER PREFERRED TIMEZONE
                        // in order to calculate with the correct UTC offset. Then it's up to the client to display the date
                        // in the CLIENT's time zone. Which is not perfect of course, because the same date would be displayed in the 
                        // "CurrentTimeZone" if rendered on server.
                        // But: it fixes the issue and is way better than converting all AJAXable dates to strings on the server.
                        value = _dateTimeHelper.ConvertToUtcTime(d, _dateTimeHelper.CurrentTimeZone);
                    }
                }

                _innerConverter.WriteJson(writer, value, serializer);
            }
        }
    }
}
