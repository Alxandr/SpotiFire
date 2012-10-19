using System;

namespace SpotiFire
{
    public class DescriptionEventArgs : EventArgs
    {
        private string description;
        public DescriptionEventArgs(string description)
        {
            this.description = description;
        }

        public string Description
        {
            get { return this.description; }
        }

    }
}
