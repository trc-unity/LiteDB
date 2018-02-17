﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LiteDB
{
    internal class IndexStartsWith : Index
    {
        private BsonValue _value;

        public IndexStartsWith(string name, BsonValue value)
            : base(name)
        {
            _value = value;
        }

        internal override IEnumerable<IndexNode> Execute(IndexService indexer, CollectionIndex index)
        {
            // find first indexNode
            var node = indexer.Find(index, _value, true, Query.Ascending);
            var str = _value.AsString;

            // navigate using next[0] do next node - if less or equals returns
            while (node != null)
            {
                var valueString = node.Key.AsString;

                // value will not be null because null occurs before string (bsontype sort order)
                if (valueString.StartsWith(str))
                {
                    if (!node.DataBlock.IsEmpty)
                    {
                        yield return node;
                    }
                }
                else
                {
                    break; // if no more starts with, stop scanning
                }

                node = indexer.GetNode(node.Next[0]);
            }
        }

        public override string ToString()
        {
            return string.Format("STARTSWITH({0})", this.Name);
        }
    }
}