using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceLayerApi.Common
{
    public static class TaskExtensions
    {
        public static Task<T> ToTask<T>(this T value) => Task.FromResult(value);
    }
}