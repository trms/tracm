using System;
using System.Collections.Generic;
using System.Text;

namespace tracm
{
    public class PBCoreGenre
    {
        private List<string> m_genres = null;
        public List<string> Genres
        {
            get
            {
                if (m_genres == null)
                    InitializeGenres();

                return m_genres;
            }
        }

        public string findMatch(string genre)
        {
            return Genres.Find(g => g.ToLower() == genre.ToLower());
        }

        private void InitializeGenres()
        {
            m_genres = new List<string>();

            #region genres array
            var genres = new string[] {
                "",
                "Action",
                "Actuality",
                "Adults Only",
                "Adventure",
                "Advice",
                "Agriculture",
                "Animals",
                "Animation",
                "Anime",
                "Anthology",
                "Art",
                "Arts/crafts",
                "Auction",
                "Auto",
                "Aviation",
                "Awards",
                "Bicycle",
                "Biography",
                "Boat",
                "Business/Financial",
                "Call-in",
                "Children",
                "Children-music",
                "Children-special",
                "Children-talk",
                "Collectibles",
                "Comedy",
                "Comedy-drama",
                "Commentary",
                "Community",
                "Computers",
                "Concert",
                "Consumer",
                "Cooking",
                "Crime",
                "Crime drama",
                "Dance",
                "Debate",
                "Docudrama",
                "Documentary",
                "Drama",
                "Educational",
                "Entertainment",
                "Environment",
                "Event",
                "Excerpts",
                "Exercise",
                "Fantasy",
                "Fashion",
                "Feature",
                "Forecast",
                "French",
                "Fundraiser",
                "Game show",
                "Gay/lesbian",
                "Health",
                "Historical drama",
                "History",
                "Holiday",
                "Holiday music",
                "Holiday music special",
                "Holiday special",
                "Holiday-children",
                "Holiday-children special",
                "Home improvement",
                "Horror",
                "Horse",
                "House/garden",
                "How-to",
                "Interview",
                "Law",
                "Magazine",
                "Medical",
                "Miniseries",
                "Music",
                "Music special",
                "Music talk",
                "Musical",
                "Musical comedy",
                "Mystery",
                "Nature",
                "News",
                "News conference",
                "Newsmagazine",
                "Obituary",
                "Opera",
                "Outtakes",
                "Panel",
                "Parade",
                "Paranormal",
                "Parenting",
                "Performance",
                "Performing arts",
                "Political commercial",
                "Politics",
                "Polls and Surveys",
                "Press Release",
                "Promotional announcement",
                "Public service announcement",
                "Question and Answer Session",
                "Quote",
                "Reading",
                "Reality",
                "Religious",
                "Retrospective",
                "Romance",
                "Romance-comedy",
                "Science",
                "Science fiction",
                "Self improvement",
                "Shopping",
                "Sitcom",
                "Soap",
                "Soap special",
                "Soap talk",
                "Spanish",
                "Special",
                "Speech",
                "Sports",
                "Standup",
                "Suspense",
                "Talk",
                "Theater",
                "Trailer",
                "Travel",
                "Variety",
                "Voice-over",
                "War",
                "Weather",
                "Western",
                };
#endregion

            m_genres.AddRange(genres);
        }
    }
}
