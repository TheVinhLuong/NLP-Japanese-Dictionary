﻿using NLPJDict.DatabaseTable.NLPJDictCore;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPJDict.NLPJDictCore.DatabaseTable
{
    public class KanjiDict
    {
        private const string KANJI_RADICAL_QUERY = "SELECT KanjiElement FROM KanjiDict WHERE RadicalList MATCH ? ORDER BY StrokeCount ASC LIMIT 5";

        [PrimaryKey]
        public string KanjiElement { get; set; }

        public string Variants { get; set; }

        public string EntrySequencInJmDict { get; set; }

        public int RadicalClassic { get; set; }

        public string RadicalList { get; set; }
        public string SkipCode { get; set; }
        public string SkipMissClass { get; set; }

        public int Grade { get; set; }
        public int StrokeCount { get; set; }
        public string RadicalName { get; set; }
        public string FrequencyRank { get; set; }
        public string OldJlpt { get; set; }

        public string Kunyomi { get; set; }
        public string Onyomi { get; set; }
        public string Nanori { get; set; }
        public string Vietnam { get; set; }
        public string Pinyin { get; set; }
        public string KoreanR { get; set; }
        public string KoreanH { get; set; }

        public string EngMean { get; set; }

        public string SVGData { get; set; }

        public KanjiDict() { }

        public static KanjiDict GetKanji(string word, Database dictionary)
        {
            var kanji = dictionary.QueryColumn<KanjiDict>("SELECT * FROM KanjiDict WHERE KanjiElement = ? LIMIT 1", word);
            if (kanji.Count > 0)
                return kanji[0];
            else
                return null;
        }

        public static List<KanjiDict> GetKanjiFromRadicals(char[] radicals, Database dictionary)
        {
            var text = String.Join(" ", radicals);
            var kanji = dictionary.QueryColumn<KanjiDict>(KANJI_RADICAL_QUERY, text);

            if (kanji.Count == 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach(var radical in radicals)
                {
                    var radicalList = dictionary.QueryColumn<KanjiDict>("SELECT RadicalList FROM KanjiDict WHERE KanjiElement MATCH ? LIMIT 1", radical.ToString());
                    if(radicalList.Count > 0)
                    {
                        AppendWithSpace(builder, radicalList[0].RadicalList);
                    }
                    else
                    {
                        AppendWithSpace(builder, radical);
                    }
                }

                kanji = dictionary.QueryColumn<KanjiDict>(KANJI_RADICAL_QUERY, builder.ToString());
            }

            return kanji;
        }

        private static void AppendWithSpace(StringBuilder builder, string radical)
        {
            builder.Append(radical);
            builder.Append(" ");
        }

        private static void AppendWithSpace(StringBuilder builder, char radical)
        {
            builder.Append(radical);
            builder.Append(" ");
        }
    }
}