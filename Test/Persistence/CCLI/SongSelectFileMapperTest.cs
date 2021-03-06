﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PraiseBase.Presenter.Model.Song;

namespace PraiseBase.Presenter.Persistence.CCLI
{
    
    
    /// <summary>
    ///This is a test class for CcliUsrSongFileReaderTest and is intended
    ///to contain all CcliUsrSongFileReaderTest Unit Tests
    ///</summary>
    [TestClass]
    public class SongSelectFileMapperTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        /// <summary>
        ///A test for IsFileSupported
        ///</summary>
        [TestMethod]
        public void IsFileSupportedTest()
        {
            SongSelectFileReader target = new SongSelectFileReader();
            const string filename = "Resources/ccli/Ein Lied für Gott.usr";
            const bool expected = true;
            bool actual = target.IsFileSupported(filename);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod]
        public void LoadTest()
        {
            SongSelectFileMapper mapper = new SongSelectFileMapper();

            SongSelectFile source = new SongSelectFile
            {
                Title = "Ein Lied für Gott",
                Author = "Muster, Hans",
                Copyright = "Gemeinfrei (Public Domain)",
                Admin = "Verlag ABC",
                Key = "E"
            };

            source.Themes.Add("Celebration");
            source.Themes.Add("God's Attributes");
            source.Themes.Add("Love");
            source.Themes.Add("Joy");

            var verse = new SongSelectFile.Verse
            {
                Caption = "Vers 1"
            };
            verse.Lines.Add("Lorem ipsum dolor sit amet,");
            verse.Lines.Add("consectetur adipiscing elit.");
            verse.Lines.Add("Vivamus odio massa,");
            verse.Lines.Add("lacinia in mollis quis,");
            verse.Lines.Add("vehicula sed justo");
            source.Verses.Add(verse);

            verse = new SongSelectFile.Verse
            {
                Caption = "Vers 2"
            };
            verse.Lines.Add("Nunc cursus libero non quam lobortis");
            verse.Lines.Add("ac pharetra leo facilisis.");
            verse.Lines.Add("Proin tortor tellus,");
            verse.Lines.Add("fermentum mattis euismod eu,");
            verse.Lines.Add("faucibus vel justo.");
            source.Verses.Add(verse);

            verse = new SongSelectFile.Verse
            {
                Caption = "Vers 3"
            };
            verse.Lines.Add("Fusce pellentesque rhoncus felis,");
            verse.Lines.Add("eu convallis ante tempus a.");
            verse.Lines.Add("Cum sociis natoque penatibus");
            verse.Lines.Add("et magnis dis parturient montes,");
            verse.Lines.Add("nascetur ridiculus mus.");
            source.Verses.Add(verse);

            Song expected = new Song
            {
                Title = "Ein Lied für Gott"
            };

            expected.Themes.Add("Celebration");
            expected.Themes.Add("God's Attributes");
            expected.Themes.Add("Love");
            expected.Themes.Add("Joy");
            var aut = new SongAuthor
            {
                Name = "Muster, Hans"
            };
            expected.Authors.Add(aut);
            expected.Copyright = "Gemeinfrei (Public Domain)";
            expected.RightsManagement = "Verlag ABC";
            expected.Key = "E";

            var part = new SongPart
            {
                Caption = "Vers 1"
            };
            var slide = new SongSlide();
            slide.Lines.Add("Lorem ipsum dolor sit amet,");
            slide.Lines.Add("consectetur adipiscing elit.");
            slide.Lines.Add("Vivamus odio massa,");
            slide.Lines.Add("lacinia in mollis quis,");
            slide.Lines.Add("vehicula sed justo");
            part.Slides.Add(slide);
            expected.Parts.Add(part);

            part = new SongPart
            {
                Caption = "Vers 2"
            };
            slide = new SongSlide();
            slide.Lines.Add("Nunc cursus libero non quam lobortis");
            slide.Lines.Add("ac pharetra leo facilisis.");
            slide.Lines.Add("Proin tortor tellus,");
            slide.Lines.Add("fermentum mattis euismod eu,");
            slide.Lines.Add("faucibus vel justo.");
            part.Slides.Add(slide);
            expected.Parts.Add(part);

            part = new SongPart
            {
                Caption = "Vers 3"
            };
            slide = new SongSlide();
            slide.Lines.Add("Fusce pellentesque rhoncus felis,");
            slide.Lines.Add("eu convallis ante tempus a.");
            slide.Lines.Add("Cum sociis natoque penatibus");
            slide.Lines.Add("et magnis dis parturient montes,");
            slide.Lines.Add("nascetur ridiculus mus.");
            part.Slides.Add(slide);
            expected.Parts.Add(part);

            Song actual = mapper.Map(source);
            Assert.AreEqual(expected.Title, actual.Title, "Wrong song title");
            Assert.AreEqual(expected.Language, actual.Language, "Wrong language");
            Assert.AreEqual(expected.Copyright, actual.Copyright, "Wrong copyright");
            Assert.AreEqual(expected.RightsManagement, actual.RightsManagement, "Wrong RightsManagement");
            Assert.AreEqual(expected.Key, actual.Key, "Wrong key");
            for (int i = 0; i < expected.Themes.Count; i++) 
            {
                Assert.AreEqual(expected.Themes[i], actual.Themes[i], "Wrong theme");
            }
            for (int i = 0; i < expected.Authors.Count; i++)
            {
                Assert.AreEqual(expected.Authors[i].Name, actual.Authors[i].Name, "Wrong Author");
            }

            Assert.AreEqual(expected.Parts.Count, actual.Parts.Count, "Parts incomplete");
            for (int i = 0; i < expected.Parts.Count; i++)
            {
                Assert.AreEqual(expected.Parts[i].Caption, actual.Parts[i].Caption, "Wrong verse name in verse " + i);
                Assert.AreEqual(expected.Parts[i].Slides.Count, actual.Parts[i].Slides.Count, "Slides incomplete in verse " + i);
                for (int j = 0; j < expected.Parts[i].Slides.Count; j++)
                {
                    Assert.AreEqual(expected.Parts[i].Slides[j].Lines.Count, actual.Parts[i].Slides[j].Lines.Count, "Slide lines incomplete in verse " + i + " slide " + j);
                    for (int k = 0; k < expected.Parts[i].Slides[j].Lines.Count; k++)
                    {
                        Assert.AreEqual(expected.Parts[i].Slides[j].Lines[k], actual.Parts[i].Slides[j].Lines[k], "Wrong slide lyrics");
                    }
                }
            }
        }

        /// <summary>
        ///A test for ReadTitle
        ///</summary>
        [TestMethod]
        public void ReadTitleTest()
        {
            SongSelectFileReader reader = new SongSelectFileReader();
            Assert.AreEqual("Ein Lied für Gott", reader.ReadTitle("Resources/ccli/Ein Lied für Gott.usr"));
            Assert.IsNull(reader.ReadTitle("Resources/ccli/non-existing-file.usr"));
        }
    }
}
