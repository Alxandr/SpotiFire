﻿using System;

namespace SpotiFire.SpotifyLib
{
    public class DescriptionEventArgs : EventArgs
    {
        private string description;
        public DescriptionEventArgs(string description)
        {
            this.description = description;
        }
    }
}
