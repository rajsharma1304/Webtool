///<Copyright> Cross-Tab  </Copyright>
///<ProjectName>SICT </ProjectName>
///<FileName> CorsWebServiceHostFactory.cs </FileName>
///<CreatedOn> 6 Jan 2015</CreatedOn>

using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace SICT
{
    public class CorsWebServiceHostFactory : WebServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceHost host = base.CreateServiceHost(serviceType, baseAddresses);
            host.Opening += new EventHandler(host_Opening);
            return host;
        }

        private void host_Opening(object sender, EventArgs e)
        {
            var endpoints = (sender as ServiceHost).Description.Endpoints.Where(se => se.Binding is WebHttpBinding);

            foreach (var endpoint in endpoints)
            {
                // Add support for cross-origin resource sharing
                endpoint.Behaviors.Add(new CorsSupportBehavior());
            }
        }
    }
}