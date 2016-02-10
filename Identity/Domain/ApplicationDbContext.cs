using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using System;
using System.Reflection;

namespace CreativeColon.Raven.Identity.Domain
{
    public static class ApplicationDbContext
    {
        static readonly Lazy<IDocumentStore> LazyDocumentStore = new Lazy<IDocumentStore>(CreateDocumentStore);
        static string ConnectionStringName = "RavenConnection";

        static IDocumentStore CreateDocumentStore()
        {
            var Store = new DocumentStore() { ConnectionStringName = ConnectionStringName }.Initialize();
            IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), Store);
            return Store;
        }

        public static IAsyncDocumentSession Create()
        {
            return Create(ConnectionStringName);
        }

        public static IAsyncDocumentSession Create(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
            return LazyDocumentStore.Value.OpenAsyncSession();
        }
    }
}
