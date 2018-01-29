using System;
using System.Collections.Generic;
using System.Text;

namespace ErikEJ.SqlCe
{
    public sealed class SqlCeBulkCopyColumnMapping
    {
        // Fields
        internal string _destinationColumnName;
        internal int _destinationColumnOrdinal;
        internal string _sourceColumnName;
        internal int _sourceColumnOrdinal;

        // Methods
        public SqlCeBulkCopyColumnMapping()
        {
        }

        public SqlCeBulkCopyColumnMapping(int sourceColumnOrdinal, int destinationOrdinal)
        {
            this.SourceOrdinal = sourceColumnOrdinal;
            this.DestinationOrdinal = destinationOrdinal;
        }

        public SqlCeBulkCopyColumnMapping(int sourceColumnOrdinal, string destinationColumn)
        {
            this.SourceOrdinal = sourceColumnOrdinal;
            this.DestinationColumn = destinationColumn;
        }

        public SqlCeBulkCopyColumnMapping(string sourceColumn, int destinationOrdinal)
        {
            this.SourceColumn = sourceColumn;
            this.DestinationOrdinal = destinationOrdinal;
        }

        public SqlCeBulkCopyColumnMapping(string sourceColumn, string destinationColumn)
        {
            this.SourceColumn = sourceColumn;
            this.DestinationColumn = destinationColumn;
        }

        // Properties
        public string DestinationColumn
        {
            get
            {
                if (this._destinationColumnName != null)
                {
                    return this._destinationColumnName;
                }
                return string.Empty;
            }
            set
            {
                this._destinationColumnOrdinal = -1;
                this._destinationColumnName = value;
            }
        }

        public int DestinationOrdinal
        {
            get
            {
                return this._destinationColumnOrdinal;
            }
            set
            {
                if (value < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                this._destinationColumnName = null;
                this._destinationColumnOrdinal = value;
            }
        }

        public string SourceColumn
        {
            get
            {
                if (this._sourceColumnName != null)
                {
                    return this._sourceColumnName;
                }
                return string.Empty;
            }
            set
            {
                this._sourceColumnOrdinal = -1;
                this._sourceColumnName = value;
            }
        }

        public int SourceOrdinal
        {
            get
            {
                return this._sourceColumnOrdinal;
            }
            set
            {
                if (value < 0)
                {
                    throw new IndexOutOfRangeException();
                }
                this._sourceColumnName = null;
                this._sourceColumnOrdinal = value;
            }
        }
    }

}
