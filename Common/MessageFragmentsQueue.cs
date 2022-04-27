using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MessageQueue<T, N>
    {
        readonly ConcurrentDictionary<T, ConcurrentBag<N>> queue = new ConcurrentDictionary<T, ConcurrentBag<N>>();
        public void Add(T key, N value)
        {
            if (!queue.ContainsKey(key))
                queue.TryAdd(key, new ConcurrentBag<N>());
            queue[key].Add(value);
        }
        public string GetAllMessages(T key, bool trimNewLine = true)
        {
            if (!queue.ContainsKey(key)) return $"{key} is not present in dictionary";
            return string.Join(
                "",
                queue[key].Select(x =>
                    {
                        return trimNewLine ? x.ToString().TrimEnd('\n') : x.ToString();
                    }
                )
            );
        }
        public void RemoveAllMessages(T key)
        {
            if (!queue.ContainsKey(key)) return;

            queue.TryRemove(key, out _);
        }
    }
}
