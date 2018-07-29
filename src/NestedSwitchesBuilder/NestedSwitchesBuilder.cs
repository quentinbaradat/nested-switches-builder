// NestedSwitchesBuilder.cs
//
// Copyright © 2018 Quentin Baradat
//
// Licensed under the Apache License, Version 2.0 <LICENSE-APACHE or
// http://www.apache.org/licenses/LICENSE-2.0> or the MIT license
// <LICENSE-MIT or http://opensource.org/licenses/MIT>, at your
// option. This file may not be copied, modified, or distributed
// except according to those terms.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NestedSwitchesBuilder
{
    public class NestedSwitchesBuilder<TKey, TValue>
    {
        private readonly NestedSwitchesBuilder<TKey, TValue> _parent;
        private readonly IDictionary<TKey, NestedSwitchesBuilder<TKey, TValue>> _children;
        private readonly int _index;

        public TKey Key { get; set; }
        public TValue Value { get; set; }

        private Expression KeyExpression
        {
            get { return Expression.Property(Expression.Constant(this), "Key"); }
        }

        private Expression ValueExpression
        {
            get { return Expression.Property(Expression.Constant(this), "Value"); }
        }

        public NestedSwitchesBuilder() : this(null, 0) { }

        public NestedSwitchesBuilder(NestedSwitchesBuilder<TKey, TValue> parent, int index)
        {
            _parent = parent;
            _children = new Dictionary<TKey, NestedSwitchesBuilder<TKey, TValue>>();
            _index = index;
        }

        public Func<IEnumerable<TKey>, TValue> Build()
        {
            ParameterExpression keysParameter = Expression.Parameter(typeof(IEnumerable<TKey>), "keys");

            LabelTarget returnTarget = Expression.Label(typeof(TValue));

            BlockExpression body;
            if (_children.Count == 0)
            {
                body = Expression.Block(
                    Expression.Return(returnTarget, Expression.Default(typeof(TValue)), typeof(TValue)),
                    Expression.Label(returnTarget, Expression.Default(typeof(TValue)))
             );

                return Expression.Lambda<Func<IEnumerable<TKey>, TValue>>(body, keysParameter).Compile();
            }

            body = Expression.Block(
                Compile(returnTarget, keysParameter),
                Expression.Label(returnTarget, Expression.Default(typeof(TValue)))
            );

            TryExpression tryCatchExpr = Expression.TryCatch(
                Expression.Block(body),
                Expression.Catch(
                    typeof(Exception),
                    Expression.Block(
                        Expression.Return(returnTarget, Expression.Default(typeof(TValue)), typeof(TValue)),
                        Expression.Label(returnTarget, Expression.Default(typeof(TValue)))
                    )
                )
            );

            return Expression.Lambda<Func<IEnumerable<TKey>, TValue>>(tryCatchExpr, keysParameter).Compile();
        }

        public void Add(IList<TKey> keys, TValue value, bool replace = true)
        {
            if (_index - keys.Count >= 0)
            {
                if (replace)
                    Value = value;
            }
            else
            {
                TKey key = keys[_index];
                NestedSwitchesBuilder<TKey, TValue> child;

                if (!_children.TryGetValue(key, out child))
                {
                    child = new NestedSwitchesBuilder<TKey, TValue>(this, _index + 1) { Key = key };
                    _children.Add(key, child);
                }

                child.Add(keys, value, replace);
            }
        }

        private SwitchExpression Compile(LabelTarget returnTarget, Expression keysParameter, int level = 0)
        {
            var switchValue = Expression.Call(
                            null,
                            typeof(Enumerable).GetMethod("ElementAt").MakeGenericMethod(typeof(TKey)),
                            keysParameter,
                            Expression.Constant(level, typeof(int))
                            );

            var defaultCase = Expression.Return(returnTarget, Expression.Default(typeof(TValue)), typeof(TValue));

            var switchCases = _children.Values.Select(x => x.ToSwitchCase(returnTarget, keysParameter, level + 1)).ToArray();

            return Expression.Switch(
                            switchValue,
                            defaultCase,
                            switchCases
                            );
        }

        private SwitchCase ToSwitchCase(LabelTarget returnTarget, Expression keysParameter, int level)
        {
            if (Value != null)
            {
                return Expression.SwitchCase(Expression.Return(returnTarget, Expression.Constant(Value), typeof(TValue)),
                                    KeyExpression
                                );
            }
            else
            {
                return Expression.SwitchCase(
                                    Compile(returnTarget, keysParameter, level),
                                    KeyExpression
                                );
            }
        }
    }
}