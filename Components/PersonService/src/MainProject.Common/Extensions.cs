using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace MainProject.Common
{
    public static class Extensions
    {
        //public static string RemoveLineEndings(this string value)
        //{
        //    return Regex.Replace(value, @"[\u000A\u000B\u000C\u000D\u2028\u2029\u0085]+", string.Empty);
        //}

        //public static string[] GetErrors(this Exception exception)
        //{
        //    var ex = exception;
        //    var errors = new List<string>();

        //    do
        //    {
        //        errors.Add(ex.ToString());
        //        ex = ex.InnerException;
        //    }
        //    while (ex != null);

        //    return errors.ToArray();
        //}

        //public static string FlattenErrors(this string[] errors)
        //{
        //    return errors.Aggregate(string.Empty, (p, n) => $"{p}{n}\n");
        //}

        //public static void LogError<T>(this ILogger<T> logger, Exception e)
        //{
        //    var exception = e;

        //    switch (e)
        //    {
        //        case AggregateException aggregateException:
        //            exception = aggregateException.Flatten();
        //            break;

        //        case TaskCanceledException taskCancelledException when taskCancelledException.InnerException != null:
        //            exception = taskCancelledException.InnerException;
        //            break;
        //    }

        //    logger.LogError(exception.ToString());
        //}

        public static T FromXmlString<T>(this string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                throw new ArgumentNullException(nameof(xml));
            }

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                return FromXmlStream<T>(stream);
            }
        }

        public static T FromXmlStream<T>(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var serializer = GetXmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }

        public static string ToXmlString<T>(this T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var serializer = GetXmlSerializer(typeof(T));

            var sb = new StringBuilder();
            using (var w = XmlWriter.Create(sb, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                serializer.Serialize(w, obj);
                return sb.ToString();
            }
        }

        //public static string ToDelimitedString(this string[] items, char delimiter = ',')
        //{
        //    return string.Join($"{delimiter}", items);
        //}

        //public static void ToXmlStream(this object @object, Stream stream)
        //{
        //    var serializer = GetXmlSerializer(@object.GetType());
        //    serializer.Serialize(stream, @object);
        //    stream.Flush();
        //}

        public static string ToJsonString<T>(this T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return JsonConvert.SerializeObject(obj);
        }

        //public static void ToJsonStream(this object @object, Stream stream)
        //{
        //    var serializer = new JsonSerializer();
        //    serializer.Serialize(new StreamWriter(stream), @object);
        //    stream.Flush();
        //}

        public static T FromJsonString<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        //public static async Task WithHeartbeat(this Task primaryTask, TimeSpan heartbeatInterval, Func<CancellationToken, Task> heartbeatTaskFactory, CancellationToken cancellationToken)
        //{
        //    var stopHeartbeatSource = new CancellationTokenSource();
        //    cancellationToken.Register(stopHeartbeatSource.Cancel);

        //    await Task.WhenAll(
        //        primaryTask,
        //        PerformHeartbeats(heartbeatInterval, heartbeatTaskFactory, stopHeartbeatSource.Token));

        //    if (!stopHeartbeatSource.IsCancellationRequested)
        //    {
        //        stopHeartbeatSource.Cancel();
        //    }
        //}

        //public static IEnumerable<IEnumerable<T>> GetBatches<T>(this IEnumerable<T> list, int batchSize)
        //{
        //    return list.Select((item, index) => new { item, index }).GroupBy(x => x.index / batchSize, x => x.item);
        //}

        //private static async Task PerformHeartbeats(TimeSpan interval, Func<CancellationToken, Task> heartbeatTaskFactory, CancellationToken cancellationToken)
        //{
        //    while (!cancellationToken.IsCancellationRequested)
        //    {
        //        try
        //        {
        //            if (!cancellationToken.IsCancellationRequested)
        //            {
        //                await heartbeatTaskFactory(cancellationToken);
        //            }
        //            cancellationToken.WaitHandle.WaitOne(interval);
        //        }
        //        catch
        //        {
        //            break;
        //        }
        //    }
        //}

        private static XmlSerializer GetXmlSerializer(Type type)
        {
            var serializer = new XmlSerializer(type);

            return serializer;
        }
    }
}
