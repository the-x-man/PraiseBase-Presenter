﻿using PraiseBase.Presenter.Model;
using PraiseBase.Presenter.Model.Song;
using PraiseBase.Presenter.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PraiseBase.Presenter.Projection
{
    public static class SongSlideTextFormattingMapper
    {
        public static void Map(Song s, ref SlideTextFormatting slideFormatting)
        {
            slideFormatting.Text = new SlideTextFormatting.MainTextFormatting()
            {
                // TODO respect specific slide text size
                MainText = (TextFormatting)s.MainText.Clone(),
                SubText = (TextFormatting)s.TranslationText.Clone(),
                Orientation = (TextOrientation)s.TextOrientation.Clone(),
                HorizontalPadding = s.TextBorders.TextLeft,
                VerticalPadding = s.TextBorders.TextTop
            };
            slideFormatting.Header = new SlideTextFormatting.TextBoxFormatting()
            {
                Text = (TextFormatting)s.SourceText.Clone(),
                // TODO Parametrize hard-coded value
                HorizontalOrientation = HorizontalOrientation.Right,
                HorizontalPadding = s.TextBorders.SourceRight,
                VerticalPadding = s.TextBorders.SourceTop
            };
            slideFormatting.Footer = new SlideTextFormatting.TextBoxFormatting()
            {
                Text = (TextFormatting)s.CopyrightText.Clone(),
                // TODO Parametrize hard-coded value
                HorizontalOrientation = HorizontalOrientation.Left,
                HorizontalPadding = s.TextBorders.CopyrightBottom,
                VerticalPadding = s.TextBorders.CopyrightBottom
            };
            slideFormatting.OutlineEnabled = s.TextOutlineEnabled;
            slideFormatting.ShadowEnabled = s.TextShadowEnabled;
        }

        public static void Map(Settings settings, ref SlideTextFormatting slideFormatting)
        {
            slideFormatting.Text = new SlideTextFormatting.MainTextFormatting()
            {
                MainText = new TextFormatting(
                    settings.ProjectionMasterFont,
                    settings.ProjectionMasterFontColor,
                    new TextOutline(settings.ProjectionOutlineSize, settings.ProjectionOutlineColor),
                    new TextShadow(settings.ProjectionShadowSize, 125, settings.ProjectionShadowColor),
                    settings.ProjectionMasterLineSpacing
                ),
                SubText = new TextFormatting(
                    settings.ProjectionMasterFontTranslation,
                    settings.ProjectionMasterTranslationColor,
                    new TextOutline(settings.ProjectionOutlineSize, settings.ProjectionOutlineColor),
                    // TODO Parametrize hard-coded value
                    new TextShadow(settings.ProjectionShadowSize, 125, settings.ProjectionShadowColor),
                    settings.ProjectionMasterTranslationLineSpacing
                ),
                // TODO Parametrize hard-coded value
                Orientation = new TextOrientation(VerticalOrientation.Middle, HorizontalOrientation.Center),
                HorizontalPadding = settings.ProjectionPadding,
                VerticalPadding = settings.ProjectionPadding
            };
            slideFormatting.Header = new SlideTextFormatting.TextBoxFormatting()
            {
                Text = new TextFormatting(
                    settings.ProjectionMasterSourceFont,
                    settings.ProjectionMasterSourceColor,
                    new TextOutline(settings.ProjectionOutlineSize, settings.ProjectionOutlineColor),
                    new TextShadow(settings.ProjectionShadowSize, 125, settings.ProjectionShadowColor),
                    settings.ProjectionMasterLineSpacing
                ),
                // TODO Parametrize hard-coded values
                HorizontalOrientation = HorizontalOrientation.Right,
                HorizontalPadding = 20,
                VerticalPadding = 40
            };
            slideFormatting.Footer = new SlideTextFormatting.TextBoxFormatting()
            {
                Text = new TextFormatting(
                    settings.ProjectionMasterCopyrightFont,
                    settings.ProjectionMasterCopyrightColor,
                    new TextOutline(settings.ProjectionOutlineSize, settings.ProjectionOutlineColor),
                    new TextShadow(settings.ProjectionShadowSize, 125, settings.ProjectionShadowColor),
                    settings.ProjectionMasterLineSpacing
                ),
                // TODO Parametrize hard-coded values
                HorizontalOrientation = HorizontalOrientation.Left,
                HorizontalPadding = 30,
                VerticalPadding = 40
            };
            // TODO Parametrize hard-coded values
            slideFormatting.OutlineEnabled = true;
            slideFormatting.ShadowEnabled = true;
        }
    }
}
