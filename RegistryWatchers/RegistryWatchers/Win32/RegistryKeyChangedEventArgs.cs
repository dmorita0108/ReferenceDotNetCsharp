﻿using System;

namespace RegistryWatchers.Win32
{
    internal class RegistryKeyChangedEventArgs : EventArgs
    {
        internal string Hive { get; set; }
        internal string KeyPath { get; set; }

        internal RegistryKeyChangedEventArgs(string hive, string keyPath)
        {
            Hive = hive;
            KeyPath = keyPath;
        }
    }
}
