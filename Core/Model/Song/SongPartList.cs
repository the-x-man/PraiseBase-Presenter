﻿/*
 *   PraiseBase Presenter
 *   The open source lyrics and image projection software for churches
 *
 *   http://praisebase.org
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
 */

using System.Collections.Generic;

namespace PraiseBase.Presenter.Model.Song
{
    /// <summary>
    ///     Provides a list of all parts in the song
    /// </summary>
    public class SongPartList : List<SongPart>
    {
        /// <summary>
        ///     Swaps the part with the previous one
        /// </summary>
        /// <param name="partId">Index of the part</param>
        /// <returns></returns>
        public bool SwapWithUpper(int partId)
        {
            if (partId > 0 && partId < Count)
            {
                var tmpPrt = this[partId - 1];
                RemoveAt(partId - 1);
                Insert(partId, tmpPrt);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Swaps the part with the next one
        /// </summary>
        /// <param name="partId">Index of the part</param>
        /// <returns></returns>
        public bool SwapWithLower(int partId)
        {
            if (partId >= 0 && partId < Count - 1)
            {
                var tmpPrt = this[partId + 1];
                RemoveAt(partId + 1);
                Insert(partId, tmpPrt);
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 19;
                for (var i = 0; i < Count; i++)
                {
                    hash = hash*31 + this[i].GetHashCode();
                }
                return hash;
            }
        }

        protected bool Equals(SongPartList other)
        {
            if (Count != other.Count) return false;
            for (var i = 0; i < Count; i++)
            {
                if (!Equals(this[i], other[i])) return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SongPartList) obj);
        }
    }
}