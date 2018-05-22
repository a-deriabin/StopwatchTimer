using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopwatchTimer
{
    static class SavedData
    {
        private static Properties.Settings settings =
            Properties.Settings.Default;

        public static bool IsTopmost
        {
            get
            {
                return (bool)settings["IsTopmost"];
            }

            set
            {
                settings["IsTopmost"] = value;
                settings.Save();
            }
        }

        /// <summary>
        /// Returns all saved time values. The first value should be topmost in list.
        /// </summary>
        public static string[] GetAllSavedTimes()
        {
            var items = (StringCollection)settings["SavedTimes"];
            if (items == null)
                return new string[0];

            // Convert StringCollection to an array
            string[] arr = new string[items.Count];
            items.CopyTo(arr, 0);

            return arr;
        }

        /// <summary>
        /// Adds an element to the top of the saves list.
        /// </summary>
        public static void AddSavedTime(string timeStr)
        {
            var items = (StringCollection)settings["SavedTimes"];
            if (items == null)
            {
                settings["SavedTimes"] = new StringCollection();
                AddSavedTime(timeStr);
                return;
            }

            items.Insert(0, timeStr);
            settings.Save();
        }

        /// <param name="index">A top-based index, starting from 0.</param>
        public static void RemoveSavedTime(int index)
        {
            var items = (StringCollection)settings["SavedTimes"];
            if (items == null)
                return;

            items.RemoveAt(index);
            settings.Save();
        }

    }
}
