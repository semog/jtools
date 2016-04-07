﻿using System;
using System.Linq;

namespace LiteDB
{
    internal partial class DbEngine : IDisposable
    {
        /// <summary>
        /// Returns first value from an index (first is min value)
        /// </summary>
        public BsonValue Min(string colName, string field)
        {
            lock (_locker)
            {
                // get collection page (no col, no min)
                var col = this.GetCollectionPage(colName, false);

                if (col == null) return BsonValue.MinValue;

                // get index (no index, no min)
                var index = col.GetIndex(field);

                if (index == null) return BsonValue.MinValue;

                var head = _indexer.GetNode(index.HeadNode);
                var next = _indexer.GetNode(head.Next[0]);

                if (next.IsHeadTail(index)) return BsonValue.MinValue;

                return next.Key;
            }
        }

        /// <summary>
        /// Returns last value from an index (last is max value)
        /// </summary>
        public BsonValue Max(string colName, string field)
        {
            lock (_locker)
            {
                // get collection page (no col, no max)
                var col = this.GetCollectionPage(colName, false);

                if (col == null) return BsonValue.MaxValue;

                // get index (no index, no max)
                var index = col.GetIndex(field);

                if (index == null) return BsonValue.MaxValue;

                var tail = _indexer.GetNode(index.TailNode);
                var prev = _indexer.GetNode(tail.Prev[0]);

                if (prev.IsHeadTail(index)) return BsonValue.MaxValue;

                return prev.Key;
            }
        }

        /// <summary>
        /// Count all nodes from a query execution - do not deserialize documents to count
        /// </summary>
        public int Count(string colName, Query query)
        {
            lock (_locker)
            {
                // get collection page (no col, returns 0)
                var col = this.GetCollectionPage(colName, false);

                if (col == null) return 0;

                if (query == null) return (int)col.DocumentCount;

                // run query in this collection
                var nodes = query.Run(col, _indexer);

                // count all nodes
                return nodes.Count();
            }
        }

        /// <summary>
        /// Check if has at least one node in a query execution - do not deserialize documents to check
        /// </summary>
        public bool Exists(string colName, Query query)
        {
            lock (_locker)
            {
                // get collection page (no col, not exists)
                var col = this.GetCollectionPage(colName, false);

                if (col == null) return false;

                // run query in this collection
                var nodes = query.Run(col, _indexer);

                // check if has at least first
                return nodes.FirstOrDefault() != null;
            }
        }
    }
}