using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangLang.FormTable
{
    public class TableItemAttribute : Attribute
    {
        public int ColumnOrder { get; }
        public TableItemAttribute(int columnOrder) {
            ColumnOrder = columnOrder;
        }
    }
}
