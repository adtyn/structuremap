using Shouldly;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using System;
using System.Linq;
using Xunit;

namespace StructureMap.Testing.Configuration.DSL
{
    public class GenericFamilyExpressionTester
    {
        public interface ITarget
        {
        }

        public class Target1 : ITarget
        {
        }

        public class Target2 : ITarget
        {
        }

        public class Target3 : ITarget
        {
        }

        public class WrappedTarget : ITarget
        {
            private readonly ITarget _inner;

            public WrappedTarget(ITarget target)
            {
                _inner = target;
            }

            public ITarget Inner
            {
                get { return _inner; }
            }
        }

        public interface IRepository<T>
        {
            void Save(T subject);
        }

        public class OnlineRepository<T> : IRepository<T>
        {
            #region IRepository<T> Members

            public void Save(T subject)
            {
                throw new NotImplementedException();
            }

            #endregion IRepository<T> Members
        }

        public class OfflineRepository<T> : IRepository<T>
        {
            #region IRepository<T> Members

            public void Save(T subject)
            {
                throw new NotImplementedException();
            }

            #endregion IRepository<T> Members
        }

        public class Invoice
        {
        }

        [Fact]
        public void Add_concrete_type()
        {
            var container =
                new Container(
                    r => r.For(typeof(ITarget)).Add(typeof(Target1)));

            container.GetAllInstances<ITarget>()
                .First()
                .ShouldBeOfType<Target1>();
        }

        [Fact]
        public void Add_concrete_type_with_name()
        {
            var container = new Container(r =>
            {
                r.For(typeof(ITarget)).Add(typeof(Target1)).Named("1");
                r.For(typeof(ITarget)).Add(typeof(Target2)).Named("2");
                r.For(typeof(ITarget)).Add(typeof(Target3)).Named("3");
            });

            container.GetInstance<ITarget>("1").ShouldBeOfType<Target1>();
            container.GetInstance<ITarget>("2").ShouldBeOfType<Target2>();
            container.GetInstance<ITarget>("3").ShouldBeOfType<Target3>();
        }

        [Fact]
        public void Add_default_by_concrete_type()
        {
            var container =
                new Container(
                    r => r.For(typeof(ITarget)).Use(typeof(Target3)));

            container.GetInstance<ITarget>().ShouldBeOfType<Target3>();
        }

        [Fact]
        public void Add_default_instance()
        {
            var container =
                new Container(r => { r.For(typeof(ITarget)).Use(typeof(Target2)); });

            container.GetInstance<ITarget>().ShouldBeOfType<Target2>();
        }

        [Fact]
        public void Add_instance_directly()
        {
            var container = new Container(r => { r.For<ITarget>().Add<Target2>(); });

            container.GetAllInstances<ITarget>()
                .First().ShouldBeOfType<Target2>();
        }

        [Fact]
        public void Set_caching()
        {
            var registry = new Registry();
            registry.For(typeof(ITarget), Lifecycles.ThreadLocal);
            var graph = registry.Build();

            graph.Families[typeof(ITarget)].Lifecycle.ShouldBeOfType<ThreadLocalStorageLifecycle>();
        }
    }
}