using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WellServiceAPI
{
    public static class DecoratorServiceCollectionExtensions
    {
        public static void AddDecorator<TService, TDecorator>(
            this IServiceCollection serviceCollection,
            Action<IServiceCollection> configureDecorateeServices)
            where TDecorator : class, TService
            where TService : class
        {
            var decorateeServices = new ServiceCollection();

            configureDecorateeServices(decorateeServices);

            var decorateeDescriptor =
                decorateeServices.SingleOrDefault(sd => sd.ServiceType == typeof(TService));

            if (decorateeDescriptor == null)
            {
                throw new InvalidOperationException("No decoratee configured!");
            }

            decorateeServices.Remove(decorateeDescriptor);

            serviceCollection.Add(decorateeServices);

            var decoratorInstanceFactory = ActivatorUtilities.CreateFactory(
                typeof(TDecorator), new[] { typeof(TService) });

            Type decorateeImplType = decorateeDescriptor.GetImplementationType();

            Func<IServiceProvider, TDecorator> decoratorFactory = sp =>
            {
                var decoratee = sp.GetRequiredService(decorateeImplType);
                var decorator = (TDecorator)decoratorInstanceFactory(sp, new[] { decoratee });
                return decorator;
            };

            var decoratorDescriptor = ServiceDescriptor.Describe(
                typeof(TService),
                decoratorFactory,
                decorateeDescriptor.Lifetime);

            decorateeDescriptor = RefactorDecorateeDescriptor(decorateeDescriptor);

            serviceCollection.Add(decorateeDescriptor);
            serviceCollection.Add(decoratorDescriptor);
        }

        private static ServiceDescriptor RefactorDecorateeDescriptor(ServiceDescriptor decorateeDescriptor)
        {
            var decorateeImplType = decorateeDescriptor.GetImplementationType();

            if (decorateeDescriptor.ImplementationFactory != null)
            {
                decorateeDescriptor =
                    ServiceDescriptor.Describe(
                    serviceType: decorateeImplType,
                    decorateeDescriptor.ImplementationFactory,
                    decorateeDescriptor.Lifetime);
            }
            else
            if (decorateeDescriptor.ImplementationInstance != null)
            {
                decorateeDescriptor =
                    ServiceDescriptor.Singleton(
                    serviceType: decorateeImplType,
                    decorateeDescriptor.ImplementationInstance);
            }
            else
            {
                decorateeDescriptor =
                    ServiceDescriptor.Describe(
                    decorateeImplType, 
                    decorateeImplType,
                    decorateeDescriptor.Lifetime);
            }

            return decorateeDescriptor;
        }

    
        private static Type GetImplementationType(this ServiceDescriptor serviceDescriptor)
        {
            if (serviceDescriptor.ImplementationType != null)
                return serviceDescriptor.ImplementationType;

            if (serviceDescriptor.ImplementationInstance != null)
                return serviceDescriptor.ImplementationInstance.GetType();

            if (serviceDescriptor.ImplementationFactory != null)
                return serviceDescriptor.ImplementationFactory.GetType().GenericTypeArguments[1];

            throw new InvalidOperationException("No way to get the decoratee implementation type.");
        }
    }
}
