using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Ticketing.Command.Application.Models;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.Common;

namespace Ticketing.Command.Infrastructure.Repositories
{
    public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;


        public MongoRepository(IMongoClient mongoClient, IOptions<MongoSettings> options)
        {
            _collection = mongoClient.GetDatabase(options.Value.Database)
                .GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
        }

        private protected string GetCollectionName(Type documentType)
        {
            var name = documentType.GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault();

            if (name is not null)
            {
                return ((BsonCollectionAttribute)name).CollectionName;
            }

            throw new ArgumentException($"Collection name not found for {documentType.Name}");
        }
        public IQueryable<TDocument> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public async Task<IClientSessionHandle> BeginSessionAsync(CancellationToken cancellationToken = default)
        {
            var options = new ClientSessionOptions
            {
                DefaultTransactionOptions = new TransactionOptions()
            };

            return await _collection.Database.Client.StartSessionAsync(options, cancellationToken);
        }

        public void BeginTransaction(IClientSessionHandle clientSessionHandle) => clientSessionHandle.StartTransaction();

        public Task CommitTransactionAsync(IClientSessionHandle clientSessionHandle, CancellationToken cancellationToken = default) => clientSessionHandle.CommitTransactionAsync(cancellationToken);

        public void DisposeSession(IClientSessionHandle clientSessionHandle) => clientSessionHandle.Dispose();

        public async Task InsertOneAsync(TDocument document, IClientSessionHandle clientSessionHandle, CancellationToken cancellationToken) => await _collection.InsertOneAsync(clientSessionHandle, document, null, cancellationToken);

        public Task RollbackTransactionAsync(IClientSessionHandle clientSessionHandle, CancellationToken cancellationToken = default) => clientSessionHandle.AbortTransactionAsync(cancellationToken);

        public async Task<IEnumerable<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken)
        {
            var result = await _collection.FindAsync(filterExpression, null, cancellationToken);
            var resultList = await result.ToListAsync(cancellationToken);
            return resultList != null && resultList.Any() ? resultList : Enumerable.Empty<TDocument>();
        }
    }
}
