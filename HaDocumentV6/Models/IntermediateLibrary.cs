using System.Collections.Generic;
using HaDocument.Interfaces;

namespace HaDocument.Models
{
    public class IntermediateLibrary
    {
        public Dictionary<string, Tradition> Traditions;
        public Dictionary<string, Person> Persons;
        public Dictionary<string, Meta> Metas;
        public Dictionary<string, Meta> ExcludedMetas;
        public Dictionary<string, List<Marginal>> Marginals;
        public Dictionary<string, Location> Locations;
        public Dictionary<string, Letter> Letters;
        public Dictionary<string, Person> HandPersons;
        public Dictionary<string, Editreason> Editreasons;
        public Dictionary<string, Comment> Comments;
        public Dictionary<string, List<Backlink>> Backlinks;
        public Dictionary<string, List<Hand>> Hands;
        public Dictionary<string, App> Apps;

        // Helper Library for precalculationg the Structure of the Document:
        public Dictionary<string, Dictionary<string, HashSet<string>>> LetterPageLines;

        public Library GetLibrary(IHaDocumentOptions options)
        {
            var Structure = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            foreach (var letter in LetterPageLines)
            {
                if (Metas.ContainsKey(letter.Key) &&
                Metas[letter.Key].ZH != null)
                {
                    if (!Structure.ContainsKey(Metas[letter.Key].ZH.Volume))
                    {
                        Structure.Add(Metas[letter.Key].ZH.Volume, new Dictionary<string, Dictionary<string, string>>());
                    }
                    foreach (var page in letter.Value)
                    {
                        if (!Structure[Metas[letter.Key].ZH.Volume].ContainsKey(page.Key))
                        {
                            Structure[Metas[letter.Key].ZH.Volume].Add(page.Key, new Dictionary<string, string>());
                        }
                        foreach (var line in page.Value)
                        {
                            if (!Structure[Metas[letter.Key].ZH.Volume][page.Key].ContainsKey(line))
                            {
                                Structure[Metas[letter.Key].ZH.Volume][page.Key].Add(line, letter.Key);
                            }
                        }
                    }
                }

            }

            return new Library(
                Traditions,
                Persons,
                Metas,
                ExcludedMetas,
                Marginals,
                Locations,
                Letters,
                HandPersons,
                Editreasons,
                Comments,
                Backlinks,
                Hands,
                Structure,
                Apps,
                options
            );
        }
    }
}