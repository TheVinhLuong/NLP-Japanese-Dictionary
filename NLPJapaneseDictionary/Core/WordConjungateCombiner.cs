﻿using NLPJDict.DatabaseTable.NLPJDictCore;
using NLPJDict.Kuromoji.Core;
using NLPJDict.Kuromoji.Core.HelperClasses;
using NLPJDict.Kuromoji.Interfaces;
using NLPJDict.NLPJDictCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLPJDict.NLPJDictCore
{
    public static class WordConjungateCombiner
    {
        public static List<WordInformation> Combine<T>(List<T> tokens, Database dictionary) where T : class, IToken
        {
            List<WordInformation> words = new List<WordInformation>();
            WordInformation word = null;
            T nextToken = null;
            T previousToken = null;
            bool isPreviousConjugated = false;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (i < tokens.Count - 1)
                    nextToken = tokens[i + 1];
                else
                    nextToken = null;

                if (isPreviousConjugated)
                {
                    if (word.TryAddConjungationPart(previousToken, tokens[i], nextToken))
                    {
                        word.AddWordPart(tokens[i]);

                        if (nextToken != null &&
                            IsConjugationForm(tokens[i], nextToken))
                            isPreviousConjugated = true;
                        else
                            isPreviousConjugated = false;

                        previousToken = tokens[i];
                        continue;
                    }
                }

                if (nextToken != null &&
                    IsConjugationForm(tokens[i], nextToken))
                    isPreviousConjugated = true;
                else
                    isPreviousConjugated = false;

                word = new WordInformation(tokens[i], nextToken, dictionary);
                words.Add(word);
                if (word.IsHaveSplitWords)
                {
                    words.AddRange(word.SplitWords);
                    isPreviousConjugated = false;
                }
                previousToken = tokens[i];
            }

            FinalizeWordList(dictionary, words);

            return words;
        }

        private static void FinalizeWordList(Database dictionary, List<WordInformation> words)
        {
            for (int i = 0; i < words.Count; i++)
            {
                words[i].FinalizeWord(dictionary);
                if (words[i].IsHaveSplitWords)
                    words.InsertRange(i + 1, words[i].SplitWords);
            }
        }

        public static bool IsHave(string tokenProp)
        {
            return !tokenProp.Equals(Constant.TOKEN_NOT_HAVE, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsConjugationForm(IToken token, IToken nextToken)
        {
            if (!IsHave(token.ConjugationForm))
            {
                //Deal with De-iru
                if (WordInformation.IsDeIru(token, nextToken))
                    return true;

                return false;
            }
            //Deal with nde*
            if (token.BaseForm.Equals("ん", StringComparison.OrdinalIgnoreCase) &&
                    (nextToken.BaseForm.Equals("です", StringComparison.OrdinalIgnoreCase) 
                     || nextToken.Surface.Equals("で", StringComparison.OrdinalIgnoreCase)) )
                return true;
            
            if (token.ConjugationForm.Equals(WordInformation.JAP_BASE_FORM, StringComparison.OrdinalIgnoreCase))
                return false;

            if (WordInformation.IsInConjugationList(nextToken.BaseForm))
                return true;

            //Deal with -masende
            if (token.Surface.Equals("ませ", StringComparison.OrdinalIgnoreCase)
                && nextToken.Surface.Equals("んで", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            //Deal with dewanai & dearu
            if (token.Surface.Equals("で", StringComparison.OrdinalIgnoreCase)
                && (   nextToken.Surface.Equals("は", StringComparison.OrdinalIgnoreCase) 
                    || (nextToken.BaseForm.Equals("ある", StringComparison.OrdinalIgnoreCase)
                        || nextToken.BaseForm.Equals("ない", StringComparison.OrdinalIgnoreCase))
                    )
               )
            {
                return true;
            }

            return false;
        }  
    }
}