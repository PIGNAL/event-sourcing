﻿using System.Linq.Expressions;
using MongoDB.Driver;
using Ticketing.Command.Domain.Common;

namespace Ticketing.Command.Domain.Abstracts
{
    public interface IMongoRepository<TDocument> : ISession where TDocument : IDocument
    {
        IQueryable<TDocument> AsQueryable();

        Task InsertOneAsync(TDocument document, IClientSessionHandle clientSessionHandle, CancellationToken cancellationToken);

        Task<IEnumerable<TDocument>> FilterByAsync(Expression<Func<TDocument, bool>> filterExpression, CancellationToken cancellationToken);
    }
}
