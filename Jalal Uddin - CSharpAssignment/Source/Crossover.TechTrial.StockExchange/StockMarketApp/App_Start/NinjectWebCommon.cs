[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(StockMarketApp.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(StockMarketApp.App_Start.NinjectWebCommon), "Stop")]

namespace StockMarketApp.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Modules;
    using StockMarketSharedLibrary;
    using StockMarketApp.Areas.MyAccount.Models;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            return Container;
        }

        public static IKernel _container;
        private static IKernel Container
        {
            get
            {
                if (_container == null)
                {
                    _container = new StandardKernel();
                    _container.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                    _container.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                    RegisterServices(_container);
                }
                return _container;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Load(new INinjectModule[]
            {
                
            });

            kernel.Bind<IStockAppUnitOfWork>().To<StockAppUnitOfWork>().InSingletonScope();
            kernel.Bind<StockAppContext>().ToSelf();
            kernel.Bind<IStockSymbolService>().To<StockSymbolService>();
            kernel.Bind <ILogHelperFactory>().To<Log4NetLogHelperFactory>().InSingletonScope();
            kernel.Bind<IDbCommandExecutionService>().To<DbCommandExecutionService>().InSingletonScope();
            kernel.Bind<IDbCommandFactory>().To<SqlCommandFactory>().InSingletonScope().WithConstructorArgument("connectionString",
                new ConnectionStringReader().ConnectionString);
        }

        /// <summary>
        /// Creates concrete class instace
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetConcreteInstance<T>()
        {
            object instance = Container.TryGet<T>();
            if (instance != null)
                return (T)instance;
            throw new InvalidOperationException(string.Format("Unable to create an instance of {0}", typeof(T).FullName));
        }
    }
}
