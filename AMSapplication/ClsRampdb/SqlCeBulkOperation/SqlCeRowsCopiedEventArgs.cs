using System;
using System.Collections.Generic;
using System.Text;

namespace ErikEJ.SqlCe
{
    public class SqlCeRowsCopiedEventArgs : EventArgs
    {
        private long _rowsCopied;
        private bool _abort;

        public SqlCeRowsCopiedEventArgs(long rowsCopied)
        {
            this._rowsCopied = rowsCopied;
        }

        public long RowsCopied
        {
            get
            {
                return this._rowsCopied;
            }
        }

        public bool Abort
        {
            get
            {
                return this._abort;
            }
            set
            {
                this._abort = value;
            }
        }

    }

}
