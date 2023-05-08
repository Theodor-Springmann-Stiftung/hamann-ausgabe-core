using System;
using System.Collections.Immutable;

using HaDocument.Models;
using System.Linq;

namespace HaDocument.Interfaces {
    public interface ILibrary {
        IHaDocumentOptions Options { get; }
        ImmutableDictionary<string, Tradition> Traditions { get; }
        ImmutableDictionary<string, Person> Persons { get; }
        ImmutableDictionary<string, Meta> Metas { get; }
        ImmutableDictionary<string, Meta> ExcludedMetas { get; }
        ImmutableDictionary<string, Marginal> Marginals { get; }
        ImmutableDictionary<string, Location> Locations { get; }
        ImmutableDictionary<string, Letter> Letters { get; }
        ImmutableDictionary<string, Person> HandPersons { get; }
        ImmutableDictionary<string, App> Apps { get; }
        ImmutableDictionary<string, Editreason> Editreasons { get; }
        ImmutableDictionary<string, Comment> Comments { get; }
        ImmutableDictionary<string, ImmutableList<Backlink>> Backlinks { get; }
        ImmutableDictionary<string, ImmutableList<Hand>> Hands { get; }
        ImmutableDictionary<string, ImmutableDictionary<string, ImmutableDictionary<string, string>>> Structure { get; }
        
        ImmutableDictionary<string, Lookup<string, Comment>> CommentsByCategoryLetter { get; }
        Lookup<string, Comment> CommentsByCategory { get; }
        Lookup<string, Marginal> MarginalsByLetter { get; }
        Lookup<string, Editreason> EditreasonsByLetter { get; }
        ImmutableSortedSet<Meta> MetasByDate { get; }
        ILookup<int, Meta> MetasByYear { get; }
        ILookup<int, Meta> ExcludedMetasByYear { get; }
        ImmutableDictionary<string, Comment> SubCommentsByID { get; }
    }
}