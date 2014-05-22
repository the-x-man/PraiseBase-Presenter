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
using System.Linq;
using System.Text;
using System.Xml;
using Pbp.Data.Statistics;

namespace Pbp.IO.Writer
{
    class StatisticsWriter
    {
        public void Write(string filename, Statistics stat)
        {
            XmlWriterHelper xml = new XmlWriterHelper("statistics", "1.0");

            XmlElement node;
            foreach (var date in stat.Dates)
            {
                node = xml.Doc.CreateElement("date");
                node.SetAttribute("year", date.Value.Year.ToString());
                node.SetAttribute("month", date.Value.Month.ToString());
                node.SetAttribute("day", date.Value.Day.ToString());
                XmlNode dateNode = xml.Root.AppendChild(node);
                foreach (var item in date.Value.Items)
                {
                    if (item.Value.Type == StatisticsItemType.Song)
                    {
                        node = xml.Doc.CreateElement("song");
                        node.SetAttribute("title", item.Value.Title);
                        node.SetAttribute("copyright", item.Value.Copyright);
                        node.SetAttribute("ccli", item.Value.CcliID);
                        node.SetAttribute("count", item.Value.Count.ToString());
                        dateNode.AppendChild(node);
                    }
                }
            }

            xml.Write(filename);
        }
    }
}
