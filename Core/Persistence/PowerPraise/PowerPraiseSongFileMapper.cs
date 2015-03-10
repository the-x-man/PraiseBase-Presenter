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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PraiseBase.Presenter.Model;
using PraiseBase.Presenter.Model.Song;

namespace PraiseBase.Presenter.Persistence.PowerPraise
{
    public class PowerPraiseSongFileMapper : ISongFileMapper<PowerPraiseSong>
    {
        /// <summary>
        /// Maps a PowerPraise song to a Song object
        /// </summary>
        /// <param name="ppl"></param>
        /// <returns></returns>
        public Song Map(PowerPraiseSong ppl)
        {
            Song song = new Song
            {
                Title = ppl.Title, 
                Language = ppl.Language
            };

            song.Themes.Add(ppl.Category);

            // Copyright text
            song.Copyright = String.Join(Environment.NewLine, ppl.CopyrightText.ToArray());
            switch (ppl.CopyrightTextPosition)
            {
                case PowerPraiseSong.CopyrightPosition.FirstSlide:
                    song.CopyrightPosition = AdditionalInformationPosition.FirstSlide;
                    break;
                case PowerPraiseSong.CopyrightPosition.LastSlide:
                    song.CopyrightPosition = AdditionalInformationPosition.LastSlide;
                    break;
                case PowerPraiseSong.CopyrightPosition.None:
                    song.CopyrightPosition = AdditionalInformationPosition.None;
                    break;
            }

            // Source / songbook
            song.SongBooks.FromString(ppl.SourceText);
            song.SourcePosition = ppl.SourceTextEnabled ? AdditionalInformationPosition.FirstSlide : AdditionalInformationPosition.None;

            // Song parts
            foreach (PowerPraiseSongPart prt in ppl.Parts)
            {
                SongPart part = new SongPart
                {
                    Caption = prt.Caption
                };
                foreach (PowerPraiseSongSlide sld in prt.Slides)
                {
                    SongSlide slide = new SongSlide();
                    if (sld.BackgroundNr >= 0 && ppl.BackgroundImages.Count > sld.BackgroundNr - 1)
                    {
                        slide.Background = ParseBackground(ppl.BackgroundImages[sld.BackgroundNr]);
                    }
                    slide.TextSize = sld.MainSize > 0 ? sld.MainSize : (song.MainText.Font != null ? song.MainText.Font.Size : 0);
                    slide.Lines.AddRange(sld.Lines);
                    slide.Translation.AddRange(sld.Translation);
                    part.Slides.Add(slide);
                }
                song.Parts.Add(part);
            }

            // Order
            foreach (PowerPraiseSongPart o in ppl.Order)
            {
                foreach (SongPart p in song.Parts)
                {
                    if (p.Caption == o.Caption)
                    {
                        song.PartSequence.Add(p);
                        break;
                    }
                }
            }

            MapFormatting(ppl, song);

            return song;
        }

        private static IBackground ParseBackground(string bg)
        {
            if (Regex.IsMatch(bg, @"^\d+$"))
            {
                int trySize;
                if (int.TryParse(bg, out trySize))
                {
                    try
                    {
                        return new ColorBackground(PowerPraiseFileUtil.ConvertColor(trySize));
                    } 
                    catch (ArgumentException)
                    {
                        return null;
                    }
                }
            }
            else if (bg.Trim() != String.Empty)
            {
                return new ImageBackground(bg);
            }
            return null;
        }

        private static string MapBackground(IBackground bg) {
            if (bg != null)
            {
                if (bg.GetType() == typeof(ImageBackground))
                {
                    return ((ImageBackground)bg).ImagePath;
                }
                if (bg.GetType() == typeof(ColorBackground))
                {
                    return PowerPraiseFileUtil.ConvertColor(((ColorBackground)bg).Color).ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// Maps a song to a PowerPraise song object
        /// </summary>
        /// <param name="song"></param>
        /// <param name="ppl"></param>
        public void Map(Song song, PowerPraiseSong ppl)
        {
            // General
            ppl.Title = song.Title;
            ppl.Language = song.Language;
            ppl.Category = song.Themes.Count > 0 ? song.Themes[0] : null;

            int bgIndex = 0;
            Dictionary<string, int> backgrounds = new Dictionary<string, int>();
            
            // Song parts
            foreach (var songPart in song.Parts)
            {
                PowerPraiseSongPart pplPart = new PowerPraiseSongPart
                {
                    Caption = songPart.Caption
                };
                foreach (var songSlide in songPart.Slides)
                {
                    PowerPraiseSongSlide pplSlide = new PowerPraiseSongSlide();

                    string bg = MapBackground(songSlide.Background);
                    int backgroundNr;
                    if (bg == null)
                    {
                        bg = PowerPraiseConstants.DefaultBackground;
                    }
                    if (!backgrounds.ContainsKey(bg))
                    {
                        backgroundNr = bgIndex;
                        backgrounds.Add(bg, bgIndex++);
                    }
                    else
                    {
                        backgroundNr = backgrounds[bg];
                    }
                    pplSlide.BackgroundNr = backgroundNr;

                    pplSlide.MainSize = (int)(songSlide.TextSize > 0 ? songSlide.TextSize : (song.MainText != null && song.MainText.Font != null ? song.MainText.Font.Size : 0));
                    pplSlide.Lines.AddRange(songSlide.Lines);
                    pplSlide.Translation.AddRange(songSlide.Translation);
                    pplPart.Slides.Add(pplSlide);
                }
                ppl.Parts.Add(pplPart);
            }

            // Part order
            if (song.PartSequence.Any())
            {
                foreach (SongPart p in song.PartSequence)
                {
                    foreach (PowerPraiseSongPart t in ppl.Parts)
                    {
                        if (p.Caption == t.Caption)
                        {
                            ppl.Order.Add(t);
                            break;
                        }
                    }
                }
            }
            else
            {
                foreach (PowerPraiseSongPart t in ppl.Parts)
                {
                    ppl.Order.Add(t);
                }
            }

            // Copyright text
            ppl.CopyrightText.Add(song.Copyright);
            if (song.CopyrightPosition ==  AdditionalInformationPosition.FirstSlide)
            {
                ppl.CopyrightTextPosition = PowerPraiseSong.CopyrightPosition.FirstSlide;
            }
            else if (song.CopyrightPosition == AdditionalInformationPosition.LastSlide)
            {
                ppl.CopyrightTextPosition = PowerPraiseSong.CopyrightPosition.LastSlide;
            }
            else if (song.CopyrightPosition == AdditionalInformationPosition.None)
            {
                ppl.CopyrightTextPosition = PowerPraiseSong.CopyrightPosition.None;
            }

            // Source / songbook
            ppl.SourceText = song.SongBooks.ToString();
            ppl.SourceTextEnabled = (song.SourcePosition == AdditionalInformationPosition.FirstSlide);


            // Backgrounds
            ppl.BackgroundImages.AddRange(backgrounds.Keys);

            if (song.MainText != null)
            {
                ppl.MainLineSpacing = song.MainText.LineSpacing;
                ppl.TranslationLineSpacing = song.TranslationText.LineSpacing;
            }

            MapFormatting(song, ppl);
        }


        private void MapFormatting(PowerPraiseSong ppl, Song song)
        {
            // Formatting definitions
            song.MainText = new TextFormatting(
                ppl.MainTextFontFormatting.Font,
                ppl.MainTextFontFormatting.Color,
                new TextOutline(ppl.MainTextFontFormatting.OutlineWidth, ppl.TextOutlineFormatting.Color),
                new TextShadow(ppl.MainTextFontFormatting.ShadowDistance, 0, ppl.TextShadowFormatting.Direction, ppl.TextShadowFormatting.Color),
                ppl.MainLineSpacing
            );
            song.TranslationText = new TextFormatting(
                ppl.TranslationTextFontFormatting.Font,
                ppl.TranslationTextFontFormatting.Color,
                new TextOutline(ppl.TranslationTextFontFormatting.OutlineWidth, ppl.TextOutlineFormatting.Color),
                new TextShadow(ppl.TranslationTextFontFormatting.ShadowDistance, 0, ppl.TextShadowFormatting.Direction, ppl.TextShadowFormatting.Color),
                ppl.TranslationLineSpacing
            );
            song.CopyrightText = new TextFormatting(
                ppl.CopyrightTextFontFormatting.Font,
                ppl.CopyrightTextFontFormatting.Color,
                new TextOutline(ppl.CopyrightTextFontFormatting.OutlineWidth, ppl.TextOutlineFormatting.Color),
                new TextShadow(ppl.CopyrightTextFontFormatting.ShadowDistance, 0, ppl.TextShadowFormatting.Direction, ppl.TextShadowFormatting.Color),
                0
            );
            song.SourceText = new TextFormatting(
                ppl.SourceTextFontFormatting.Font,
                ppl.SourceTextFontFormatting.Color,
                new TextOutline(ppl.SourceTextFontFormatting.OutlineWidth, ppl.TextOutlineFormatting.Color),
                new TextShadow(ppl.SourceTextFontFormatting.ShadowDistance, 0, ppl.TextShadowFormatting.Direction, ppl.TextShadowFormatting.Color),
                0
            );

            // Text orientation
            song.TextOrientation = ppl.TextOrientation;
            song.TranslationPosition = ppl.TranslationTextPosition;

            // Enable or disable outline/shadow
            song.TextOutlineEnabled = ppl.TextOutlineFormatting.Enabled;
            song.TextShadowEnabled = ppl.TextShadowFormatting.Enabled;

            // Borders
            song.TextBorders = new SongTextBorders(
                ppl.Borders.TextLeft,
                ppl.Borders.TextTop,
                ppl.Borders.TextRight,
                ppl.Borders.TextBottom,
                ppl.Borders.CopyrightBottom,
                ppl.Borders.SourceTop,
                ppl.Borders.SourceRight
            );
        }

        private void MapFormatting(Song song, PowerPraiseSong ppl)
        {
            // Formatting definitions
            if (song.MainText != null)
            {
                ppl.MainTextFontFormatting = new PowerPraiseSong.FontFormatting
                {
                    Font = song.MainText.Font,
                    Color = song.MainText.Color,
                    OutlineWidth = song.MainText.Outline.Width,
                    ShadowDistance = song.MainText.Shadow.Distance
                };
            }
            if (song.TranslationText != null)
            {
                ppl.TranslationTextFontFormatting = new PowerPraiseSong.FontFormatting
                {
                    Font = song.TranslationText.Font,
                    Color = song.TranslationText.Color,
                    OutlineWidth = song.TranslationText.Outline.Width,
                    ShadowDistance = song.TranslationText.Shadow.Distance
                };
            }
            if (song.CopyrightText != null)
            {
                ppl.CopyrightTextFontFormatting = new PowerPraiseSong.FontFormatting
                {
                    Font = song.CopyrightText.Font,
                    Color = song.CopyrightText.Color,
                    OutlineWidth = song.CopyrightText.Outline.Width,
                    ShadowDistance = song.CopyrightText.Shadow.Distance
                };
            }
            if (song.SourceText != null)
            {
                ppl.SourceTextFontFormatting = new PowerPraiseSong.FontFormatting
                {
                    Font = song.SourceText.Font,
                    Color = song.SourceText.Color,
                    OutlineWidth = song.SourceText.Outline.Width,
                    ShadowDistance = song.SourceText.Shadow.Distance
                };
            }

            // Enable or disable outline/shadow
            if (song.MainText != null)
            {
                ppl.TextOutlineFormatting = new PowerPraiseSong.OutlineFormatting
                {
                    Color = song.MainText.Outline.Color,
                    Enabled = song.TextOutlineEnabled
                };
                ppl.TextShadowFormatting = new PowerPraiseSong.ShadowFormatting
                {
                    Color = song.MainText.Shadow.Color,
                    Direction = song.MainText.Shadow.Direction,
                    Enabled = song.TextShadowEnabled
                };
            }

            // Text orientation
            ppl.TextOrientation = song.TextOrientation;
            ppl.TranslationTextPosition = song.TranslationPosition;

            // Borders
            if (song.TextBorders != null)
            {
                ppl.Borders = new PowerPraiseSong.TextBorders
                {
                    TextLeft = song.TextBorders.TextLeft,
                    TextTop = song.TextBorders.TextTop,
                    TextRight = song.TextBorders.TextRight,
                    TextBottom = song.TextBorders.TextBottom,
                    CopyrightBottom = song.TextBorders.CopyrightBottom,
                    SourceTop = song.TextBorders.SourceTop,
                    SourceRight = song.TextBorders.SourceRight
                };
            }
        }
    }
}