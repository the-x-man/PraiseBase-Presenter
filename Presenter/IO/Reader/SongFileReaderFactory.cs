﻿/*
 *   PraiseBase Presenter
 *   The open source lyrics and image projection software for churches
 *
 *   http://code.google.com/p/praisebasepresenter
 *
 *   This program is free software; you can redistribute it and/or
 *   modify it under the terms of the GNU General Public License
 *   as published by the Free Software Foundation; either version 2
 *   of the License, or (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program; if not, write to the Free Software
 *   Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 *   Author:
 *      Nicolas Perrenoud <nicu_at_lavine.ch>
 *   Co-authors:
 *      ...
 *
 */

using System;
using System.Collections.Generic;
using Pbp.Data.Song;
using System.Reflection;
using System.Linq;

namespace Pbp.IO
{
    public class SongFileReaderFactory
    {
        private Dictionary<Type, SongFileReader> readers = new Dictionary<Type,SongFileReader>();
        private Dictionary<String, HashSet<Type>> SupportedExtensionMapping = new Dictionary<string, HashSet<Type>>();

        public List<String> SupportedExtensions { get { return SupportedExtensionMapping.Keys.ToList(); } }

        /// <summary>
        /// Singleton variable
        /// </summary>
        private static SongFileReaderFactory _instance;

        /// <summary>
        /// Gets the singleton of this class (field alternative)
        /// </summary>
        public static SongFileReaderFactory Instance
        {
            get { return _instance ?? (_instance = new SongFileReaderFactory()); }
        }

        private SongFileReaderFactory() 
        {
            IEnumerable<Type> AllTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(SongFileReader)));
            foreach (var atype in AllTypes)
            {
                SongFileReader inst = (SongFileReader)Activator.CreateInstance(atype);
                readers.Add(atype, inst);
                if (! SupportedExtensionMapping.Keys.Contains(inst.FileExtension))
                {
                    SupportedExtensionMapping.Add(inst.FileExtension, new HashSet<Type>(new[] { atype }));
                }
                else
                {
                    SupportedExtensionMapping[inst.FileExtension].Add(atype);
                }
                Console.WriteLine("Loaded song reader: " + atype);
            }
        }

        public SongFileReader CreateFactory(Type type)
        {
            if (readers[type] != null)
            {
                return readers[type];
            }            
            throw new NotImplementedException();
        }

        public SongFileReader CreateFactoryByFile(string filename)
        {
            string ext = System.IO.Path.GetExtension(filename);

            if (SupportedExtensionMapping[ext] != null)
            {
                foreach (Type t in SupportedExtensionMapping[ext])
                {
                    return CreateFactory(t);
                }
            }
            throw new NotImplementedException();
        }

        public string GetFileBoxFilter()
        {
            String fltr = String.Empty;
            foreach (var t in readers.Values)
            {
                fltr += t.FileTypeDescription + " (*" + t.FileExtension + ")|*" + t.FileExtension + "|";
            }
            fltr += "Alle Dateien (*.*)|*.*";
            return fltr;
        }
    }
}