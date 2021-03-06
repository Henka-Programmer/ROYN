﻿using Newtonsoft.Json;
using ROYN.Extensions;
using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace ROYN
{
    //public class RoynProperty
    //{
    //    public string Name { get; set; }
    //    public Type Type { get; set; }
    //    public RoynType Parent { get; private set; }
    //}

    //public class RoynType
    //{
    //    public string Name { get; private set; }
    //    private List<RoynProperty> _Properties = new List<RoynProperty>();
    //    public IReadOnlyList<RoynProperty> Properties { get { return _Properties; } }
    //}

    [DataContract]
    public enum SortDirection
    {
        [EnumMember]
        Ascending,

        [EnumMember]
        Descending
    }

    [DataContract]
    public class TypeName
    {
        [DataMember]
        public string Name { get; protected set; }

        [DataMember]
        public ResolveOptions ResolveOption { get; protected set; }

        [JsonConstructor]
        internal TypeName(string name, ResolveOptions resolveOption)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("message", nameof(name));
            }

            Name = name;
            ResolveOption = resolveOption;
        }

        public TypeName(Type type, ResolveOptions resolveOption)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            ResolveOption = resolveOption;
            switch (resolveOption)
            {
                case ResolveOptions.Name:
                    Name = type.Name;
                    break;

                case ResolveOptions.FullName:
                    Name = type.FullName;
                    break;

                case ResolveOptions.AssemblyQualifiedName:
                    Name = type.AssemblyQualifiedName;
                    break;

                default:
                    Name = type.Name;
                    break;
            }
        }
    }

    [DataContract]
    public class RoynRequest
    {
        [DataMember]
        public TypeName TypeName { get; protected set; }

        [JsonIgnore]
        public Type CLRType { get; internal set; }

        [DataMember]
        [JsonProperty]
        protected readonly List<string> _columns;

        [JsonIgnore]
        internal int? InternalSkipSize { get { return SkipSize; } }

        [JsonIgnore]
        internal int? InternalTakeSize { get { return TakeSize; } }

        internal List<string> Columns { get { return _columns; } }

        [JsonProperty]
        [DataMember]
        protected string Filter { get; set; }

        [JsonIgnore]
        internal string InternalFilter { get { return Filter; } }

        [DataMember]
        protected int? SkipSize = null;

        [DataMember]
        protected int? TakeSize = null;

        internal Dictionary<string, SortDirection> InternalOrders { get { return Orders; } }

        [JsonProperty]
        [DataMember]
        protected readonly Dictionary<string, SortDirection> Orders = new Dictionary<string, SortDirection>();

        public RoynRequest(TypeName typeName)
        {
            _columns = new List<string>();
            TypeName = typeName;
        }

        internal virtual RoynRequest<T> AsGeneric<T>()
        {
            var g = new RoynRequest<T>();
            CopyTo(g);
            return g;
        }

        internal void CopyTo(RoynRequest roynRequest)
        {
            roynRequest._columns.Clear();
            roynRequest._columns.AddRange(_columns);
            roynRequest.CLRType = CLRType;
            roynRequest.Filter = Filter;
            roynRequest.Orders.Clear();
            roynRequest.SkipSize = SkipSize;
            roynRequest.TakeSize = TakeSize;
            foreach (var order in Orders)
            {
                roynRequest.Orders.Add(order.Key, order.Value);
            }
            if (TypeName != null)
            {
                roynRequest.TypeName = new TypeName(TypeName.Name, TypeName.ResolveOption);
            }
        }

        protected RoynRequest Where(Expression expression)
        {
            if (!string.IsNullOrEmpty(Filter))
            {
                throw new InvalidOperationException("Multiple Where Clause Not Supported");
            }
            if (expression == null) throw new ArgumentNullException("expression");
            ExpressionSerializer Serializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer(), new Serialize.Linq.Factories.FactorySettings { AllowPrivateFieldAccess = true });
            Filter = Serializer.SerializeText(expression);
            return this;
        }
    }

    public class RoynRequest<T> : RoynRequest
    {
        public RoynRequest<T> Where(Expression<Func<T, bool>> expression)
        {
            return base.Where(expression) as RoynRequest<T>;
        }

        public RoynRequest<T> OrderBy<TValue>(Expression<Func<T, TValue>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var propertyPath = expression.GetPropertyPath();
            if (Orders.ContainsKey(propertyPath))
            {
                Orders[propertyPath] = SortDirection.Ascending;
            }
            else
            {
                Orders.Add(propertyPath, SortDirection.Ascending);
            }
            return this;
        }

        public RoynRequest<T> OrderByDescending<TValue>(Expression<Func<T, TValue>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var propertyPath = expression.GetPropertyPath();

            if (Orders.ContainsKey(propertyPath))
            {
                Orders[propertyPath] = SortDirection.Descending;
            }
            else
            {
                Orders.Add(propertyPath, SortDirection.Descending);
            }
            return this;
        }

        public RoynRequest<T> Add<TValue>(Expression<Func<T, TValue>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var pInfo = selector.GetPropertyInfo();
            if (pInfo.IsAttributeDefined<NotMappedAttribute>())
            {
                throw new InvalidOperationException($"{pInfo.Name} Marked as Not Mapped Property, Couldn't Included into a Royn query");
            }
            var p = selector.GetPropertyPath();
            _columns.Add(p);
            return this;
        }

        public RoynRequest<T> Add(string property)
        {
            _columns.Add(property);
            return this;
        }

        public RoynRequest<T> Skip(int size)
        {
            if (Orders.Count == 0)
            {
                throw new InvalidOperationException("Skip and Take must be called after OrderBy");
            }
            SkipSize = size;
            return this;
        }

        public RoynRequest<T> Take(int size)
        {
            if (Orders.Count == 0)
            {
                throw new InvalidOperationException("Skip and Take must be called after OrderBy");
            }

            if (size < 0)
            {
                throw new ArgumentOutOfRangeException($"Argument '{nameof(size)}' must be greater than zero ");
            }

            if (SkipSize == null)
            {
                throw new InvalidOperationException("Take must be called after Skip");
            }
            TakeSize = size;
            return this;
        }

        public RoynRequest() : base(new TypeName(typeof(T), ResolveOptions.FullName))
        {
            CLRType = typeof(T);
        }
    }
}