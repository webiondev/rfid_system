using System;
using System.Collections.Generic;
using System.Text;

namespace ErikEJ.SqlCe
{
    public sealed class SqlCeBulkCopyColumnMappingCollection : List<SqlCeBulkCopyColumnMapping>
    {
        internal void ValidateCollection(System.Data.SqlServerCe.SqlCeUpdatableRecord rec, System.Data.DataColumnCollection dataColumnCollection)
        {
            // If no user defined mappings
            if (this.Count == 0)
            { 
                            
            }

        }
    }
}
