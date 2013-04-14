// Copyright 2011-2013 Anodyne.
//   
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
//  
//      http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

namespace Kostassoid.Anodyne.Domain.DataAccess
{
	using System;
	using Abstractions.DataAccess;
	using Policy;

	public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IDataSessionFactory DataSessionFactory { get; internal set; }

	    public UnitOfWorkFactory(IDataSessionFactory sessionFactory)
	    {
		    DataSessionFactory = sessionFactory;
	    }

		public IUnitOfWork Build(StaleDataPolicy staleDataPolicy)
		{
			return new UnitOfWorkContext(OpenDomainSession(), staleDataPolicy);
		}

        public IUnitOfWork Build(IUnitOfWork parent)
		{
			return new UnitOfWorkContext(parent);
		}

	    public IDomainDataSession OpenDomainSession()
	    {
			var session = new DomainDataSession(DataSessionFactory.Open());
			if (session == null)
				throw new Exception("Unable to create IDataSession (bad configuration?)");

		    return session;
	    }
    }
}