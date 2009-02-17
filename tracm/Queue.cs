using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace tracm
{
    class QueueItem
    {
        private string m_filePath = "";
        private double m_percentComplete = 0;

        public string FilePath
        {
            get
            {return m_filePath;}
            set
            {m_filePath = value;}
        }

        public string PercentComplete
        {
            get
            { return m_percentComplete.ToString(); }
            set
            {m_percentComplete = double.Parse(value);}
        }
    }

    class Queue : BindingList<QueueItem>
    {
    }
}
