using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;                                  
using System.Threading.Tasks;
/*В этом проекте вы создадите структуру данных индекса, который используется для быстрого поиска слов в документах.

В файле Indexer.cs реализуйте предложенные методы

    Add. Этот метод должен индексировать все слова в документе. 
    Разделители слов: { ' ', '.', ',', '!', '?', ':', '-','\r','\n' };
    Сложность – O(document.Length)
    GetIds. Этот метод должен искать по слову все id документов,
    где оно встречается. Сложность — O(result),
    где result — размер ответа на запрос
    GetPositions. Этот метод по слову и id документа должен искать все
    позиции, в которых слово начинается. Сложность — O(result)
    Remove. Этот метод должен удалять документ из индекса, 
    после чего слова в нем искаться больше не должны. 
    Сложность — O(document.Length)

Сложность операций с коллекциями

    Remove, Insert, Contains, IndexOf в List имеют сложность O(n).
    Add в коллекциях Dictionary и List имеет среднюю сложность O(1).
    Доступ по ключу в Dictonary имеет среднюю сложность O(1).
    Доступ по индексу в List имеет сложность O(1).
    Remove, ContainsKey в Dictionary имеют среднюю сложность O(1*/


namespace PocketGoogle
{
    public class My_Indexer : IIndexer
    {
        private char[] wordsSplitter = new char[] { ' ', '.', ',', '!', '?', ':', '-', '\r', '\n' };
        private Dictionary<string, Dictionary<int, List<int>>> words;

        public void Add(int id, string documentText)
        {
            var document = documentText.Split(wordsSplitter, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in document)
            {
                if (!words.ContainsKey(word))
                {
                    words.Add(word, new Dictionary<int, List<int>>());
                    words[word].Add(id, wordPositions(word, documentText));
                }
                else if (!words[word].ContainsKey(id))
                {
                    words[word].Add(id, wordPositions(word, documentText));
                }
            }
        }

        private int[] prefixFunction(string pattern)
        {
            var len = pattern.Length;
            var prefixLens = new int[len];
            prefixLens[0] = 0;
            for (var i = 1; i < len; i++)
            {
                var j = prefixLens[i - 1];
                while ((j > 0) && (pattern[i] != pattern[j]))
                    j = prefixLens[j - 1];
                if (pattern[i] == pattern[j])
                    j++;
                prefixLens[i] = j;
            }

            return prefixLens;
        }

        private List<int> wordPositions(string word, string text)
        {
            var result = new List<int>();
            var sample = prefixFunction(word);
            var len = word.Length;
            var j = 0;
            for (var i = 0; i < text.Length; i++)
            {
                if (j == len)
                    j = len - 1;
                while ((j > 0) && (text[i] != word[j]))
                    j = sample[j - 1];
                if (text[i] == word[j])
                    j++;
                if (j == len)
                    result.Add(i - len + 1);
            }
            return result;
        }

        public List<int> GetIds(string word)
        {
            if (words.ContainsKey(word)) return new List<int>(words[word].Keys);
            return new List<int>();
        }

        public List<int> GetPositions(int id, string word)
        {
            if (words.ContainsKey(word))
                if (words[word].ContainsKey(id))
                    return words[word][id];
            return new List<int>();
        }

        public void Remove(int id)
        {
            foreach (var word in words.Keys)
            {
                if (words[word].ContainsKey(id))
                    words[word].Remove(id);
            }
        }

        public My_Indexer()
        {
            words = new Dictionary<string, Dictionary<int, List<int>>>();
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PocketGoogle
//{
//    class Database : Dictionary<string, Dictionary<int, List<int>>>
//    {
//        public void Add(int documentId, int position, string word)
//        {
//            var docDict = ContainsKey(word) ? this[word]
//                                            : this[word] = new Dictionary<int, List<int>>();
//            var docList = docDict.ContainsKey(documentId) ? docDict[documentId]
//                                                          : docDict[documentId] = new List<int>();
//            docList.Add(position);
//        }

//        public List<int> GetIds(string word) =>
//            ContainsKey(word) ? this[word].Keys.ToList() : new List<int>();

//        public List<int> GetPositions(int documentId, string word) =>
//            ContainsKey(word) && this[word].ContainsKey(documentId) ? this[word][documentId]
//                                                                    : new List<int>();

//        public void RemoveDocument(int documentId)
//        {
//            foreach (var docDict in this.Values) docDict.Remove(documentId);
//        }
//    }

//    class Parser
//    {
//        public Parser(Database database, int documentId, string document)
//        {
//            id = documentId;
//            db = database;
//            text = document;
//            position = 0;
//        }

//        public void Process()
//        {
//            for (; ; )
//            {
//                SkipDelimiters();
//                if (position == text.Length) break;
//                ProcessWord();
//            }
//        }

//        void ProcessWord()
//        {
//            var length = GetWordLength();
//            string word = text.Substring(position, length);
//            db.Add(id, position, word);
//            position += length;
//        }

//        void SkipDelimiters()
//        {
//            while (position < text.Length &&
//                Delimiters.IndexOf(text[position]) != -1)
//                ++position;
//        }

//        int GetWordLength()
//        {
//            var endPosition = position;
//            while (endPosition < text.Length &&
//                Delimiters.IndexOf(text[endPosition]) == -1)
//                ++endPosition;
//            return endPosition - position;
//        }

//        private const string Delimiters = " .,!?:-\r\n";
//        private Database db;
//        private int id;
//        private string text;
//        private int position;
//    }

//    public class Indexer : IIndexer
//    {
//        public Indexer()
//        {
//            db = new Database();
//        }

//        /*
//         * Add. Этот метод должен индексировать все слова в документе. 
//         * Разделители слов: { ' ', '.', ',', '!', '?', ':', '-','\r','\n' }; 
//         * Сложность – O(document.Length)
//         */
//        public void Add(int id, string documentText)
//        {
//            new Parser(db, id, documentText).Process();
//        }

//        /*
//         * GetIds. Этот метод должен искать по слову все id документов, где оно встречается. 
//         * Сложность — O(result), где result — размер ответа на запрос
//         */
//        public List<int> GetIds(string word) => db.GetIds(word);

//        /*
//         * GetPositions. Этот метод по слову и id документа должен искать все позиции, 
//         * в которых слово начинается. Сложность — O(result)
//         */
//        public List<int> GetPositions(int id, string word) => db.GetPositions(id, word);

//        /*
//         * Remove.Этот метод должен удалять документ из индекса, 
//         * после чего слова в нем искаться больше не должны.
//         * Сложность — O(document.Length)
//         */
//        public void Remove(int id)
//        {
//            db.RemoveDocument(id);
//        }

//        // two-level dictionary word -> id -> indexes
//        private Database db;
//    }
//}

