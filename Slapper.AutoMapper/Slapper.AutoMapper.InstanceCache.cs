using System.Collections.Generic;

namespace Slapper
{
    public static partial class AutoMapper
    {
        /// <summary>
        /// Identity map used to merge rows that refer to the same instance (type and identifiers).
        /// Each mapping call uses its own cache unless an instance of this class is passed explicitly,
        /// in which case results are merged across the calls that share it.
        /// This class is not thread-safe: do not share an instance between concurrent mapping calls.
        /// </summary>
        public sealed class InstanceCache
        {
            internal Dictionary<InternalHelpers.InstanceKey, object> Items { get; } = new Dictionary<InternalHelpers.InstanceKey, object>();

            /// <summary>
            /// Removes all instances from this cache.
            /// </summary>
            public void Clear()
            {
                Items.Clear();
            }
        }
    }
}
