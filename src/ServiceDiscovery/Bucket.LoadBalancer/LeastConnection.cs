using Bucket.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bucket.LoadBalancer
{
    public class LeastConnection : ILoadBalancer
    {
        private readonly Func<Task<IList<ServiceInformation>>> _services;
        private readonly List<Lease> _leases;
        private readonly string _serviceName;
        private static readonly object _syncLock = new object();

        public LeastConnection(Func<Task<IList<ServiceInformation>>> services, string serviceName)
        {
            _services = services;
            _serviceName = serviceName;
            _leases = new List<Lease>();
        }

        public async Task<HostAndPort> Lease()
        {
            var services = await _services.Invoke();

            if (services == null)
                throw new ArgumentNullException($"{_serviceName}");

            if (!services.Any())
                throw new ArgumentNullException($"{_serviceName}");

            lock (_syncLock)
            {
                //todo - maybe this should be moved somewhere else...? Maybe on a repeater on seperate thread? loop every second and update or something?
                UpdateServices(services);

                var leaseWithLeastConnections = GetLeaseWithLeastConnections();

                _leases.Remove(leaseWithLeastConnections);

                leaseWithLeastConnections = AddConnection(leaseWithLeastConnections);

                _leases.Add(leaseWithLeastConnections);

                return new HostAndPort(leaseWithLeastConnections.HostAndPort.Address, leaseWithLeastConnections.HostAndPort.Port);
            }
        }

        public void Release(HostAndPort hostAndPort)
        {
            lock (_syncLock)
            {
                var matchingLease = _leases.FirstOrDefault(l => l.HostAndPort.Address == hostAndPort.Address
                    && l.HostAndPort.Port == hostAndPort.Port);

                if (matchingLease != null)
                {
                    var replacementLease = new Lease(hostAndPort, matchingLease.Connections - 1);

                    _leases.Remove(matchingLease);

                    _leases.Add(replacementLease);
                }
            }
        }

        private Lease AddConnection(Lease lease)
        {
            return new Lease(lease.HostAndPort, lease.Connections + 1);
        }

        private Lease GetLeaseWithLeastConnections()
        {
            //now get the service with the least connections?
            Lease leaseWithLeastConnections = null;

            for (var i = 0; i < _leases.Count; i++)
            {
                if (i == 0)
                {
                    leaseWithLeastConnections = _leases[i];
                }
                else
                {
                    if (_leases[i].Connections < leaseWithLeastConnections.Connections)
                    {
                        leaseWithLeastConnections = _leases[i];
                    }
                }
            }

            return leaseWithLeastConnections;
        }

        private bool UpdateServices(IList<ServiceInformation> services)
        {
            if (_leases.Count > 0)
            {
                var leasesToRemove = new List<Lease>();

                foreach (var lease in _leases)
                {
                    var match = services.FirstOrDefault(s => s.HostAndPort.Address == lease.HostAndPort.Address
                        && s.HostAndPort.Port == lease.HostAndPort.Port);

                    if (match == null)
                    {
                        leasesToRemove.Add(lease);
                    }
                }

                foreach (var lease in leasesToRemove)
                {
                    _leases.Remove(lease);
                }

                foreach (var service in services)
                {
                    var exists = _leases.FirstOrDefault(l => l.HostAndPort.ToString() == service.HostAndPort.ToString());

                    if (exists == null)
                    {
                        _leases.Add(new Lease(service.HostAndPort, 0));
                    }
                }
            }
            else
            {
                foreach (var service in services)
                {
                    _leases.Add(new Lease(service.HostAndPort, 0));
                }
            }

            return true;
        }
    }
}
