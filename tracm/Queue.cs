using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace tracm
{
    interface QueueDisplay
    {
        string Title
        {
            get;
            set;
        }
        string PercentComplete
        {
            get;
            set;
        }
    }

    class QueueItem : QueueDisplay
    {
        private string m_title = "";
        private string m_filePath = "";
        private double m_percentComplete = 0;
        private int m_content_id = 0;
        private bool m_processing = false;
        public event ListChangedEventHandler ItemChanged;

        public string Title
        {
            get
            { return m_title; }
            set
            { m_title = value; }
        }

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
            {
                if(m_processing)
                    return m_percentComplete.ToString("0%");
                else
                    return "Queued"; 
            }
            set
            {
                if(double.Parse(value) != m_percentComplete)
                    ItemChanged(this, new ListChangedEventArgs(ListChangedType.ItemChanged, 0));
                m_percentComplete = double.Parse(value);
           }
        }

        public int Content_ID
        {
            get
            { return m_content_id; }
            set
            { m_content_id = value; }
        }

        public bool Processing
        {
            get
            { return m_processing; }
            set
            { m_processing = value; }
        }
    }

    class Queue : BindingList<QueueDisplay>
    {
        public void Add(QueueItem Item)
        {
            base.Add(Item);
            Item.ItemChanged += new ListChangedEventHandler(Item_ItemChanged);
        }

        void Item_ItemChanged(object sender, ListChangedEventArgs e)
        {
            int index = base.IndexOf((QueueItem)sender);
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
        }

        public bool Content_ID_Exists(int Content_ID)
        {
            foreach (QueueItem item in this)
            {
                if (item.Content_ID == Content_ID)
                    return true;
            }
            return false;
        }
    }
}
