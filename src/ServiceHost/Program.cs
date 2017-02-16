namespace PurchaseApproval.ServiceHost
{
    using System;
    using Castle.Windsor;
    using Topshelf;

    internal class Program
    {
        internal static void Main(string[] args)
        {
            const string serviceName = "PurchaseApprovalService-v2";
            const string serviceDisplayName = "MobileLife PurchaseApproval Service";
            const string serviceDescription = "Service for PurchaseApprovals in Sunday";

            var container = new WindsorContainer().Install(new ServiceInstaller());

            HostFactory.Run(hostConfigurator =>
            {
                hostConfigurator.Service<IPurchaseApprovalWindowsService>(serviceConfigurator =>
                {
                    serviceConfigurator.ConstructUsing(name => container.Resolve<IPurchaseApprovalWindowsService>());
                    serviceConfigurator.WhenStarted(service =>
                    {
                        service.Start();
                        Console.WriteLine($"Service started. Listening on '{service.ServiceUri}'.");
                    });
                    serviceConfigurator.WhenStopped(service =>
                    {
                        service.Stop();
                        container.Release(service);
                        container.Dispose();
                    });
                });

                hostConfigurator.SetServiceName(serviceName);
                hostConfigurator.SetDisplayName(serviceDisplayName);
                hostConfigurator.SetDescription(serviceDescription);
            });
        }
    }
}
