using System;
using HaDocument.Interfaces;
using System.Collections.Immutable;
using System.Collections.Generic;
using HaDocument.Models;
using HaDocument.Comparers;
using System.Linq;
using System.Collections.Specialized;
namespace HaDocument.Models
{
    public class Library : ILibrary
    {
        public IHaDocumentOptions Options { get; }
        public ImmutableDictionary<string, Tradition> Traditions { get; }
        public ImmutableDictionary<string, Person> Persons { get; }
        public ImmutableDictionary<string, Meta> Metas { get; }
        public ImmutableDictionary<string, Marginal> Marginals { get; }
        public ImmutableDictionary<string, Location> Locations { get; }
        public ImmutableDictionary<string, Letter> Letters { get; }
        public ImmutableDictionary<string, Person> HandPersons { get; }
        public ImmutableDictionary<string, Editreason> Editreasons { get; }
        public ImmutableDictionary<string, Comment> Comments { get; }
        public ImmutableDictionary<string, ImmutableList<Backlink>> Backlinks { get; }
        public ImmutableDictionary<string, ImmutableList<Hand>> Hands { get; }

        // Structure for lookups from ZH:
        public ImmutableDictionary<string, ImmutableDictionary<string, ImmutableDictionary<string, string>>> Structure { get; }

        // Lookups:
        // Auswählen von Kommentaren nach (1) Kategorie, (2) Anfangsbuchstaben vom Lemma.
        // So: _ = CommentsByCategoryLetter['neuzeit']['A']
        public ImmutableDictionary<string, Lookup<string, Comment>> CommentsByCategoryLetter { get; }
        public Lookup<string, Comment> CommentsByCategory { get; }
        // Auswählen von Subkommentaren nach ID
        public ImmutableDictionary<string, Comment> SubCommentsByID { get; }
        // Auswählen von Marginalien nach Briefen
        public Lookup<string, Marginal> MarginalsByLetter { get; }
        // Ausw?hlen von Edits nach Briefen
        public Lookup<string, Editreason> EditreasonsByLetter { get; }
        // Auswählen von Briefen nach autoptischer Numemr und in zeitlich sortierter Reihenfolge.
        public ImmutableSortedSet<Meta> MetasByDate { get; }
        // Auswählen von Briefen nach dem Jahr, sortiert nach Datum
        public ILookup<int, Meta> MetasByYear { get; }


        public Library(
            Dictionary<string, Tradition> traditions,
            Dictionary<string, Person> persons,
            Dictionary<string, Meta> meta,
            Dictionary<string, Marginal> marginals,
            Dictionary<string, Location> locations,
            Dictionary<string, Letter> letters,
            Dictionary<string, Person> handPersons,
            Dictionary<string, Editreason> editReasons,
            Dictionary<string, Comment> comments,
            Dictionary<string, List<Backlink>> backlinks,
            Dictionary<string, List<Hand>> hands,
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> Structure,
            IHaDocumentOptions options
        )
        {
            // Dictionaries
            Traditions = ImmutableDictionary.ToImmutableDictionary(traditions);
            Persons = ImmutableDictionary.ToImmutableDictionary(persons);
            Metas = ImmutableDictionary.ToImmutableDictionary(meta);
            Marginals = ImmutableDictionary.ToImmutableDictionary(marginals);
            Locations = ImmutableDictionary.ToImmutableDictionary(locations);
            Letters = ImmutableDictionary.ToImmutableDictionary(letters);
            HandPersons = ImmutableDictionary.ToImmutableDictionary(handPersons);
            Editreasons = ImmutableDictionary.ToImmutableDictionary(editReasons);
            Comments = ImmutableDictionary.ToImmutableDictionary(comments);

            var backbuilder = ImmutableDictionary.CreateBuilder<string, ImmutableList<Backlink>>();
            foreach (var entry in backlinks)
                backbuilder.Add(entry.Key, ImmutableList.ToImmutableList(entry.Value));
            Backlinks = backbuilder.ToImmutableDictionary();

            var handbuilder = ImmutableDictionary.CreateBuilder<string, ImmutableList<Hand>>();
            foreach (var entry in hands)
                handbuilder.Add(entry.Key, ImmutableList.ToImmutableList(entry.Value));
            Hands = handbuilder.ToImmutableDictionary();

            // Lookups
            CommentsByCategory = (Lookup<string, Comment>)Comments.Values.ToLookup(x => x.Type);
            var CommentsByLetter_builder = ImmutableDictionary.CreateBuilder<string, Lookup<string, Comment>>();
            foreach (var ts in CommentsByCategory)
            {
                CommentsByLetter_builder.Add(ts.Key, (Lookup<string, Comment>)ts.ToLookup(x => x.Index.Substring(0, 1).ToUpper()));
            }
            CommentsByCategoryLetter = CommentsByLetter_builder.ToImmutableDictionary();
            MarginalsByLetter = (Lookup<string, Marginal>)Marginals.Values.ToLookup(x => x.Letter);
            EditreasonsByLetter = (Lookup<string, Editreason>)Editreasons.Values.ToLookup(x => x.Letter);
            MetasByDate = Metas.Values.ToImmutableSortedSet<Meta>(new DefaultComparer());
            MetasByYear = Metas.Values.ToLookup(x => x.Sort.Year);

            var tempbuilder = ImmutableDictionary.CreateBuilder<string, Comment>();
            foreach (var comm in Comments)
                if (comm.Value.Kommentare != null)
                    foreach (var subcomm in comm.Value.Kommentare)
                        if (!tempbuilder.ContainsKey(subcomm.Key))
                            tempbuilder.Add(subcomm.Key, subcomm.Value);
            SubCommentsByID = tempbuilder.ToImmutableDictionary();

            var tempstructurebuilder = ImmutableDictionary.CreateBuilder<string, ImmutableDictionary<string, ImmutableDictionary<string, string>>>();
            foreach (var volume in Structure)
            {
                if (volume.Value != null)
                {
                    var tempvolbuilder = ImmutableDictionary.CreateBuilder<string, ImmutableDictionary<string, string>>();
                    foreach (var page in volume.Value)
                    {
                        if (page.Value != null)
                        {
                            tempvolbuilder.Add(page.Key, page.Value.ToImmutableDictionary());
                        }
                    }
                    if (tempvolbuilder.Any())
                    {
                        tempstructurebuilder.Add(volume.Key, tempvolbuilder.ToImmutableDictionary());
                    }
                }
            }

            this.Structure = tempstructurebuilder.ToImmutableDictionary();

            Options = options;

        }

        // public List<Meta> MetasByDate() {
        //     var ret = Metas.OrderBy(x => x.Value, new DefaultComparer()).ToLookup(x => x.Value.Autopsic, x => x.Value);
        //     ret.Sort(new DefaultComparer());
        //     return ret;
        // }

        public ImmutableList<Meta> MetasByZH()
        {
            return Metas.Values.ToImmutableList().Sort(new Comparers.ZHComparer());
        }

        public List<Person> PersonByAlphabet()
        {
            var ret = Persons.Values.ToList();
            ret.Sort(new PersonComparer());
            return ret;
        }


    }
}