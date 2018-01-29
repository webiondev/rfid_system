using System;
using System.Collections.Generic;
using System.Text;

namespace ErikEJ.SqlCe
{
    /// <summary>
    /// Options for bulk copy API
    /// </summary>
    [Flags]
    public enum SqlCeBulkCopyOptions
    {
        /// <summary>
        /// No options enabled
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Preserve source identity values. When not specified, identity values are assigned by the destination.
        /// This is implemented by using 'SET IDENTITY_INSERT [table] ON' when enabled
        /// </summary>
        KeepIdentity = 0x1,
        /// <summary>
        /// Preserve null values in the destination table regardless of the settings for default values. When not specified, null values are replaced by default values where applicable.
        /// </summary>
        KeepNulls = 0x8,
    }
}
