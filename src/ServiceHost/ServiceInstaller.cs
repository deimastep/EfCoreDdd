namespace PurchaseApproval.ServiceHost
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    internal class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IPurchaseApprovalWindowsService>()
                .ImplementedBy<PurchaseApprovalWindowsService>().LifestyleTransient());
        }
    }
}