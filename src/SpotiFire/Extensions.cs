using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Serilog;

namespace SpotiFire
{
    internal static class Extensions
    {
        public delegate void ArgumentsDelegate(params object[] args);

        public static T AddAndReturn<T>(this IList<T> list, T item)
        {
            list.Add(item);
            return item;
        }

        public static void EnsureSuccess(this sp_error error)
        {
            if (error != sp_error.OK)
                throw new SpotifyException(error);
        }

        public static ArgumentsDelegate LogMethod(
            this ILogger logger,
            [CallerMemberName] string name = "")
        {
            return args =>
                logger.Debug("Method {method} called with args: {args}", name, args);
        }
    }
}